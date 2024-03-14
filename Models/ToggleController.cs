using FadeIn.Managers;
using MuseDashMirror;
using MuseDashMirror.EventArguments;
using UnityEngine;
using UnityEngine.UI;
using static MuseDashMirror.UIComponents.ToggleUtils;

namespace FadeIn.Models;

internal class ToggleController
{
    private GameObject _toggleObject;

    private bool _toggleValue;

    internal ToggleController(string name, string text, Difficulties difficulty)
    {
        Name = name;
        DisplayText = text;
        ToggleValue = difficulty == SettingsManager.Difficulty;
        Difficulty = difficulty;
        PatchEvents.PnlMenuPatch += RegisterToggle;
    }

    private static GameObject DiffGroup { get; set; }

    private string Name { get; }
    private string DisplayText { get; }
    private Difficulties Difficulty { get; }

    internal GameObject ToggleObject
    {
        get => _toggleObject;
        private set
        {
            _toggleObject = value;
            if (!DiffGroup)
            {
                DiffGroup = new GameObject("FadeInToggleGroup");
                DiffGroup.AddComponent<ToggleGroup>();
            }

            if (!_toggleObject.TryGetComponent(out Toggle toggle)) return;
            toggle.group = DiffGroup.GetComponent<ToggleGroup>();
        }
    }

    internal bool ToggleValue
    {
        get => _toggleValue;
        set
        {
            _toggleValue = value;
            if (!_toggleValue) return;
            SettingsManager.Difficulty = Difficulty;
        }
    }

    private void RegisterToggle(object sender, PnlMenuEventArgs args)
    {
        ToggleObject =
            CreatePnlMenuToggle(Name, DisplayText, ToggleValue, new Action<bool>(val => ToggleValue = val));
    }
}