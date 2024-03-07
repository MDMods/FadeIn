using FadeIn.Managers;
using HarmonyLib;
using Il2Cpp;
using Il2CppSpine;
using Il2CppSpine.Unity;
using UnityEngine;

namespace FadeIn.Patches
{
    [HarmonyPatch(typeof(BaseEnemyObjectController))]
    internal static class EnemyPatch
    {
        [HarmonyPatch(nameof(BaseEnemyObjectController.EnableVisible))]
        [HarmonyPostfix]
        public static void PostfixEnableVisible(BaseEnemyObjectController __instance)
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


    /*
   [HarmonyPatch(typeof(LongPressController))]
   internal static class GetVisibleEnemiesPssddfsfsfsatch
   {
       // Messing aroudn with holds
       [HarmonyPostfix]
       [HarmonyPatch(nameof(LongPressController.SetVisible))]
       public static void PostfixBEOC(LongPressController __instance, bool enable)
       {
           if (!enable) return;
           //static bool x = false;

           var x = __instance.transform.GetChild(0).gameObject;
           Melon<Main>.Logger.Msg($"~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~1");
           foreach ( var c in x.GetComponentsInChildren<Component>()) { 
               Melon<Main>.Logger.Msg( c.ToString() );
           }
           __instance.m_Sac.SetAlpha(0.1f);
           Melon<Main>.Logger.Msg($"~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~2");

       }
   }
   */
}
