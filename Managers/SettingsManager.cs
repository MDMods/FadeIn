using MelonLoader;

namespace FadeIn.Managers;

public enum Difficulties
{
    Easy,
    Medium,
    Hard
}

internal class SettingsManager
{
    private static MelonPreferences_Entry<bool> _isEnabled;

    internal static bool IsEnabled
    {
        get => _isEnabled.Value;
        set => _isEnabled.Value = value;
    }

    private static MelonPreferences_Entry<string> _difficulty;

    internal static Difficulties Difficulty
    {
        get
        {
            return _difficulty.Value switch
            {
                "Easy" => Difficulties.Easy,
                "Hard" => Difficulties.Hard,
                _ => Difficulties.Medium
            };
        }
    }

    public static float DisappearPositionX { get; private set; }
    public static float DisappearPositionR { get; private set; }
    public static float MinimalDistanceX { get; private set; }
    public static float MinimalDistanceR { get; private set; }

    private static void InitValues()
    {
        switch (Difficulty)
        {
            case Difficulties.Easy:
                DisappearPositionX = -1.8f;
                DisappearPositionR = 8f;
                break;

            case Difficulties.Hard:
                DisappearPositionX = 0f;
                DisappearPositionR = 35f;
                break;

            case Difficulties.Medium:
            default:
                DisappearPositionX = -0.9f;
                DisappearPositionR = 20f;
                break;
        }

        MinimalDistanceX = 5.8f;
        MinimalDistanceR = 70f;
    }

    public static void Load()
    {
        var settings = MelonPreferences.CreateCategory("FadeIn");
        _isEnabled = settings.CreateEntry(nameof(IsEnabled), false);
        _difficulty = settings.CreateEntry(nameof(Difficulty), "Medium", description: "Options:\nEasy\nMedium\nHard");

        InitValues();
    }
}