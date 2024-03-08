using Il2Cpp;
using Il2CppFormulaBase;
using Il2CppPeroPeroGames.GlobalDefines;
using Il2CppSpine;
using Il2CppSpine.Unity;
using MelonLoader;
using System.Collections;
using UnityEngine;
using static FadeIn.Managers.SettingsManager;

namespace FadeIn.Managers
{
    internal static class ModManager
    {
        // Coroutines
        private static readonly List<object> CoroutinesList = new();
        public static void ClearCoroutines()
        {
            foreach (var coroutine in CoroutinesList) MelonCoroutines.Stop(coroutine);
            CoroutinesList.Clear();
        }

        // Holds 
        private static readonly HashSet<string> PressList = new();
        public static void ClearPress() => PressList.Clear();

        public static GameObject FadeToggle = null;
        public static StageBattleComponent SBC { get; set; } = null;

        //private static readonly WaitForSeconds WFS = new WaitForSeconds(0.05f);
        private static readonly WaitForSeconds WFS = null;

        private static void UpdateAlphaValue(Skeleton sk, float coordinate, float LowerLimit, float initial, float LowerPosition)
        {
            if (coordinate > LowerLimit) return;
            sk.a = Mathf.Clamp(
                (coordinate - LowerPosition) / (initial - LowerPosition),
                0f, sk.a);
        }

        private static void UpdateAlphaValuePress(SpriteRenderer sr, ref Color baseColor, float coordinate, float LowerLimit, float initial, float LowerPosition)
        {
            if (coordinate > LowerLimit) return;
            baseColor.a = Mathf.Clamp(
                (coordinate - LowerPosition) / (initial - LowerPosition),
                0f, sr.color.a);
            sr.color = baseColor;
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

        private static IEnumerator UpdateAlphaPress(SpriteRenderer start, SpriteRenderer end, Material mtrl, GameObject gameObject, Transform transform)
        {
            Color startColor = new(1, 1, 1, 1);
            Color endColor = new(1, 1, 1, 1);

            yield return null;

            while (gameObject)
            {
                if ((SBC?.isInGame ?? false) && (!SBC?.isPause ?? false))
                {
                    float startX = gameObject.transform?.position.x ?? -3f;
                    float endX = transform?.position.x ?? -3f;
                    UpdateAlphaValuePress(start, ref startColor, startX, MinimalDistanceX, MinimalDistanceX, DissapearPositionX);
                    UpdateAlphaValuePress(end, ref endColor, endX, MinimalDistanceX, MinimalDistanceX, DissapearPositionX);
                    mtrl.SetFloat("_Alpha", (startColor.a + endColor.a) / 2);

                    if (startColor.a < 0.01f && endColor.a < 0.01f) break;
                }
                yield return WFS;
            }
            startColor.a = 0;
            endColor.a = 0;
            start.color = startColor;
            end.color = endColor;
        }

        private static IEnumerator UpdateClip(Material mtrl, GameObject gameObject, Transform endTransform)
        {
            Transform startTransform = gameObject.transform;
            float length = endTransform.position.x - startTransform.position.x;
            float clip = 0f;
            float startx;

            while (clip < 1f && gameObject)
            {
                if ((SBC?.isInGame ?? false) && (!SBC?.isPause ?? false))
                {
                    startx = startTransform.position.x;
                    if (startx <= DissapearPositionX)
                    {
                        clip = Mathf.Clamp((DissapearPositionX - startx) / length, 0f, 1f);
                        mtrl.SetFloat("_ClipValue", clip);
                    }
                }
                yield return WFS;
            }
            clip = 1f;
            mtrl.SetFloat("_ClipValue", clip);
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

        internal static void AddCallBackPress(GameObject gameObject, SpriteRenderer start, SpriteRenderer end, Material mtrl)
        {
            if (PressList.Contains(gameObject.name)) return;

            Transform endTransform = gameObject.transform.GetChild(0).GetChild(1);

            CoroutinesList.Add(MelonCoroutines.Start(UpdateAlphaPress(start, end, mtrl, gameObject, endTransform)));
            CoroutinesList.Add(MelonCoroutines.Start(UpdateClip(mtrl, gameObject, endTransform)));

            PressList.Add(gameObject.name);
        }

        internal static void ProcessEnemy(BaseEnemyObjectController beoc, Skeleton sk)
        {
            //Skeleton sk = beoc.m_SkeletonAnimation.skeleton;

            switch (beoc.m_NodeType)
            {
                case (uint)NoteType.Hp:
                    AddCallBackNote(sk, beoc.gameObject);
                    beoc.Cast<AirEnergyBottleController>().m_Fx.SetActive(false);
                    return;
                case (uint)NoteType.Music:
                    AddCallBackNote(sk, beoc.gameObject);
                    beoc.Cast<AirMusicNodeController>().m_Fx.SetActive(false);
                    return;

                // Dont do anything with the holds
                case (uint)NoteType.Press: return;
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