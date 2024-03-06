using MelonLoader;

namespace FadeIn
{
    public class Main : MelonMod
    {

        public override void OnInitializeMelon()
        {
            base.OnInitializeMelon();
            LoggerInstance.Msg("FadeIn has loaded correctly!");
        }
    }
}
