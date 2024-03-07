using FadeIn.Managers;
using HarmonyLib;
using Il2Cpp;
using Il2CppAssets.Scripts.PeroTools.GeneralLocalization;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static FadeIn.Managers.ModManager;
using Nice = Il2CppAssets.Scripts.PeroTools.Nice;

namespace FadeIn.Patches
{
    [HarmonyPatch(typeof(PnlPreparation), nameof(PnlPreparation.Awake))]
    internal static class TogglePatch
    {
        public static void Postfix(PnlPreparation __instance)
        {
            if (FadeToggle) return;

            FadeToggle = UnityEngine.Object.Instantiate(
                GameObject.Find("Forward").transform.Find("PnlVolume").Find("LogoSetting").Find("Toggles").Find("TglOn").gameObject,
                __instance.startButton.transform
                );
            FadeToggle.name = "FadeToggle";

            Toggle toggleComp = FadeToggle.GetComponent<Toggle>();
            toggleComp.onValueChanged.AddListener((UnityAction<bool>)
                ((bool val) => { SettingsManager.IsEnabled = val; })
                );
            toggleComp.group = null;
            toggleComp.SetIsOnWithoutNotify(SettingsManager.IsEnabled);

            Text txt = FadeToggle.transform.Find("Txt").GetComponent<Text>();
            txt.text = "Fade In";
            txt.fontSize = 40;
            txt.color = new Color(1, 1, 1, 76 / 255f);

            RectTransform rect = txt.transform.Cast<RectTransform>();
            Vector2 vect = rect.offsetMax;
            rect.offsetMax = new Vector2(txt.text.Length * 25, vect.y);

            Image checkBox = FadeToggle.transform.Find("Background").GetComponent<Image>();
            checkBox.color = new(60 / 255f, 40 / 255f, 111 / 255f);

            Image checkMark = FadeToggle.transform.Find("Background").GetChild(0).GetComponent<Image>();
            checkMark.color = new(103 / 255f, 93 / 255f, 130 / 255f);

            FadeToggle.transform.position = new Vector3(-7.6f, -4.8f, 100f);

            txt.GetComponent<Localization>().Destroy();
            FadeToggle.GetComponent<Nice.Events.OnToggle>().Destroy();
            FadeToggle.GetComponent<Nice.Events.OnToggleOn>().Destroy();
            FadeToggle.GetComponent<Nice.Events.OnActivate>().Destroy();
            FadeToggle.GetComponent<Nice.Variables.VariableBehaviour>().Destroy();
        }
    }
}
