using Il2CppAssets.Scripts.PeroTools.Commons;
using Il2CppFormulaBase;
using Il2CppSpine;
using Il2CppSpine.Unity;
using System.Collections.Concurrent;
using UnityEngine;

namespace FadeIn
{
    internal static class ModManager
    {
        internal static bool _enabled;
        public static bool Enabled
        {
            get { return _enabled; }
            set
            {
                _enabled = value;
                if (_enabled)
                {
                    OnSceneWasLoadedFunc = ClearSceneElements;
                    EnableVisiblePatch = EnqueueSkeleton;
                    OnUpdateFunc = UpdateQueueElements;
                }
                else
                {
                    OnSceneWasLoadedFunc = () => { };
                    EnableVisiblePatch = (_) => { };
                    OnUpdateFunc = () => { };
                }
            }
        }

        public static GameObject FadeToggle = null;

        public static bool battleScene = false;
        public static readonly ConcurrentQueue<Skeleton> enemiesSkeletons = new();

        internal delegate void OnSceneWasLoadedFuncType();
        public static OnSceneWasLoadedFuncType OnSceneWasLoadedFunc;

        internal delegate void EnableVisiblePatchFuncType(GameObject beoc);
        public static EnableVisiblePatchFuncType EnableVisiblePatch;

        internal delegate void OnUpdateFuncType();
        public static OnUpdateFuncType OnUpdateFunc;

        public static readonly float frequency = 1f / Time.fixedDeltaTime;

        internal static void ClearSceneElements()
        {
            enemiesSkeletons.Clear();
        }

        internal static void EnqueueSkeleton(GameObject beoc)
        {
            Skeleton currentEnemySkeleton = beoc.GetComponent<SkeletonAnimation>().skeleton;
            currentEnemySkeleton.a = 0.7f;
            enemiesSkeletons.Enqueue(currentEnemySkeleton);
            // Check airmusicnodecontroller for note particles
        }

        internal static void UpdateQueueElements()
        {
            StageBattleComponent sbc = Singleton<StageBattleComponent>.instance;
            if ((!sbc?.isInGame ?? true) || (sbc?.isPause ?? true)) return;
            float delta = 0.01f * frequency * Time.deltaTime;

            foreach (Skeleton skeleton in enemiesSkeletons)
            {
                skeleton.a -= delta;
                if (skeleton.a <= 0)
                {
                    skeleton.a = 0f;
                    enemiesSkeletons.TryDequeue(out _);
                }
            }
        }
    }
}