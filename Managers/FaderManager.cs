using Il2Cpp;
using Il2CppSpine;
using Il2CppSpine.Unity;
using UnityEngine;
using Logger = FadeIn.Utilities.Logger;

namespace FadeIn.Managers;

internal interface IFader
{
    bool StopUpdating { get; }

    void Start();
    void Update();
}

internal interface IInterpolator
{
    float Interpolate(float x, float min, float max);
}

internal interface ITransformation
{
    public float Transform(float x, float min, float max);
}

#region classes

internal abstract class AirEnemyFader : IFader
{
    #region properties
    public bool StopUpdating { get; private set; }
    private bool IsActive => NoteObject is not null && NoteObject.active;
    private GameObject NoteObject { get; init; }
    private Skeleton NoteSkeleton { get; init; }
    private Transform NoteTransform { get; init; }
    private AlphaCalculator XCalculator { get; init; }
    #endregion

    private static readonly Logger logger = new(nameof(AirEnemyFader));

    internal AirEnemyFader(BaseEnemyObjectController beoc)
    {
        logger.Debug(beoc.name);
        logger.Debug(beoc.m_NodeType);
        NoteObject = beoc.gameObject;
        logger.Debug("Got game object");
        NoteSkeleton = beoc.m_SkeletonAnimation.skeleton;
        logger.Debug("Got skeleton");
        NoteTransform = NoteObject.transform;
        logger.Debug("Got transform");
        XCalculator = FadeManager.GetXAlphaCalculator();
        StopUpdating = false;
    }

    public abstract void Start();

    public void Update()
    {
        if (!IsActive)
        {
            StopUpdating = true;
            return;
        }

        NoteSkeleton.a = XCalculator.GetAlpha(NoteTransform.position.x);
    }
}

internal class AlphaCalculator
{
    private const float AlphaLowerLimit = 0.005f;
    private const float AlphaUpperLimit = 0.995f;

    private readonly ValueRange AlphaRange;
    private readonly IInterpolator Interpolator;
    private readonly ValueRange PositionRange;
    private readonly ITransformation Transform;

    internal AlphaCalculator(
        IInterpolator interpolator,
        ITransformation transform,
        ValueRange positionRange,
        ValueRange alphaRange
    )
    {
        /*
        PositionRange should contain as Start value the lowest value of the two
        and End value as the biggest value.
        In the context of the game, the start is the leftmost part of the screen
        and the end is the rightmost part of the screen.
        */
        /*
        AlphaRange should contain as Start value the initial value we should have
        on the object as alpha (0 in case of FadeOut, 1 in case of FadeIn) which
        is the value we will clamp to if the position of the object is to the left
        of PositionRange.End.
        The End value should have the value that we will clamp to if the object
        is to the left of PositionRange.Start.
        * AAAA
        */
        (Interpolator, Transform, PositionRange, AlphaRange) = (
            interpolator,
            transform,
            positionRange,
            alphaRange
        );
    }

    public float GetAlpha(float position)
    {
        if (position > PositionRange.Right)
        {
            return AlphaRange.Left;
        }
        if (position < PositionRange.Left)
        {
            return AlphaRange.Right;
        }

        var alpha = CalculateAlpha(position);
        return CalculateAlpha(position);
    }

    private float CalculateAlpha(float position)
    {
        // Min and max correspond to the settings
        var min = PositionRange.Left;
        var max = PositionRange.Right;
        var transformedPosition = Transform.Transform(position, min, max);
        var newAlpha = Interpolator.Interpolate(transformedPosition, min, max);
        if (newAlpha < AlphaLowerLimit)
            return 0f;
        if (newAlpha > AlphaUpperLimit)
            return 1f;
        return newAlpha;
    }
}

internal class ExponentialInterpolator : IInterpolator
{
    private const float ExpDecayConst = 1.5f;

    private static readonly float ExpNorm = Mathf.Exp(ExpDecayConst) - 1;

    public float Interpolate(float x, float min, float max)
    {
        var top = Mathf.Exp(ExpDecayConst * x / (max - min)) - 1;
        return top / ExpNorm;
    }
}

internal static class FadeManager
{
    private static readonly IInterpolator Exponential = new ExponentialInterpolator();
    private static readonly ITransformation InTransform = new InTransformation();
    private static readonly IInterpolator Linear = new LinearInterpolator();
    private static readonly ITransformation NoTransform = new NoTransformation();
    private static readonly ITransformation OutTransform = new OutTransformation();

    internal static AlphaCalculator GetRAlphaCalculator()
    {
        GetParameters(
            out var interpolator,
            out var transform,
            out var parameters,
            out var alphaRange
        );

        var rangeParameters = new ValueRange(parameters.LeftR, parameters.RightR);

        return new AlphaCalculator(interpolator, transform, rangeParameters, alphaRange);
    }

    internal static AlphaCalculator GetXAlphaCalculator()
    {
        GetParameters(
            out var interpolator,
            out var transform,
            out var parameters,
            out var alphaRange
        );

        var rangeParameters = new ValueRange(parameters.LeftX, parameters.RightX);

        return new AlphaCalculator(interpolator, transform, rangeParameters, alphaRange);
    }

    private static void GetParameters(
        out IInterpolator interpolator,
        out ITransformation transform,
        out FadeParameters parameters,
        out ValueRange alphaRange
    )
    {
        interpolator = SettingsManager.FadeDecay switch
        {
            SettingsManager.Decay.Exponential => Exponential,
            _ => Linear
        };

        switch (SettingsManager.FadeMode)
        {
            case SettingsManager.Mode.FadeIn:
                transform = InTransform;
                parameters = SettingsManager.InSettings;
                alphaRange = new ValueRange(0, 1);
                break;
            default:
                transform = OutTransform;
                parameters = SettingsManager.OutSettings;
                alphaRange = new ValueRange(1, 0);
                break;
        }
    }
}

internal class HealthFader : AirEnemyFader
{
    private AirEnergyBottleController Controller { get; init; }

    public HealthFader(BaseEnemyObjectController beoc)
        : base(beoc)
    {
        Controller = beoc.Cast<AirEnergyBottleController>();
    }

    public override void Start()
    {
        Controller.m_Fx.SetActive(false);
    }
}

internal class InTransformation : ITransformation
{
    public float Transform(float x, float min, float max) => max - x;
}

internal class LinearInterpolator : IInterpolator
{
    public float Interpolate(float x, float min, float max) => x / (max - min);
}

internal class NormalEnemyFader : IFader
{
    #region properties
    public bool StopUpdating { get; private set; }
    private Il2CppSystem.Collections.Generic.Dictionary<string, Bone>? Bones { get; set; }
    private BaseEnemyObjectController Controller { get; init; }
    private GameObject EnemyObject { get; init; }
    private List<Skeleton> EnemySkeletons { get; init; }
    private Skeleton? HealthSkeleton { get; init; }
    private bool IsActive => EnemyObject is not null && EnemyObject.active;
    private AlphaCalculator RCalculator { get; init; }
    private Bone? XBone { get; set; }
    private AlphaCalculator XCalculator { get; init; }
    private Bone? YBone { get; set; }
    #endregion

    private static readonly Logger logger = new(nameof(NormalEnemyFader));

    internal NormalEnemyFader(BaseEnemyObjectController beoc)
    {
        var startName = beoc.name;
        logger.Debug(startName);
        logger.Debug(beoc.m_NodeType);
        Controller = beoc;
        EnemyObject = beoc.gameObject;
        EnemySkeletons = new List<Skeleton> { beoc.m_SkeletonAnimation.skeleton };
        Bones = beoc.m_Sac.bones;
        if (Bones is null)
        {
            logger.Debug("Bones are null.");
        }
        StopUpdating = false;
        XCalculator = FadeManager.GetXAlphaCalculator();
        RCalculator = FadeManager.GetRAlphaCalculator();

        // Handle hearts on notes
        var heartOnNote = beoc?.m_Blood;
        HealthSkeleton = heartOnNote?.GetComponent<SkeletonAnimation>().skeleton;
        if (heartOnNote is not null)
        {
            var heartTransform = heartOnNote.transform;
            for (var i = 0; i < heartTransform.childCount; i++)
            {
                var child = heartTransform.GetChild(i);
                child.gameObject.SetActive(false);
            }
        }
    }

    public void Start()
    {
        logger.Debug("Starting: " + EnemyObject.name);
        if (Bones is not null)
        {
            return;
        }

        Bones = Controller.m_Sac.bones;
        XBone = Bones["X"];
        YBone = Bones["Y"];

        // TODO: try to do all of these on the constructor...
        // * Somewhere between the init and start the parent changes....
        // Add siblings if it has multiple skins
        // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        // ! We are still generating NormalEnemyFaders for the siblings, they are simply not being activated
        // ! There may be another way of handling that...
        // SceneChangeController may be relevant here...
        var parent = Controller.transform.parent;
        var parentName = Controller.name.Replace("(Clone)", "");
        logger.Debug(parentName + " " + parent.name + " " + parentName.Equals(parent.name));
        if (parentName.Equals(parent.name))
        {
            logger.Debug("Has skin siblings!");
            EnemySkeletons.Clear();
            for (var i = 0; i < parent.childCount; i++)
            {
                var skeleton = parent.GetChild(i).GetComponent<SkeletonAnimation>()?.skeleton;
                if (skeleton is not null)
                {
                    EnemySkeletons.Add(skeleton);
                }
            }

            logger.Debug(EnemySkeletons.Count + " siblings!");
        }
    }

    public void Update()
    {
        if (!IsActive || XBone is null)
        {
            StopUpdating = true;
            return;
        }

        var alpha = XCalculator.GetAlpha(XBone.x);
        // TODO: Handle rotating enemies...

        EnemySkeletons.ForEach(sk => sk.a = alpha);
        if (HealthSkeleton is not null)
        {
            HealthSkeleton.a = alpha;
        }
    }
}

internal class NoteFader : AirEnemyFader
{
    private AirMusicNodeController Controller { get; init; }

    public NoteFader(BaseEnemyObjectController beoc)
        : base(beoc)
    {
        Controller = beoc.Cast<AirMusicNodeController>();
    }

    public override void Start()
    {
        Controller.m_Fx.SetActive(false);
    }
}

internal class NoTransformation : ITransformation
{
    public float Transform(float x, float min, float max) => x;
}

internal class OutTransformation : ITransformation
{
    public float Transform(float x, float min, float max) => x - min;
}

#endregion
