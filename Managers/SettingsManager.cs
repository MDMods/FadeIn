using FadeIn.Properties;
using MelonLoader;
using MelonLoader.Preferences;
using MelonLoader.Utils;

namespace FadeIn.Managers;

internal static class SettingsManager
{
    internal static event FileSystemEventHandler WatcherEvent
    {
        add => Watcher.Changed += value;
        remove => Watcher.Changed -= value;
    }

    internal const float AlphaLowerLimit = 0.005f;
    internal const string FileName = $"{MelonBuildInfo.ModName}.cfg";
    internal const float MinimalDistanceR = 70f;
    internal const float MinimalDistanceX = 5.8f;
    internal const string Path = "UserData/" + FileName;

    #region properties
    internal static float DisappearR => difficultySettings.DisappearR;
    internal static float DisappearX => difficultySettings.DisappearX;
    internal static Mode FadeMode => _fadeMode.Value;

    internal static bool IsEnabled
    {
        get => _isEnabled.Value;
        set => _isEnabled.Value = value;
    }

    internal static bool IsGameScene { get; private set; } = false;
    internal static bool NeedReload { get; private set; } = false;
    #endregion

    #region fields
    private static MelonPreferences_Category _category;
    private static EnumEntry<Difficulty> _difficulty;
    private static EnumEntry<Mode> _fadeMode;
    private static MelonPreferences_Entry<bool> _isEnabled;
    private static DifficultySettings difficultySettings;
    private static readonly DifficultySettings Easy = new(-1.8f, 8f);
    private static readonly DifficultySettings Hard = new(0f, 35f);
    private static readonly DifficultySettings Medium = new(-0.9f, 20f);
    private static readonly FileSystemWatcher Watcher = new(MelonEnvironment.UserDataDirectory);
    #endregion

    static SettingsManager() { }

    #region methods

    internal static void Init()
    {
        WatcherConfig();

        _category = MelonPreferences.CreateCategory(MelonBuildInfo.ModName);
        _category.SetFilePath(Path, false, false);

        _isEnabled = _category.CreateEntry(
            "Enabled",
            true,
            description: "Enable or disable the mod!"
        );

        _difficulty = new EnumEntry<Difficulty>(_category, "Difficulty", Difficulty.Medium);
        _fadeMode = new EnumEntry<Mode>(_category, "FadeMode", Mode.FadeOut);

        Load();
    }

    internal static void Load()
    {
        _category.LoadFromFile(false);

        difficultySettings = _difficulty.Value switch
        {
            Difficulty.Easy => Easy,
            Difficulty.Medium => Medium,
            Difficulty.Hard => Hard,
            _ => Medium,
        };

        Melon<Main>.Logger.Msg("Enabled: " + IsEnabled);
        Melon<Main>.Logger.Msg("Difficulty: " + _difficulty.Value);
    }

    internal static void Reload(bool sceneChanged = false)
    {
        Melon<Main>.Logger.Msg("SceneChanged: " + sceneChanged);
        Melon<Main>.Logger.Msg("IsGameScene: " + IsGameScene);
        Melon<Main>.Logger.Msg("NeedReload: " + NeedReload);
        if (IsGameScene)
        {
            if (!sceneChanged)
            {
                NeedReload = true;
            }
            return;
        }

        if (!sceneChanged)
        {
            Load();
            NeedReload = false;
            return;
        }

        if (!NeedReload)
        {
            return;
        }

        Load();
        NeedReload = false;
        return;
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

        internal EnumEntry(MelonPreferences_Category category, string name, T defaultValue)
        {
            _defaultEnumValue = defaultValue;
            _entry = category.CreateEntry(
                name,
                _defaultEnumValue,
                validator: new EnumEntryValidator(defaultValue)
            );

            // Todo: add description (show options)
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
                return Enum.TryParse(value.ToString().Trim(), true, out T result)
                    ? result
                    : DefaultValue;
            }

            public override bool IsValid(object value) => true;
        }
    }

    internal enum Difficulty
    {
        Easy,
        Medium,
        Hard
    }

    internal enum Mode
    {
        FadeIn,
        FadeOut,
        Random
    }

    private readonly struct DifficultySettings
    {
        public float DisappearR { get; init; }
        public float DisappearX { get; init; }

        public DifficultySettings(float disappearX, float disappearR)
        {
            DisappearX = disappearX;
            DisappearR = disappearR;
        }
    }
}
