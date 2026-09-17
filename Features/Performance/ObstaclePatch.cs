using HarmonyLib;
using IPA.Utilities;
using SpeecilTweaks.Configuration;
using UnityEngine;

namespace SpeecilTweaks.Features.Performance
{
    [HarmonyPatch(typeof(PlayerHeadAndObstacleInteraction), nameof(PlayerHeadAndObstacleInteraction.playerHeadIsInObstacle), MethodType.Getter)]
    internal class ObstaclePatch
    {
        internal static ObstacleDetector? obstacleDetector;

        static bool Prefix(ref bool __result)
        {
            if (PluginConfig.Instance?.Performance?.EnableObstacleOptimization != true)
            {
                return true;
            }

            if (obstacleDetector == null)
            {
                InitializeDetector();
            }

            if (obstacleDetector != null)
            {
                __result = obstacleDetector.intersectingObstacles.Count > 0;
                return false;
            }
            return true;
        }

        private static void InitializeDetector()
        {
            try
            {
                var playerTransforms = Object.FindObjectOfType<PlayerTransforms>();
                if (playerTransforms == null) return;

                Transform headTransform = playerTransforms.GetField<Transform, PlayerTransforms>("_headTransform");
                if (headTransform == null) return;

                GameObject detectorObject = new GameObject("SpeecilObstacleDetector");
                detectorObject.transform.SetParent(headTransform, false);
                detectorObject.layer = 9;

                obstacleDetector = detectorObject.AddComponent<ObstacleDetector>();
                Plugin.Log?.Info("[SpeecilTweaks] Obstacle detector initialized dynamically via patch");
            }
            catch (System.Exception e)
            {
                Plugin.Log?.Error($"[SpeecilTweaks] Failed to initialize obstacle detector: {e.Message}");
            }
        }
    }
}