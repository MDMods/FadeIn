using HarmonyLib;
using Il2Cpp;
using Il2CppAssets.Scripts.PeroTools.Nice.Events;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static FadeIn.ModManager;

namespace FadeIn.Patches
{
    [HarmonyPatch(typeof(PnlPreparation))]
    internal static class TogglePatch
    {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(PnlPreparation.Awake))]
        public static void Postfix(PnlPreparation __instance)
        {
            if (FadeToggle != null) return;

            FadeToggle = UnityEngine.Object.Instantiate(
                GameObject.Find("Forward").transform.Find("PnlVolume").Find("LogoSetting").Find("Toggles").Find("TglOn").gameObject,
                __instance.startButton.transform
                );
            FadeToggle.name = "FadeToggle";

            Text txt = FadeToggle.transform.Find("Txt").GetComponent<Text>();
            Image checkBox = FadeToggle.transform.Find("Background").GetComponent<Image>();
            Image checkMark = FadeToggle.transform.Find("Background").GetChild(0).GetComponent<Image>();

            FadeToggle.transform.position = new Vector3(-7.6f, -4.8f, 100f);
            FadeToggle.GetComponent<OnToggle>().enabled = false;
            FadeToggle.GetComponent<OnToggleOn>().enabled = false;
            FadeToggle.GetComponent<OnActivate>().enabled = false;

            Toggle toggleComp = FadeToggle.GetComponent<Toggle>();
            toggleComp.onValueChanged.AddListener((UnityAction<bool>)
                ((bool val) => { Enabled = val; })
                );
            toggleComp.group = null;
            toggleComp.SetIsOnWithoutNotify(Enabled);

            txt.text = "Fade In";
            txt.fontSize = 40;
            txt.color = new Color(1, 1, 1, 76 / 255f);
            RectTransform rect = txt.transform.Cast<RectTransform>();
            Vector2 vect = rect.offsetMax;
            rect.offsetMax = new Vector2(txt.text.Length * 25, vect.y);

            checkBox.color = new(60 / 255f, 40 / 255f, 111 / 255f);
            checkMark.color = new(103 / 255f, 93 / 255f, 130 / 255f);
        }
    }
}
