using FadeIn.Managers;
using MelonLoader;

namespace FadeIn;

public class Main : MelonMod
{
    public override void OnDeinitializeMelon()
    {
        base.OnDeinitializeMelon();
        SettingsManager.WatcherStop();
    }

    public override void OnInitializeMelon()
    {
        base.OnInitializeMelon();
        SettingsManager.Init();
        LoggerInstance.Msg("FadeIn has loaded correctly!");
    }

    public override void OnLateInitializeMelon()
    {
        base.OnLateInitializeMelon();
        SettingsManager.WatcherStart();
    }

    public override void OnLateUpdate()
    {
        base.OnLateUpdate();
        if (ModManager.IsPause)
        {
            return;
        }
    }

    public override void OnSceneWasLoaded(int buildIndex, string sceneName)
    {
        base.OnSceneWasLoaded(buildIndex, sceneName);
        SettingsManager.SceneChanged(sceneName);
        if (SettingsManager.IsGameScene)
            return;
        ModManager.ClearCoroutines();
        // PressEnemyManager.ClearPress();
    }
}
