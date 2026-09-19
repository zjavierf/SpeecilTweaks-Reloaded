using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.XR;
using SpeecilTweaks.Configuration;

namespace SpeecilTweaks.Features.Performance
{
    public class SpeecilPerformanceManager : MonoBehaviour
    {
        private static SpeecilPerformanceManager? _instance;
        private static long _initialMemoryBeforeSong;
        private const long GCMemoryThresholdBytes = 250L * 1024L * 1024L; 
        private static bool _lastKnownXrState = true;

        public static void Init()
        {
            if (_instance == null)
            {
                var obj = new GameObject("SpeecilPerformanceManager");
                _instance = obj.AddComponent<SpeecilPerformanceManager>();
                DontDestroyOnLoad(obj);
                _instance.StartCoroutine(_instance.MonitorPerformanceRoutine());
                Plugin.Log?.Info("[SpeecilPerformanceManager] Initialized, marked DontDestroyOnLoad, and started monitor routine.");
            }
        }

        public static void ApplyAllOptimizations()
        {
            var config = PluginConfig.Instance?.Performance;
            if (config == null) return;

            Init();

            ApplyPhysicsOptimization(config.EnablePhysicsOptimization);
            ApplyTextureOptimization(config.EnableTextureOptimization);
            ApplyHighPriority(config.EnableHighPriority);
        }

        public static void ApplyPhysicsOptimization(bool enable)
        {
            if (enable)
            {
                float targetRate = 90f; 
                string source = "Default Fallback";
                float xrRate = 0f;

                bool isXrActive = UnityEngine.XR.Management.XRGeneralSettings.Instance?.Manager?.activeLoader != null;

                if (isXrActive)
                {
                    var xrDisplays = new List<XRDisplaySubsystem>();
                    SubsystemManager.GetInstances(xrDisplays);

                    if (xrDisplays.Count > 0 && xrDisplays[0].running)
                    {
                        if (xrDisplays[0].TryGetDisplayRefreshRate(out xrRate) && xrRate > 0f)
                        {
                            targetRate = xrRate;
                            source = "XRDisplaySubsystem";
                        }
                    }
                
                    if (xrRate <= 0f)
                    {
                        float legacyRate = XRDevice.refreshRate;
                        if (legacyRate > 0f)
                        {
                            targetRate = legacyRate;
                            source = "XRDevice.refreshRate";
                        }
                    }
                }
                else
                {
                    float monitorRate = (float)Screen.currentResolution.refreshRateRatio.value;
                    if (monitorRate > 0f)
                    {
                        targetRate = monitorRate;
                        source = "Screen.currentResolution (FPFC Mode)";
                    }
                }

                Time.fixedDeltaTime = 1f / targetRate;
                Plugin.Log?.Info($"[PerformanceManager] Physics optimization enabled | Target Rate: {targetRate}Hz | Source: {source} | fixedDeltaTime: {Time.fixedDeltaTime}");
            }
            else
            {
                Time.fixedDeltaTime = 0.02f;
                Plugin.Log?.Info("[PerformanceManager] Physics optimization disabled (fixedDeltaTime reset to 0.02).");
            }
        }

        public static void ApplyTextureOptimization(bool enable)
        {
            QualitySettings.globalTextureMipmapLimit = enable ? 1 : 0;
            Plugin.Log?.Info($"[PerformanceManager] Texture mipmap limit set to {(enable ? 1 : 0)}.");
        }

        public static void ApplyHighPriority(bool enable)
        {
            try
            {
                using var currentProcess = Process.GetCurrentProcess();
                currentProcess.PriorityClass = enable ? ProcessPriorityClass.High : ProcessPriorityClass.Normal;
                Plugin.Log?.Info($"[PerformanceManager] Process priority set to: {currentProcess.PriorityClass}");
            }
            catch (Exception ex)
            {
                Plugin.Log?.Error($"[PerformanceManager] Failed to toggle process priority: {ex.Message}");
            }
        }

        public static void SetGameStarted(bool started)
        {
            var config = PluginConfig.Instance?.Performance;
            if (config?.EnableGarbageCollectionControl != true) return;

            try
            {
                if (started)
                {
                    _initialMemoryBeforeSong = GC.GetTotalMemory(false);
                    GarbageCollector.GCMode = GarbageCollector.Mode.Disabled;
                    Plugin.Log?.Info("[PerformanceManager] Garbage collection disabled for song performance.");
                }
                else
                {
                    GarbageCollector.GCMode = GarbageCollector.Mode.Enabled;
                    GC.Collect();
                    Plugin.Log?.Info("[PerformanceManager] Garbage collection re-enabled and executed.");
                }
            }
            catch (Exception ex)
            {
                Plugin.Log?.Error($"[PerformanceManager] Failed to toggle GC mode: {ex.Message}");
            }
        }

        private IEnumerator MonitorPerformanceRoutine()
        {
            var wait = new WaitForSecondsRealtime(5f);
            while (true)
            {
                yield return wait;

                var config = PluginConfig.Instance?.Performance;
                if (config == null) continue;
                
                bool currentXrState = UnityEngine.XR.Management.XRGeneralSettings.Instance?.Manager?.activeLoader != null;
                if (currentXrState != _lastKnownXrState)
                {
                    _lastKnownXrState = currentXrState;
                    Plugin.Log?.Info($"[PerformanceManager] XR Loader state changed (FPFC active: {!currentXrState}). Refreshing physics rate...");
                    ApplyPhysicsOptimization(config.EnablePhysicsOptimization);
                }
                
                if (config.EnableGarbageCollectionControl && GarbageCollector.GCMode == GarbageCollector.Mode.Disabled)
                {
                    try
                    {
                        if (AudioListener.pause || Time.timeScale == 0f)
                        {
                            continue;
                        }

                        long currentMemory = GC.GetTotalMemory(false);
                        if (currentMemory - _initialMemoryBeforeSong > GCMemoryThresholdBytes)
                        {
                            Plugin.Log?.Warn($"[PerformanceManager] Memory allocation threshold reached (>250MB allocated during song). Triggering safe GC pass.");
                            GarbageCollector.GCMode = GarbageCollector.Mode.Enabled;
                            GC.Collect();
                            _initialMemoryBeforeSong = GC.GetTotalMemory(false);
                            GarbageCollector.GCMode = GarbageCollector.Mode.Disabled;
                        }
                    }
                    catch (Exception ex)
                    {
                        Plugin.Log?.Error($"[PerformanceManager] Error during GC safety check: {ex.Message}");
                    }
                }
            }
        }

        public static void PurgeUnusedAssets()
        {
            if (PluginConfig.Instance?.Performance.EnableAssetPurge != true) return;
            Plugin.Log?.Info("[PerformanceManager] Purging unused assets...");
            Resources.UnloadUnusedAssets();
        }
    }
}
