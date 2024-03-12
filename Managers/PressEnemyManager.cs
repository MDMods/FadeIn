using System.Collections;
using MelonLoader;
using UnityEngine;

namespace FadeIn.Managers;

using static ModManager;
using static SettingsManager;

internal static class PressEnemyManager
{
    private static readonly int Alpha = Shader.PropertyToID("_Alpha");
    private static readonly int ClipValue = Shader.PropertyToID("_ClipValue");

    private static void UpdateAlphaValuePress(SpriteRenderer sr, ref Color baseColor, float coordinate,
        float lowerLimit, float initial, float lowerPosition)
    {
        if (coordinate > lowerLimit) return;
        baseColor.a = Mathf.Clamp(
            (coordinate - lowerPosition) / (initial - lowerPosition),
            0f, sr.color.a);
        sr.color = baseColor;
    }

    private static IEnumerator UpdateAlphaPress(SpriteRenderer start, SpriteRenderer end, Material mtrl,
        GameObject gameObject, Transform transform)
    {
        Color startColor = new(1, 1, 1, 1);
        Color endColor = new(1, 1, 1, 1);

        yield return null;

        while (gameObject)
        {
            yield return WFS;
            if ((!SBC?.isInGame ?? true) || (SBC?.isPause ?? true)) continue;

            var startX = gameObject.transform.position.x;
            var endX = transform.position.x;

            UpdateAlphaValuePress(start, ref startColor, startX, MinimalDistanceX, MinimalDistanceX,
                DisappearPositionX);
            UpdateAlphaValuePress(end, ref endColor, endX, MinimalDistanceX, MinimalDistanceX, DisappearPositionX);
            mtrl.SetFloat(Alpha, (startColor.a + endColor.a) / 2);

            if (!gameObject.active) break;
        }

        startColor.a = 0;
        endColor.a = 0;
        start.color = startColor;
        end.color = endColor;
    }

    private static IEnumerator UpdateClip(Material mtrl, GameObject gameObject, Transform endTransform)
    {
        var startTransform = gameObject.transform;
        var length = endTransform.position.x - startTransform.position.x;
        float clip;
        float startx;

        while (gameObject)
        {
            yield return WFS;
            if ((!SBC?.isInGame ?? true) || (SBC?.isPause ?? true)) continue;

            startx = startTransform.position.x;
            if (startx > DisappearPositionX) continue;

            clip = Mathf.Clamp((DisappearPositionX - startx) / length, 0f, 1f);
            mtrl.SetFloat(ClipValue, clip);

            if (!gameObject.active) break;
        }

        clip = 1f;
        mtrl.SetFloat(ClipValue, clip);
    }

    internal static void AddCallBackPress(GameObject gameObject, SpriteRenderer start, SpriteRenderer end,
        Material mtrl)
    {
        if (PressList.Contains(gameObject.name)) return;

        var endTransform = gameObject.transform.GetChild(0).GetChild(1);

        CoroutinesList.Add(MelonCoroutines.Start(UpdateAlphaPress(start, end, mtrl, gameObject, endTransform)));
        CoroutinesList.Add(MelonCoroutines.Start(UpdateClip(mtrl, gameObject, endTransform)));

        PressList.Add(gameObject.name);
    }
}