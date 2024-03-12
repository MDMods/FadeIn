using System.Collections;
using Il2Cpp;
using Il2CppPeroPeroGames.GlobalDefines;
using Il2CppSpine;
using Il2CppSpine.Unity;
using MelonLoader;
using UnityEngine;

namespace FadeIn.Managers;

using static ModManager;
using static SettingsManager;

internal static class NormalEnemyManager
{
    private static void UpdateAlphaValue(Skeleton sk, float coordinate, float lowerLimit, float initial,
        float lowerPosition)
    {
        if (coordinate > lowerLimit) return;
        sk.a = Mathf.Clamp(
            (coordinate - lowerPosition) / (initial - lowerPosition),
            0f, sk.a);
    }

    private static IEnumerator UpdateAlphaX(Skeleton sk, GameObject gameObject, Bone x, float initialX)
    {
        var lowerLimit = Mathf.Min(MinimalDistanceX, initialX);
        while (gameObject)
        {
            yield return WFS;
            if ((!SBC?.isInGame ?? true) || (SBC?.isPause ?? true)) continue;

            UpdateAlphaValue(sk, x.x, MinimalDistanceX, lowerLimit, DisappearPositionX);

            if (sk.a < 0.01f || x.x < DisappearPositionX) break;
        }

        sk.a = 0f;
    }

    private static IEnumerator UpdateAlphaR(Skeleton sk, GameObject gameObject, Bone y, float initialR)
    {
        while (gameObject)
        {
            yield return WFS;
            if ((!SBC?.isInGame ?? true) || (SBC?.isPause ?? true)) continue;

            UpdateAlphaValue(sk, y.rotation, MinimalDistanceR, initialR, DisappearPositionR);

            if (sk.a < 0.01f || y.rotation < DisappearPositionR) break;
        }

        sk.a = 0f;
    }

    private static IEnumerator UpdateAlphaNote(Skeleton sk, GameObject gameObject)
    {
        //Waiting for the proper position
        yield return WFS;

        while (gameObject)
        {
            yield return WFS;
            if ((!SBC?.isInGame ?? true) || (SBC?.isPause ?? true)) continue;

            var x = gameObject.transform.position.x;
            UpdateAlphaValue(sk, x, MinimalDistanceX, MinimalDistanceX, DisappearPositionX);

            if (sk.a < 0.01f || x < DisappearPositionX) break;
        }

        sk.a = 0f;
    }

    private static void AddCallBackEnemy(Skeleton sk, GameObject gameObject, Bone xPos, Bone yPos)
    {
        CoroutinesList.Add(MelonCoroutines.Start(yPos.rotation > 80f
            ? UpdateAlphaR(sk, gameObject, yPos, yPos.rotation)
            : UpdateAlphaX(sk, gameObject, xPos, xPos.x)));
    }

    private static void AddCallBackNote(Skeleton sk, GameObject gameObject)
    {
        CoroutinesList.Add(MelonCoroutines.Start(UpdateAlphaNote(sk, gameObject)));
    }

    internal static void ProcessEnemy(BaseEnemyObjectController beoc, Skeleton sk)
    {
        switch ((NoteType)beoc.m_NodeType)
        {
            case NoteType.Hp:
                AddCallBackNote(sk, beoc.gameObject);
                beoc.Cast<AirEnergyBottleController>().m_Fx.SetActive(false);
                return;
            case NoteType.Music:
                AddCallBackNote(sk, beoc.gameObject);
                beoc.Cast<AirMusicNodeController>().m_Fx.SetActive(false);
                return;

            // Dont do anything with the holds
            case NoteType.Press: return;
        }

        var xPos = beoc.m_Sac.bones["X"];
        var yPos = beoc.m_Sac.bones["Y"];
        AddCallBackEnemy(sk, beoc.gameObject, xPos, yPos);

        //Hearts on notes
        var hpOnNote = beoc.m_Blood;
        if (!hpOnNote) return;

        AddCallBackEnemy(hpOnNote.GetComponent<SkeletonAnimation>().skeleton, beoc.gameObject, xPos, yPos);
        var heartFx = hpOnNote.transform.Find("fx");

        if (!heartFx) return;
        heartFx.GetComponent<ParticleSystem>().Stop();
    }
}