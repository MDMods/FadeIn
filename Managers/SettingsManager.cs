using MelonLoader;

namespace FadeIn.Managers;

internal enum Difficulties
{
    Easy,
    Medium,
    Hard
}

internal static class SettingsManager
{
    private const string SettingsPath = "UserData/FadeIn.cfg";

    internal const float AlphaLowerLimit = 0.005f;

    private static MelonPreferences_Entry<bool> _isEnabled;
    internal static bool IsEnabled
    {
        get => _isEnabled.Value;
        set => _isEnabled.Value = value;
    }
    
    private static MelonPreferences_Entry<string> _difficulty;
    private static Difficulties _currentDifficulty;
    internal static Difficulties Difficulty
    {
        get => _currentDifficulty;

        set
        {
            _currentDifficulty = value;
            switch (_currentDifficulty)
            {
                case Difficulties.Easy:
                    _difficulty.Value = "Easy";
                    DisappearPositionX = -1.8f;
                    DisappearPositionR = 8f;
                    break;

                case Difficulties.Hard:
                    _difficulty.Value = "Hard";
                    DisappearPositionX = 0f;
                    DisappearPositionR = 35f;
                    break;

                default:
                    _difficulty.Value = "Medium";
                    DisappearPositionX = -0.9f;
                    DisappearPositionR = 20f;
                    break;
            }
        }
    }

    internal static float DisappearPositionX { get; private set; }
    internal static float DisappearPositionR { get; private set; }
    internal static float MinimalDistanceX { get; private set; }
    internal static float MinimalDistanceR { get; private set; }

    private static void InitValues()
    {
        Difficulty = _difficulty.Value switch
        {
            "Easy" => Difficulties.Easy,
            "Hard" => Difficulties.Hard,
            _ => Difficulties.Medium
        };

        MinimalDistanceX = 5.8f;
        MinimalDistanceR = 70f;
    }

    internal static void Load()
    {
        var settings = MelonPreferences.CreateCategory("FadeIn");
        settings.SetFilePath(SettingsPath, true, false);

        _isEnabled = settings.CreateEntry(nameof(IsEnabled), false);
        _difficulty = settings.CreateEntry(nameof(Difficulty), "Medium", description: "Options:\nEasy\nMedium\nHard");

        InitValues();
    }
}