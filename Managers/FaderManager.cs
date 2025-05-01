using UnityEngine;

namespace FadeIn.Managers;

internal interface IFader
{
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

    internal static AlphaCalculator GetAlphaCalculator()
    {
        var interpolator = SettingsManager.FadeDecay switch
        {
            SettingsManager.Decay.Exponential => Exponential,
            _ => Linear
        };

        ITransformation transform;
        FadeParameters parameters;
        ValueRange alphaRange;
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

        var rangeParameters = new ValueRange(parameters.LeftX, parameters.RightX);

        return new AlphaCalculator(interpolator, transform, rangeParameters, alphaRange);
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

internal class NoTransformation : ITransformation
{
    public float Transform(float x, float min, float max) => x;
}

internal class OutTransformation : ITransformation
{
    public float Transform(float x, float min, float max) => x - min;
}

#endregion
