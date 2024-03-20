using FadeIn.Managers;
using HarmonyLib;
using Il2Cpp;
using Il2CppAssets.Scripts.PeroTools.GeneralLocalization;
using Il2CppAssets.Scripts.PeroTools.Nice.Events;
using Il2CppAssets.Scripts.PeroTools.Nice.Variables;
using MuseDashMirror.Extensions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Object = UnityEngine.Object;
using Text = UnityEngine.UI.Text;

namespace FadeIn.Patches;

using static TogglesManager;

[HarmonyPatch(typeof(PnlPreparation), nameof(PnlPreparation.Awake))]
internal static class TogglePatch
{
    internal static void Postfix(PnlPreparation __instance)
    {
        if (FadeToggle) return;
        CreateEnableToggle(__instance.startButton.transform);
    }
}