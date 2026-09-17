using HarmonyLib;
using IPA.Utilities.Async;
using SpeecilTweaks.Configuration;
using TMPro;
using UnityEngine;

namespace SpeecilTweaks.Features.MenuTweaks
{
    [HarmonyPatch(typeof(MenuEnvironmentManager))]
    public class MenuLogoTweaks
    {
        private static Transform? _cachedDefaultEnv;
        private static GameObject? _spawnedLogoTextObj;

        [HarmonyPatch("Start")]
        [HarmonyPatch(nameof(MenuEnvironmentManager.ShowEnvironmentType))]
        [HarmonyPriority(int.MinValue)]
        private static void Postfix(MenuEnvironmentManager __instance)
        {
            if (__instance != null)
            {
                _cachedDefaultEnv = __instance.transform.Find("DefaultMenuEnvironment");
            }
            ApplyLogoTweaks();
        }

        public static void ApplyLogoTweaks()
        {
            if (PluginConfig.Instance?.Text == null) return;

            UnityMainThreadTaskScheduler.Factory.StartNew(async () =>
            {
                await System.Threading.Tasks.Task.Yield();

                if (_cachedDefaultEnv == null)
                {
                    var envManager = Object.FindObjectOfType<MenuEnvironmentManager>();
                    if (envManager != null)
                    {
                        _cachedDefaultEnv = envManager.transform.Find("DefaultMenuEnvironment");
                    }
                }

                if (_cachedDefaultEnv == null) return;

                var textConfig = PluginConfig.Instance.Text;
                var colorConfig = PluginConfig.Instance.Colors;
                
                if (textConfig.HideMenuLogo)
                {
                    SafeDisableChild(_cachedDefaultEnv, "Logo");
                    SafeDisableChild(_cachedDefaultEnv, "GlowLines");
                    SafeDisableChild(_cachedDefaultEnv, "GlowLines (1)");
                }
                else
                {
                    SafeEnableChild(_cachedDefaultEnv, "Logo");
                    SafeEnableChild(_cachedDefaultEnv, "GlowLines");
                    SafeEnableChild(_cachedDefaultEnv, "GlowLines (1)");
                }
                
                if (textConfig.EnableCustomMenuLogoText && !string.IsNullOrEmpty(textConfig.CustomMenuLogoText))
                {
                    CreateOrUpdateCustomLogo(_cachedDefaultEnv, textConfig.CustomMenuLogoText, colorConfig);
                }
                else
                {
                    if (_spawnedLogoTextObj != null)
                    {
                        Object.Destroy(_spawnedLogoTextObj);
                        _spawnedLogoTextObj = null;
                    }
                }
            });
        }

        private static void SafeDisableChild(Transform parent, string childName)
        {
            var targetChild = parent.Find(childName);
            if (targetChild != null && targetChild.gameObject.activeSelf)
            {
                targetChild.gameObject.SetActive(false);
            }
        }

        private static void SafeEnableChild(Transform parent, string childName)
        {
            var targetChild = parent.Find(childName);
            if (targetChild != null && !targetChild.gameObject.activeSelf)
            {
                targetChild.gameObject.SetActive(true);
            }
        }

        private static void CreateOrUpdateCustomLogo(Transform defaultEnv, string customText, PluginConfig.ColorSettings colorConfig)
        {
            Color mainColor = new Color32(
                (byte)colorConfig.CustomLogoColorR,
                (byte)colorConfig.CustomLogoColorG,
                (byte)colorConfig.CustomLogoColorB,
                255
            );
            
            int layerCount = 12;
            float depthStep = 0.12f;

            if (_spawnedLogoTextObj == null)
            {
                var originalLogo = defaultEnv.Find("Logo");
                Vector3 spawnPosition = originalLogo != null ? originalLogo.position : new Vector3(0f, 3.7f, 15f);
                Quaternion spawnRotation = originalLogo != null ? originalLogo.rotation : Quaternion.identity;

                _spawnedLogoTextObj = new GameObject("Speecil_CustomMenuLogoText_CachedStack");
                _spawnedLogoTextObj.transform.SetPositionAndRotation(spawnPosition, spawnRotation);
                _spawnedLogoTextObj.transform.localScale = Vector3.one * 1.0f;
                _spawnedLogoTextObj.transform.SetParent(defaultEnv, true);
            }
            
            bool needsRecreation = _spawnedLogoTextObj.transform.childCount != layerCount;

            if (needsRecreation)
            {
                foreach (Transform child in _spawnedLogoTextObj.transform)
                {
                    Object.Destroy(child.gameObject);
                }

                for (int i = layerCount - 1; i >= 0; i--)
                {
                    GameObject layerObj = new GameObject($"Layer_{i}");
                    layerObj.transform.SetParent(_spawnedLogoTextObj.transform, false);
                    layerObj.transform.localPosition = new Vector3(0f, 0f, i * depthStep);
                    layerObj.transform.localRotation = Quaternion.identity;

                    var textMesh = layerObj.AddComponent<TextMeshPro>();
                    textMesh.fontSize = 85f;
                    textMesh.alignment = TextAlignmentOptions.Center;
                    textMesh.enableWordWrapping = false;
                    textMesh.raycastTarget = false;
                }
            }
            
            int index = 0;
            for (int i = layerCount - 1; i >= 0; i--, index++)
            {
                var childTransform = _spawnedLogoTextObj.transform.GetChild(index);
                var textMesh = childTransform.GetComponent<TextMeshPro>();
                if (textMesh != null)
                {
                    textMesh.text = customText;

                    if (i == 0)
                    {
                        textMesh.color = mainColor;
                        textMesh.outlineWidth = 0.25f;
                    }
                    else
                    {
                        float shade = Mathf.Lerp(0.05f, 0.45f, (float)i / layerCount);
                        textMesh.color = new Color(mainColor.r * 0.08f, mainColor.g * shade, mainColor.b * 0.08f);
                        textMesh.outlineWidth = 0f;
                    }
                }
            }
        }
    }
}