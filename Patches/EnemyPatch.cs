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
            ModManager.ProcessEnemy(__instance, sk);

            Transform parent = __instance.transform.parent;
            if (parent.name.Equals("SceneObjectController")) return;
            for (int i = 0; i < parent.childCount; i++)
            {
                Transform child = parent.GetChild(i);
                if (child.name.Equals(__instance.name)) continue;
                ModManager.ProcessEnemy(__instance, child.GetComponent<SkeletonAnimation>().skeleton);
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
