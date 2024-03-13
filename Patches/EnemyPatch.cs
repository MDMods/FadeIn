using FadeIn.Managers;
using HarmonyLib;
using Il2Cpp;
using Il2CppSpine.Unity;

namespace FadeIn.Patches;

[HarmonyPatch(typeof(BaseEnemyObjectController), nameof(BaseEnemyObjectController.EnableVisible))]
internal static class BaseEnemyPatch
{
    internal static void Postfix(BaseEnemyObjectController __instance)
    {
        if (!SettingsManager.IsEnabled) return;

        var sk = __instance.m_SkeletonAnimation.skeleton;
        NormalEnemyManager.ProcessEnemy(__instance, sk);

        var parent = __instance.transform.parent;
        if (parent.name.Equals("SceneObjectController")) return;

        for (var i = 0; i < parent.childCount; i++)
        {
            var child = parent.GetChild(i);
            if (child.name.Equals(__instance.name)) continue;

            NormalEnemyManager.ProcessEnemy(__instance, child.GetComponent<SkeletonAnimation>().skeleton);
        }
    }
}

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