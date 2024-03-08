using FadeIn.Managers;
using HarmonyLib;
using Il2Cpp;
using Il2CppSpine;
using Il2CppSpine.Unity;
using UnityEngine;

namespace FadeIn.Patches
{
    [HarmonyPatch(typeof(BaseEnemyObjectController), nameof(BaseEnemyObjectController.EnableVisible))]
    internal static class BaseEnemyPatch
    {
        public static void Postfix(BaseEnemyObjectController __instance)
        {
            if (!SettingsManager.IsEnabled) return;

            Skeleton sk = __instance.m_SkeletonAnimation.skeleton;
            Transform parent = __instance.transform.parent;

            ModManager.ProcessEnemy(__instance, sk);

            if (parent.name.Equals("SceneObjectController")) return;
            for (int i = 0; i < parent.childCount; i++)
            {
                if (parent.GetChild(i).name.Equals(__instance.name)) continue;
                ModManager.ProcessEnemy(__instance, parent.GetChild(i).GetComponent<SkeletonAnimation>().skeleton);
            }
        }
    }

    [HarmonyPatch(typeof(LongPressController), nameof(LongPressController.SetVisible))]
    internal static class LongPressEnemyPatch
    {
        public static void Postfix(LongPressController __instance, bool enable)
        {
            if (!SettingsManager.IsEnabled) return;
            if (!enable) return;

            SpineActionController sac = __instance.m_Sac;
            ModManager.AddCallBackPress(__instance.gameObject, sac.m_StartStar, sac.m_EndStar, sac.m_Mtrl);
        }
    }
}
