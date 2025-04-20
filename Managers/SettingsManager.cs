using FadeIn.Properties;
using MelonLoader;
using MelonLoader.Preferences;
using MelonLoader.Utils;
using UnityEngine;

namespace FadeIn.Managers;

internal interface IInterpolator
{
    float Interpolate(float x, float min, float max);
}

internal interface ITransformation
{
    public float Transform(float x, float min, float max);
}

#region classes

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

internal class Fader
{
    private const float AlphaLowerLimit = 0.005f;
    private const float AlphaUpperLimit = 0.995f;

    #region fields
    internal static readonly IInterpolator Exponential = new ExponentialInterpolator();
    internal static readonly ITransformation InTransform = new InTransformation();
    internal static readonly IInterpolator Linear = new LinearInterpolator();
    internal static readonly ITransformation OutTransform = new OutTransformation();
    private readonly ValueRange AlphaRange;
    private readonly IInterpolator Interpolator;
    private readonly ValueRange PositionRange;
    private readonly ITransformation Transform;
    #endregion

    internal Fader(
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

internal class InTransformation : ITransformation
{
    public float Transform(float x, float min, float max) => max - x;
}

internal class LinearInterpolator : IInterpolator
{
    public float Interpolate(float x, float min, float max) => x / (max - min);
}

internal class OutTransformation : ITransformation
{
    public float Transform(float x, float min, float max) => x - min;
}

internal static class SettingsManager
{
    internal static event FileSystemEventHandler WatcherEvent
    {
        add => Watcher.Changed += value;
        remove => Watcher.Changed -= value;
    }

    internal const string FileName = $"{MelonBuildInfo.ModName}.cfg";
    internal const string Path = "UserData/" + FileName;

    #region properties
    internal static bool Debug => _debug.Value;
    internal static Decay FadeDecay => _fadeDecay.Value;
    internal static Difficulty FadeDifficulty => _difficulty.Value;
    internal static Mode FadeMode => _fadeMode.Value;
    internal static FadeParameters InSettings { get; private set; }

    internal static bool IsEnabled
    {
        get => _isEnabled.Value;
        set => _isEnabled.Value = value;
    }

    internal static bool IsGameScene { get; private set; } = false;
    internal static bool NeedReload { get; private set; } = false;
    internal static FadeParameters OutSettings { get; private set; }
    #endregion

    #region fields
    private static readonly MelonPreferences_Category _category;
    private static readonly MelonPreferences_Entry<bool> _debug;
    private static readonly EnumEntry<Difficulty> _difficulty;
    private static readonly EnumEntry<Decay> _fadeDecay;
    private static readonly EnumEntry<Mode> _fadeMode;
    private static readonly MelonPreferences_Entry<bool> _isEnabled;
    private static readonly FadeParameters InEasy = new(5.8f, 0f, 70f, 8f);
    private static readonly FadeParameters InHard = new(2f, -0.8f, 70f, 35f);
    private static readonly FadeParameters InMedium = new(3.9f, -0.4f, 70f, 20f);
    private static readonly FadeParameters InVeryHard = new(1f, -1.8f, 70f, 35f);
    private static readonly Utilities.Logger logger = new(nameof(SettingsManager));
    private static readonly FadeParameters OutEasy = new(5.8f, -1.8f, 70f, 8f);
    private static readonly FadeParameters OutHard = new(5.8f, 0f, 70f, 35f);
    private static readonly FadeParameters OutMedium = new(5.8f, -0.9f, 70f, 20f);
    private static readonly FileSystemWatcher Watcher = new(MelonEnvironment.UserDataDirectory);
    #endregion

    static SettingsManager()
    {
        _category = MelonPreferences.CreateCategory(MelonBuildInfo.ModName);
        _category.SetFilePath(Path, false, false);

        _isEnabled = _category.CreateEntry(
            "Enabled",
            true,
            description: "Enable or disable the mod!"
        );

        _difficulty = new EnumEntry<Difficulty>(
            _category,
            "Difficulty",
            Difficulty.Medium,
            "Difficulty presets!"
        );
        _fadeMode = new EnumEntry<Mode>(_category, "FadeMode", Mode.FadeOut, "Notes fade mode!");
        _fadeDecay = new EnumEntry<Decay>(
            _category,
            "FadeDecay",
            Decay.Linear,
            "Function used to calculate alpha value."
        );

        _debug = _category.CreateEntry("Debug", false, description: "Show debug logs!");

        InSettings = OutMedium;
        OutSettings = OutMedium;
    }

    #region methods

    internal static Fader GetFader()
    {
        var interpolator = FadeDecay switch
        {
            Decay.Exponential => Fader.Exponential,
            _ => Fader.Linear
        };

        ITransformation transform;
        FadeParameters parameters;
        ValueRange alphaRange;
        switch (FadeMode)
        {
            case Mode.FadeIn:
                transform = Fader.InTransform;
                parameters = InSettings;
                alphaRange = new ValueRange(0, 1);
                break;
            default:
                transform = Fader.OutTransform;
                parameters = OutSettings;
                alphaRange = new ValueRange(1, 0);
                break;
        }

        var rangeParameters = new ValueRange(parameters.LeftX, parameters.RightX);

        return new Fader(interpolator, transform, rangeParameters, alphaRange);
    }

    internal static void Init()
    {
        WatcherConfig();

        Load();
    }

    internal static void Load()
    {
        _category.LoadFromFile(false);

        InSettings = FadeDifficulty switch
        {
            Difficulty.Easy => InEasy,
            Difficulty.Medium => InMedium,
            Difficulty.Hard => InHard,
            Difficulty.VeryHard => InVeryHard,
            _ => InMedium
        };

        OutSettings = FadeDifficulty switch
        {
            Difficulty.Easy => OutEasy,
            Difficulty.Medium => OutMedium,
            Difficulty.Hard => OutHard,
            _ => OutMedium
        };

        if (Debug)
        {
            _category.Entries.ForEach(
                (entry) => logger.Debug($"{entry.DisplayName}: {entry.GetValueAsString()}")
            );
        }
    }

    internal static void Reload(bool sceneChanged = false)
    {
        // Reload only if not on game scene
        // If on game scene, queue reload
        // Reload on scene change if not on game scene and reload is queued
        if (IsGameScene)
        {
            NeedReload |= !sceneChanged;
            return;
        }

        if (sceneChanged && !NeedReload)
        {
            return;
        }

        Load();
        NeedReload = false;
    }

    internal static void SceneChanged(string sceneName)
    {
        IsGameScene = sceneName?.Equals("GameMain") ?? false;

        Reload(true);
    }

    internal static void WatcherStart()
    {
        // It is highly recommended to only enable events on melon late start...
        Watcher.EnableRaisingEvents = true;
    }

    internal static void WatcherStop()
    {
        Watcher.EnableRaisingEvents = false;
    }

    private static void WatcherConfig()
    {
        Watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size;

        Watcher.Filter = FileName;

        WatcherEvent += (_, _) =>
        {
            Reload(false);
        };
    }

    #endregion

    internal class EnumEntry<T>
        where T : struct, Enum, IConvertible
    {
        internal T Value => _entry.Value;

        private readonly T _defaultEnumValue;
        private readonly MelonPreferences_Entry<T> _entry;

        internal EnumEntry(
            MelonPreferences_Category category,
            string name,
            T defaultValue,
            string description
        )
        {
            _defaultEnumValue = defaultValue;
            var desc = description + "\n" + string.Join('\n', Enum.GetNames<T>());
            _entry = category.CreateEntry(
                name,
                _defaultEnumValue,
                validator: new EnumEntryValidator(defaultValue),
                description: desc
            );
        }

        internal void Subscribe(LemonAction<T, T> action)
        {
            _entry.OnEntryValueChanged.UnsubscribeAll();
            _entry.OnEntryValueChanged.Subscribe(action);
        }

        private sealed class EnumEntryValidator : ValueValidator
        {
            public readonly T DefaultValue;

            public EnumEntryValidator(T defaultValue)
            {
                DefaultValue = defaultValue;
            }

            public override object EnsureValid(object value)
            {
                return Enum.TryParse(value.ToString()?.Trim() ?? "", true, out T result)
                    ? result
                    : DefaultValue;
            }

            public override bool IsValid(object value) => true;
        }
    }

    internal enum Decay
    {
        Linear,
        Exponential
    }

    internal enum Difficulty
    {
        Easy,
        Medium,
        Hard,
        VeryHard
    }

    internal enum Mode
    {
        FadeIn,
        FadeOut,
        // Random
    }
}

#endregion

internal record FadeParameters(float RightX, float LeftX, float RightR, float LeftR);

internal record ValueRange(float Left, float Right);
