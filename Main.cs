using MelonLoader;
using static FadeIn.ModManager;

namespace FadeIn
{
    public class Main : MelonMod
    {

        public override void OnInitializeMelon()
        {
            base.OnInitializeMelon();
            Enabled = false;
            LoggerInstance.Msg("FadeIn has loaded correctly!");
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            battleScene = sceneName == "GameMain";
            if (!battleScene)
            {
                OnSceneWasLoadedFunc();
            }
        }
        public override void OnUpdate()
        {
            if (!battleScene) return;
            OnUpdateFunc();
        }
    }
}
