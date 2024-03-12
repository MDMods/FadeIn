using MelonLoader;

namespace FadeIn.Managers;

using static ToggleManagers;

public enum Difficulties
{
    Easy,
    Medium,
    Hard
}

internal class SettingsManager
{
    private const string SettingsPath = "UserData/FadeIn.cfg";
    
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
        set
        {
            switch (value)
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

                case Difficulties.Medium:
                default:
                    _difficulty.Value = "Medium";
                    DisappearPositionX = -0.9f;
                    DisappearPositionR = 20f;
                    break;
            }
        }
    }

    public static float DisappearPositionX { get; private set; }
    public static float DisappearPositionR { get; private set; }
    public static float MinimalDistanceX { get; private set; }
    public static float MinimalDistanceR { get; private set; }

    private static void InitValues()
    {
        Difficulty = Difficulty;
        
        MinimalDistanceX = 5.8f;
        MinimalDistanceR = 70f;
    }

    public static void Load()
    {
        var settings = MelonPreferences.CreateCategory("FadeIn");
        settings.SetFilePath(SettingsPath, true, false);
        
        _isEnabled = settings.CreateEntry(nameof(IsEnabled), false);
        _difficulty = settings.CreateEntry(nameof(Difficulty), "Medium", description: "Options:\nEasy\nMedium\nHard");
        
        InitValues();

        EzController = new ToggleController("EzToggle", "FadeIn EZ", Difficulties.Easy);
        MdController = new ToggleController("MdToggle", "FadeIn MD", Difficulties.Medium);
        HrController = new ToggleController("HrToggle", "FadeIn HR", Difficulties.Hard);
    }
}