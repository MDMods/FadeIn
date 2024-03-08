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
            get { return _isEnabled.Value; }
            set { _isEnabled.Value = value; }
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
        private static float _dissapearPositionX;
        private static float _dissapearPositionR;
        private static float _minimalDistanceX;
        private static float _minimalDistanceR;

        public static float DissapearPositionX => _dissapearPositionX;
        public static float DissapearPositionR => _dissapearPositionR;
        public static float MinimalDistanceX => _minimalDistanceX;
        public static float MinimalDistanceR => _minimalDistanceR;

        internal static void InitValues()
        {
            switch (Difficulty)
            {
                case Difficulties.Easy:
                    _dissapearPositionX = -1.8f;
                    _dissapearPositionR = 8f;
                    break;

                case Difficulties.Medium:
                    _dissapearPositionX = -0.9f;
                    _dissapearPositionR = 20f;
                    break;

                case Difficulties.Hard:
                    _dissapearPositionX = 0f;
                    _dissapearPositionR = 35f;
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
