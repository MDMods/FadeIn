using FadeIn.Models;
using MuseDashMirror.Models;
using UnityEngine;
using static MuseDashMirror.UIComponents.ToggleUtils;

namespace FadeIn.Managers;

using static SettingsManager;

internal static class TogglesManager
{
    private static ToggleController EzController { get; set; }
    private static ToggleController MdController { get; set; }
    private static ToggleController HrController { get; set; }

    internal static GameObject FadeToggle { get; set; }

    /*
    internal static void CreateEnableToggle(Transform parent)
    {
        ToggleParameters toggleParameters = new("FadeToggle",
            new TextParameters("Fade In"),
            IsEnabled,
            val => IsEnabled = val);
        TransformParameters transformParameters = new(new Vector3(-7.6f, -4.8f, 100f));
        if (FadeToggle) return;

        //EnableToggle = CreateToggle(parent.gameObject, toggleParameters, transformParameters);
        FadeToggle = CreateToggle(toggleParameters, transformParameters);
    }
    */

    internal static void Init()
    {
        EzController = new ToggleController("EzToggle", "FadeIn <color=#00800096>Easy</color>", Difficulties.Easy);
        MdController = new ToggleController("MdToggle", "FadeIn <color=#AAAA0096>Med</color>", Difficulties.Medium);
        HrController = new ToggleController("HrToggle", "FadeIn <color=#AA000096>Hard</color>", Difficulties.Hard);
    }
}