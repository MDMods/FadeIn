using FadeIn.Managers;
using MelonLoader;

namespace FadeIn;

public class Main : MelonMod
{
    public override void OnInitializeMelon()
    {
        base.OnInitializeMelon();
        SettingsManager.Load();
        LoggerInstance.Msg("FadeIn has loaded correctly!");
    }

    public override void OnSceneWasLoaded(int buildIndex, string sceneName)
    {
        base.OnSceneWasLoaded(buildIndex, sceneName);
        if (sceneName.Equals("GameMain")) return;
        ModManager.ClearCoroutines();
        PressEnemyManager.ClearPress();
    }
}