using MelonLoader;

namespace FadeIn.Managers
{
    public enum Difficulties
    {
        Easy,
        Medium,
        Hard,
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
                    _ => Difficulties.Medium,
                };
            }
        }
        private static float _disappearPositionX;
        private static float _disappearPositionR;
        private static float _minimalDistanceX;
        private static float _minimalDistanceR;

        public static float DisappearPositionX => _disappearPositionX;
        public static float DisappearPositionR => _disappearPositionR;
        public static float MinimalDistanceX => _minimalDistanceX;
        public static float MinimalDistanceR => _minimalDistanceR;

        internal static void InitValues()
        {
            switch (Difficulty)
            {
                case Difficulties.Easy:
                    _disappearPositionX = -1.8f;
                    _disappearPositionR = 8f;
                    break;

                case Difficulties.Medium:
                    _disappearPositionX = -0.9f;
                    _disappearPositionR = 20f;
                    break;

                case Difficulties.Hard:
                    _disappearPositionX = 0f;
                    _disappearPositionR = 35f;
                    break;
            }
            _minimalDistanceX = 5.8f;
            _minimalDistanceR = 70f;

        }

        public static void Load()
        {
            MelonPreferences_Category settings = MelonPreferences.CreateCategory("FadeIn");
            _isEnabled = settings.CreateEntry<bool>(nameof(IsEnabled), false);
            _difficulty = settings.CreateEntry<string>(nameof(Difficulty), "Medium", description: "Options:\nEasy\nMedium\nHard");

            InitValues();
        }
    }
}
