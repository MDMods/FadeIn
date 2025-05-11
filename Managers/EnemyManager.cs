using System.Collections.Concurrent;
using FadeIn.Utilities;
using Il2Cpp;
using Il2CppPeroPeroGames.GlobalDefines;

namespace FadeIn.Managers;

internal static class EnemyManager
{
    private static readonly ConcurrentDictionary<int, IFader> ActiveEnemies = new();
    private static readonly ConcurrentDictionary<int, IFader> Enemies = new();
    private static readonly Logger logger = new(nameof(EnemyManager));

    internal static void ActivateEnemy(BaseEnemyObjectController beoc)
    {
        var id = beoc.gameObject.GetInstanceID();
        if (Enemies.TryGetValue(id, out var fader))
        {
            fader.Start();
            ActiveEnemies.TryAdd(id, fader);
        }
    }

    internal static void ClearEnemies()
    {
        Enemies.Clear();
        ActiveEnemies.Clear();
    }

    internal static void InitEnemy(BaseEnemyObjectController beoc)
    {
        if (beoc is null || beoc.gameObject is null)
        {
            logger.Debug($"Enemy '{beoc?.name}' game object is null!");
            return;
        }

        if (beoc.m_SkeletonAnimation is null)
            return;

        var id = beoc.gameObject.GetInstanceID();

        IFader fader;
        logger.Debug("Node: " + beoc.m_NodeType);
        switch ((NoteType)beoc.m_NodeType)
        {
            case NoteType.None:
            case NoteType eventValue when eventValue <= NoteType.None || eventValue > NoteType.Mul:
            case NoteType.Press:
                return;

            case NoteType.Hp:
                fader = new HealthFader(beoc);
                break;
            case NoteType.Music:
                fader = new NoteFader(beoc);
                break;
            default:
                fader = new NormalEnemyFader(beoc);
                break;
        }

        Enemies.TryAdd(id, fader);
    }

    internal static void UpdateEnemies()
    {
        foreach (var (uid, fader) in ActiveEnemies)
        {
            fader.Update();
            if (fader.StopUpdating)
            {
                ActiveEnemies.TryRemove(uid, out _);
            }
        }
    }
}
