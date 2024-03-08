using Il2Cpp;
using Il2CppPeroPeroGames.GlobalDefines;
using Il2CppSpine;
using Il2CppSpine.Unity;
using MelonLoader;
using System.Collections;
using UnityEngine;
using static FadeIn.Managers.SettingsManager;

namespace FadeIn.Managers
{
    internal static partial class ModManager
    {
        private static void UpdateAlphaValue(Skeleton sk, float coordinate, float LowerLimit, float initial, float LowerPosition)
        {
            if (coordinate > LowerLimit) return;
            sk.a = Mathf.Clamp(
                (coordinate - LowerPosition) / (initial - LowerPosition),
                0f, sk.a);
        }

        private static IEnumerator UpdateAlphaX(Skeleton sk, GameObject gameObject, Bone x, float initialX)
        {
            float lowerLimit = Mathf.Min(MinimalDistanceX, initialX);
            while (sk.a > 0.01f && x.x > DissapearPositionX && gameObject)
            {
                if ((SBC?.isInGame ?? false) && (!SBC?.isPause ?? false))
                {
                    UpdateAlphaValue(sk, x.x, MinimalDistanceX, lowerLimit, DissapearPositionX);
                }
                yield return WFS;
            }
            sk.a = 0f;
        }

        private static IEnumerator UpdateAlphaR(Skeleton sk, GameObject gameObject, Bone y, float initialR)
        {
            while (sk.a > 0.01f && y.rotation > DissapearPositionR && gameObject)
            {
                if ((SBC?.isInGame ?? false) && (!SBC?.isPause ?? false))
                {
                    UpdateAlphaValue(sk, y.rotation, MinimalDistanceR, initialR, DissapearPositionR);
                }
                yield return WFS;
            }
            sk.a = 0f;
        }

        private static IEnumerator UpdateAlphaNote(Skeleton sk, GameObject gameObject)
        {
            //Waiting for the proper position
            yield return null;

            while (sk.a > 0.01f && gameObject)
            {
                if ((SBC?.isInGame ?? false) && (!SBC?.isPause ?? false))
                {
                    float x = gameObject?.transform?.position.x ?? -3f;
                    UpdateAlphaValue(sk, x, MinimalDistanceX, MinimalDistanceX, DissapearPositionX);
                }
                yield return WFS;
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
            //Skeleton sk = beoc.m_SkeletonAnimation.skeleton;

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

            Bone xPos = beoc.m_Sac.bones["X"];
            Bone yPos = beoc.m_Sac.bones["Y"];
            AddCallBackEnemy(sk, beoc.gameObject, xPos, yPos);

            //Hearts on notes
            GameObject hpOnNote = beoc.m_Blood;
            if (!hpOnNote) return;

            AddCallBackEnemy(hpOnNote.GetComponent<SkeletonAnimation>().skeleton, beoc.gameObject, xPos, yPos);
            Transform heartFx = hpOnNote.transform.Find("fx");

            if (!heartFx) return;
            heartFx.GetComponent<ParticleSystem>().Stop();
        }
    }
}
