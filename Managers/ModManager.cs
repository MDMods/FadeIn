using Il2Cpp;
using Il2CppFormulaBase;
using Il2CppSpine;
using Il2CppSpine.Unity;
using MelonLoader;
using System.Collections;
using UnityEngine;

namespace FadeIn.Managers
{
    internal static class ModManager
    {
        private static bool _enabled = false;
        public static bool IsEnabled
        {
            get { return _enabled; }
            set { _enabled = value; }
        }

        public static GameObject FadeToggle = null;
        public static StageBattleComponent SBC { get; set; } = null;

        private static readonly float LowerPositionX = -1.8f;
        private static readonly float LowerPositionR = 8.0f;
        private static readonly float MinimalInitialX = 5.8f;
        private static readonly float NoteInitial = 11.8f;
        //private static readonly WaitForSeconds WFS = new WaitForSeconds(0.05f);
        private static readonly WaitForSeconds WFS = null;

        private static IEnumerator UpdateAlphaX(Skeleton sk, Bone x, float initialX)
        {
            float lowerLimit = Mathf.Min(MinimalInitialX, initialX);
            while (sk.a > 0.01f && x.x > LowerPositionX)
            {
                if ((!SBC?.isInGame ?? true) || (SBC?.isPause ?? true)) yield return null;

                if (x.x <= MinimalInitialX)
                    sk.a = Mathf.Clamp((x.x - LowerPositionX) / (lowerLimit - LowerPositionX), 0f, 1f);
                yield return WFS;
            }
            sk.a = 0f;
        }

        private static IEnumerator UpdateAlphaR(Skeleton sk, Bone y, float initialR)
        {
            while (sk.a > 0.01f && y.rotation > LowerPositionR)
            {
                if ((!SBC?.isInGame ?? true) || (SBC?.isPause ?? true)) yield return null;

                if (y.rotation <= 85f)
                    sk.a = Mathf.Clamp((y.rotation - LowerPositionR) / (initialR - LowerPositionR), 0f, 1f);
                yield return WFS;
            }
            sk.a = 0f;
        }

        private static IEnumerator UpdateAlphaNote(Skeleton sk, GameObject gameObject)
        {
            Transform transform = gameObject.transform;
            while (sk.a > 0.01f && transform.position.x > LowerPositionX)
            {
                if ((!SBC?.isInGame ?? true) || (SBC?.isPause ?? true)) yield return null;

                float x = transform.position.x;
                if (x <= MinimalInitialX)
                    sk.a = Mathf.Clamp((x - LowerPositionX) / (MinimalInitialX - LowerPositionX), 0f, 1f);
                yield return WFS;
            }
            sk.a = 0f;
        }

        private static object AddCallBackEnemy(Bone xPos, Bone yPos, Skeleton sk)
        {
            return MelonCoroutines.Start(yPos.rotation > 70f
                    ? UpdateAlphaR(sk, yPos, yPos.rotation)
                    : UpdateAlphaX(sk, xPos, xPos.x));
        }

        private static object AddCallBackNote(GameObject gameObject, Skeleton sk)
        {
            return MelonCoroutines.Start(UpdateAlphaNote(sk, gameObject));
        }

        internal static float ProcessEnemy(BaseEnemyObjectController beoc, Skeleton sk)
        {
            Bone xPos = beoc.m_Sac.bones["X"];
            Bone yPos = beoc.m_Sac.bones["Y"];
            //Skeleton sk = beoc.m_SkeletonAnimation.skeleton;

            // Music note particles
            if (beoc.TryGetComponent(out AirMusicNodeController note))
            {
                ModManager.AddCallBackNote(note.gameObject, sk);
                note.m_Fx.SetActive(false);
            }
            // Heart particles
            else if (beoc.TryGetComponent(out AirEnergyBottleController heart))
            {
                ModManager.AddCallBackNote(heart.gameObject, sk);
                heart.m_Fx.SetActive(false);
            }
            else
            {
                ModManager.AddCallBackEnemy(xPos, yPos, sk);
            }

            //Hearts on notes
            GameObject hpOnNote = beoc.m_Blood;
            if (!hpOnNote) return xPos.x;

            ModManager.AddCallBackEnemy(xPos, yPos, hpOnNote.GetComponent<SkeletonAnimation>().skeleton);
            Transform heartFx = hpOnNote.transform.Find("fx");

            if (!heartFx) return xPos.x;
            heartFx.GetComponent<ParticleSystem>().Stop();

            return xPos.x;
        }

    }
}