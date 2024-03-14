using Il2CppFormulaBase;
using MelonLoader;
using UnityEngine;

namespace FadeIn.Managers;

internal static class ModManager
{
    internal static readonly WaitForEndOfFrame CoroutineWait = new();
    internal static StageBattleComponent StageBattleComponent { get; set; } = null;
    
    internal static bool IsPause =>
        (!StageBattleComponent?.isInGame ?? true)
        || (StageBattleComponent?.isPause ?? true);

    // Coroutines
    internal static readonly List<object> CoroutinesList = new();
    internal static void ClearCoroutines()
    {
        foreach (var coroutine in CoroutinesList) MelonCoroutines.Stop(coroutine);
        CoroutinesList.Clear();
    }
}