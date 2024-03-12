using MelonLoader;
using System.Collections;
using UnityEngine;
using static FadeIn.Managers.SettingsManager;

namespace FadeIn.Managers
{
    internal static partial class ModManager
    {
        private static void UpdateAlphaValuePress(SpriteRenderer sr, ref Color baseColor, float coordinate, float LowerLimit, float initial, float LowerPosition)
        {
            if (coordinate > LowerLimit) return;
            baseColor.a = Mathf.Clamp(
                (coordinate - LowerPosition) / (initial - LowerPosition),
                0f, sr.color.a);
            sr.color = baseColor;
        }

        private static IEnumerator UpdateAlphaPress(SpriteRenderer start, SpriteRenderer end, Material mtrl, GameObject gameObject, Transform transform)
        {
            Color startColor = new(1, 1, 1, 1);
            Color endColor = new(1, 1, 1, 1);

            yield return null;

            while (gameObject)
            {
                yield return WFS;
                if ((!SBC?.isInGame ?? true) || (SBC?.isPause ?? true)) continue;

                float startX = gameObject.transform?.position.x ?? -3f;
                float endX = transform?.position.x ?? -3f;

                UpdateAlphaValuePress(start, ref startColor, startX, MinimalDistanceX, MinimalDistanceX, DisappearPositionX);
                UpdateAlphaValuePress(end, ref endColor, endX, MinimalDistanceX, MinimalDistanceX, DisappearPositionX);
                mtrl.SetFloat("_Alpha", (startColor.a + endColor.a) / 2);

                if (!gameObject.active) break;
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
            float clip;
            float startx;

            while (gameObject)
            {
                yield return WFS;
                if ((!SBC?.isInGame ?? true) || (SBC?.isPause ?? true)) continue;

                startx = startTransform.position.x;
                if (startx > DisappearPositionX) continue;

                clip = Mathf.Clamp((DisappearPositionX - startx) / length, 0f, 1f);
                mtrl.SetFloat("_ClipValue", clip);

                if (!gameObject.active) break;
            }

            clip = 1f;
            mtrl.SetFloat("_ClipValue", clip);
        }

        internal static void AddCallBackPress(GameObject gameObject, SpriteRenderer start, SpriteRenderer end, Material mtrl)
        {
            if (PressList.Contains(gameObject.name)) return;

            Transform endTransform = gameObject.transform.GetChild(0).GetChild(1);

            CoroutinesList.Add(MelonCoroutines.Start(UpdateAlphaPress(start, end, mtrl, gameObject, endTransform)));
            CoroutinesList.Add(MelonCoroutines.Start(UpdateClip(mtrl, gameObject, endTransform)));

            PressList.Add(gameObject.name);
        }
    }
}
