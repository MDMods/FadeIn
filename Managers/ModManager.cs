using Il2CppFormulaBase;
using MelonLoader;
using UnityEngine;

namespace FadeIn.Managers;

internal static partial class ModManager
{
    internal static GameObject FadeToggle { get; set; }
    public static StageBattleComponent SBC { get; set; } = null;

    // Coroutines
    internal static readonly List<object> CoroutinesList = new();

    public static void ClearCoroutines()
    {
        foreach (var coroutine in CoroutinesList) MelonCoroutines.Stop(coroutine);
        CoroutinesList.Clear();
    }

    // Holds 
    internal static readonly HashSet<string> PressList = new();

    public static void ClearPress()
    {
        PressList.Clear();
    }

    //private static readonly WaitForSeconds WFS = new WaitForSeconds(0.05f);
    internal static readonly WaitForSeconds WFS = null;
}