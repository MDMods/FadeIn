using MuseDashMirror;
using MuseDashMirror.EventArguments;
using UnityEngine;
using UnityEngine.UI;
using static MuseDashMirror.UIComponents.ToggleUtils;

namespace FadeIn.Managers;

internal static class ToggleManagers
{
    internal static ToggleController EzController { get; set; }
    internal static ToggleController MdController { get; set; }
    internal static ToggleController HrController { get; set; }

    // Difficulty Toggles
    private static GameObject DiffGroup { get; set; }

    internal class ToggleController
    {
        private static Difficulties _toggleGroupValue;

        private static Difficulties ToggleGroupValue
        {
            set
            {
                _toggleGroupValue = value;
                SettingsManager.Difficulty = _toggleGroupValue;
            }
        }

        internal ToggleController(string name, string text, Difficulties difficulty)
        {
            Name = name;
            DisplayText = text;
            ToggleValue = difficulty == SettingsManager.Difficulty;
            Difficulty = difficulty;
            PatchEvents.PnlMenuPatch += RegisterToggle;
        }
        
        private GameObject _toggleObject;
        
        private Difficulties Difficulty { get; }

        private string Name { get; }
        private string DisplayText { get; }

        private bool _toggleValue;

        internal bool ToggleValue
        {
            get => _toggleValue;
            set
            {
                _toggleValue = value;
                if (!_toggleValue) return;
                ToggleGroupValue = Difficulty;
            }
        }

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

        private void RegisterToggle(object sender, PnlMenuEventArgs args)
        {
            ToggleObject =
                CreatePnlMenuToggle(Name, DisplayText, ToggleValue, new Action<bool>(val => ToggleValue = val));
        }
    }
}