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
        public static float DissapearPositionX { get; set; }
        public static float DissapearPositionR { get; set; }
        public static float MinimalDistanceX { get; set; }
        public static float MinimalDistanceR { get; set; }

        internal static void InitValues()
        {
            switch (Difficulty)
            {
                case Difficulties.Easy:
                    DissapearPositionX = -1.8f;
                    DissapearPositionR = 8f;
                    break;

                case Difficulties.Medium:
                    DissapearPositionX = -0.9f;
                    DissapearPositionR = 20f;
                    break;

                case Difficulties.Hard:
                    DissapearPositionX = 0f;
                    DissapearPositionR = 35f;
                    break;
            }
            MinimalDistanceX = 5.8f;
            MinimalDistanceR = 70f;

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
