using FadeIn.Managers;
using UnityEngine;
using UnityEngine.UI;
using static MuseDashMirror.UIComponents.ToggleUtils;

namespace FadeIn.Models;

internal class ToggleController
{
    private bool _toggleValue;

    internal ToggleController(string name, string text, Difficulties difficulty)
    {
        Name = name;
        DisplayText = text;
        ToggleValue = difficulty == SettingsManager.Difficulty;
        Difficulty = difficulty;
        TogglesList.Add(this);
    }

    private static List<ToggleController> TogglesList { get; } = new();

    private static GameObject DiffTogglesGroupGameObject { get; set; }
    private static ToggleGroup DiffTogglesGroup { get; set; }

    private string Name { get; }
    private string DisplayText { get; }
    private Difficulties Difficulty { get; }

    internal GameObject ToggleObject { get; private set; }

    private bool ToggleValue
    {
        get => _toggleValue;
        set
        {
            _toggleValue = value;
            if (!_toggleValue) return;
            SettingsManager.Difficulty = Difficulty;
        }
    }

    internal static void InitToggles()
    {
        if (!DiffTogglesGroupGameObject)
        {
            DiffTogglesGroupGameObject = new GameObject("FadeInToggleGroup");
            DiffTogglesGroup = DiffTogglesGroupGameObject.AddComponent<ToggleGroup>();
        }

        DiffTogglesGroup.allowSwitchOff = true;
        foreach (var toggle in TogglesList)
        {
            toggle.RegisterToggle();
            if (!toggle.ToggleObject.TryGetComponent(out Toggle toggleComponent)) continue;
            toggleComponent.group = DiffTogglesGroup;
        }
        DiffTogglesGroup.allowSwitchOff = false;
    }

    private void RegisterToggle()
    {
        ToggleObject =
            CreatePnlMenuToggle(Name, DisplayText, ToggleValue, new Action<bool>(val => ToggleValue = val));
    }
}