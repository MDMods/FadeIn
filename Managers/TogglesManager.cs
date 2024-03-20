using FadeIn.Models;
using MuseDashMirror;
using MuseDashMirror.Models;
using MuseDashMirror.UIComponents;
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

    
    internal static void CreateEnableToggle(Transform parent)
    {
        if (FadeToggle) return;

        ToggleParameters toggleParameters = new("FadeToggle",
            "Fade In",
            IsEnabled,
            val => IsEnabled = val)
        {
            CheckMarkColor = new Color(103 / 255f, 93 / 255f, 130 / 255f)
        };

        var position = new Vector3(-7.6f, -4.8f, 100f);
        
        FadeToggle = CreateToggle(parent, toggleParameters, position);
        FadeToggle.transform.position = position;
    }
    

    internal static void Init()
    {
        EzController = new ToggleController("EzToggle",
            "FadeIn <color=#00800096>Easy</color>",
            Difficulties.Easy);

        MdController = new ToggleController("MdToggle",
            "FadeIn <color=#AAAA0096>Med</color>",
            Difficulties.Medium);

        HrController = new ToggleController("HrToggle",
            "FadeIn <color=#AA000096>Hard</color>",
            Difficulties.Hard);
        
        PatchEvents.PnlMenuPatch += (_,_) => ToggleController.InitToggles();
    }
}