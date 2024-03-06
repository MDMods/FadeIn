using HarmonyLib;
using Il2Cpp;

namespace FadeIn.Patches
{
    [HarmonyPatch(typeof(Boss))]
    internal static class BossPatch
    {
        [HarmonyPatch(nameof(Boss.InitBossObject))]
        [HarmonyPostfix]
        public static void BossAlpha(Boss __instance)
        {
            //__instance.spineActionController.m_SkeletonAnimation.skeleton.a = 0.35f;
        }
    }
}
