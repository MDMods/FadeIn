using FadeIn.Models;

namespace FadeIn.Managers;

internal static class TogglesManager
{
    private static ToggleController EzController { get; set; }
    private static ToggleController MdController { get; set; }
    private static ToggleController HrController { get; set; }

    internal static void Init()
    {
        EzController = new ToggleController("EzToggle", "FadeIn <color=#00800096>Easy</color>", Difficulties.Easy);
        MdController = new ToggleController("MdToggle", "FadeIn <color=#AAAA0096>Med</color>", Difficulties.Medium);
        HrController = new ToggleController("HrToggle", "FadeIn <color=#AA000096>Hard</color>", Difficulties.Hard);
    }
}