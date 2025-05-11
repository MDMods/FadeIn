using FadeIn.Managers;
using FadeIn.Utilities;
using HarmonyLib;
using Il2Cpp;

namespace FadeIn.Patches;

[HarmonyPatch(typeof(BaseEnemyObjectController))]
internal static class BaseEnemyPatch
{
    // , nameof(BaseEnemyObjectController.EnableVisible)
    [HarmonyPatch(nameof(BaseEnemyObjectController.EnableVisible))]
    [HarmonyPostfix]
    internal static void DisablePostfix(BaseEnemyObjectController __instance)
    {
        if (!SettingsManager.IsEnabled)
            return;

        EnemyManager.ActivateEnemy(__instance);
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(BaseEnemyObjectController.Init))]
    internal static void Init(BaseEnemyObjectController __instance)
    {
        if (!SettingsManager.IsEnabled)
            return;

        EnemyManager.InitEnemy(__instance);
    }
}

[HarmonyPatch("Il2CppInterop.HarmonySupport.Il2CppDetourMethodPatcher", "ReportException")]
internal static class Il2CppDetourMethodPatcherPatch
{
    private static readonly Logger Logger = new(nameof(Il2CppDetourMethodPatcherPatch));

    private static bool Prefix(Exception ex)
    {
        Logger.Msg("During invoking native->managed trampoline: " + ex);
        return false;
    }
}
