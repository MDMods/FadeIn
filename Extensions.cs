using UnityEngine;

namespace FadeIn
{
    internal static class Extensions
    {
        public static void Destroy(this Component component) => UnityEngine.Object.Destroy(component);
    }
}
