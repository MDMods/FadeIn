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

using static ModManager;

[HarmonyPatch(typeof(PnlPreparation), nameof(PnlPreparation.Awake))]
internal static class TogglePatch
{
    internal static void Postfix(PnlPreparation __instance)
    {
        if (FadeToggle) return;

        FadeToggle = Object.Instantiate(
            GameObject.Find("Forward").transform.Find("PnlVolume").Find("LogoSetting").Find("Toggles").Find("TglOn")
                .gameObject,
            __instance.startButton.transform
        );
        FadeToggle.name = "FadeToggle";

        var toggleComp = FadeToggle.GetComponent<Toggle>();
        toggleComp.onValueChanged.AddListener((UnityAction<bool>)
            ((bool val) => { SettingsManager.IsEnabled = val; })
        );
        toggleComp.group = null;
        toggleComp.SetIsOnWithoutNotify(SettingsManager.IsEnabled);

        var txt = FadeToggle.transform.Find("Txt").GetComponent<Text>();
        txt.text = "Fade In";
        txt.fontSize = 40;
        txt.color = new Color(1, 1, 1, 76 / 255f);

        var rect = txt.transform.Cast<RectTransform>();
        var vect = rect.offsetMax;
        rect.offsetMax = new Vector2(txt.text.Length * 25, vect.y);

        var checkBox = FadeToggle.transform.Find("Background").GetComponent<Image>();
        checkBox.color = new Color(60 / 255f, 40 / 255f, 111 / 255f);

        var checkMark = FadeToggle.transform.Find("Background").GetChild(0).GetComponent<Image>();
        checkMark.color = new Color(103 / 255f, 93 / 255f, 130 / 255f);

        FadeToggle.transform.position = new Vector3(-7.6f, -4.8f, 100f);

        txt.GetComponent<Localization>().Destroy();
        FadeToggle.GetComponent<OnToggle>().Destroy();
        FadeToggle.GetComponent<OnToggleOn>().Destroy();
        FadeToggle.GetComponent<OnActivate>().Destroy();
        FadeToggle.GetComponent<VariableBehaviour>().Destroy();
    }
}