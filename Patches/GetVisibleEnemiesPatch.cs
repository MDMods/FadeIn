using HarmonyLib;
using Il2Cpp;
using static FadeIn.ModManager;

namespace FadeIn.Patches
{
    [HarmonyPatch(typeof(BaseEnemyObjectController))]
    internal static class GetVisibleEnemiesPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(BaseEnemyObjectController.EnableVisible))]
        public static void PostfixBEOC(BaseEnemyObjectController __instance)
        {
            EnableVisiblePatch(__instance.gameObject);
        }
    }
}
