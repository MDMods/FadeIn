using UnityEngine;

namespace FadeIn
{
    internal static class Extensions
    {
        public static void Destroy(this GameObject gameObject) => UnityEngine.Object.Destroy(gameObject);
    }
}
