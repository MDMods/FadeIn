using FadeIn.Managers;
using HarmonyLib;
using Il2Cpp;

namespace FadeIn.Patches;

[HarmonyPatch(typeof(LongPressController), nameof(LongPressController.SetVisible))]
internal static class LongPressEnemyPatch
{
    internal static void Postfix(LongPressController __instance, bool enable)
    {
        if (!SettingsManager.IsEnabled) return;
        if (!enable) return;

        var sac = __instance.m_Sac;
        PressEnemyManager.AddCallBackPress(__instance.gameObject, sac.m_StartStar, sac.m_EndStar, sac.m_Mtrl);
    }
}