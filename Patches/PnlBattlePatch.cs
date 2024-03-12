using FadeIn.Managers;
using HarmonyLib;
using Il2CppAssets.Scripts.PeroTools.Commons;
using Il2CppAssets.Scripts.UI.Panels;
using Il2CppFormulaBase;

namespace FadeIn.Patches;

[HarmonyPatch(typeof(PnlBattle), nameof(PnlBattle.Awake))]
internal static class PnlBattlePatch
{
    internal static void Postfix()
    {
        ModManager.SBC = Singleton<StageBattleComponent>.instance;
    }
}