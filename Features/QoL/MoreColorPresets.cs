using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using SpeecilTweaks.Configuration;
using UnityEngine;

namespace SpeecilTweaks.Features.QoL
{
    [HarmonyPatch]
    public static class MoreColorPresetsPatch
    {
        private static FieldInfo? _cachedSchemesListField;

        [HarmonyPatch(typeof(ColorsOverrideSettingsPanelController), "SetData")]
        [HarmonyPrefix]
        static void SettingsPanelController_SetData_Prefix(ref ColorSchemesSettings colorSchemesSettings)
        {
            InjectCustomPresets(colorSchemesSettings);
        }

        [HarmonyPatch(typeof(ColorsOverrideSettingsPanelController), "HandleDropDownDidSelectCellWithIdx")]
        [HarmonyPostfix]
        static void HandleDropDownDidSelectCellWithIdx_Postfix(ColorsOverrideSettingsPanelController __instance)
        {
            if (__instance == null) return;

            var traverse = Traverse.Create(__instance);
            var colorSettings = traverse.Field("_colorSchemesSettings")?.GetValue<ColorSchemesSettings>();
            if (colorSettings == null) return;

            string selectedId = colorSettings.selectedColorSchemeId;
            if (!string.IsNullOrEmpty(selectedId))
            {
                if (PluginConfig.Instance?.QoL != null)
                {
                    PluginConfig.Instance.QoL.SelectedPresetId = selectedId;
                    Plugin.SaveConfig();
                    Plugin.Log?.Info($"[MoreColorPresets] User selected color scheme ID '{selectedId}'. Saved to config.");
                }
            }
        }

        private static List<ColorScheme>? GetCustomSchemesList(ColorSchemesSettings settings)
        {
            if (settings == null) return null;

            if (_cachedSchemesListField == null)
            {
                try
                {
                    foreach (var field in AccessTools.GetDeclaredFields(settings.GetType()))
                    {
                        if (field != null && field.FieldType != null && typeof(IEnumerable<ColorScheme>).IsAssignableFrom(field.FieldType))
                        {
                            _cachedSchemesListField = field;
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Plugin.Log?.Error($"[MoreColorPresets] Error finding schemes list field via reflection: {ex.Message}");
                }
            }
            return _cachedSchemesListField?.GetValue(settings) as List<ColorScheme>;
        }

        public static void InjectCustomPresets(ColorSchemesSettings colorSchemesSettings)
        {
            if (colorSchemesSettings == null) return;

            var customPresets = PluginConfig.Instance?.QoL?.Presets;
            if (customPresets == null || customPresets.Count == 0) return;

            var customSchemesList = GetCustomSchemesList(colorSchemesSettings);
            if (customSchemesList == null)
            {
                Plugin.Log?.Error("[MoreColorPresets] Could not find List<ColorScheme> via reflection.");
                return;
            }

            int insertIndex = customSchemesList.FindLastIndex(s => s != null && s.isEditable);
            insertIndex = (insertIndex == -1) ? 4 : insertIndex + 1;

            var traverse = Traverse.Create(colorSchemesSettings);
            IDictionary? colorSchemesDict = traverse.Field("_colorSchemesDict")?.GetValue<IDictionary>()
                                          ?? traverse.Field("_overrideColorSchemesDict")?.GetValue<IDictionary>()
                                          ?? traverse.Field("_colorSchemes")?.GetValue<IDictionary>();

            int injectedCount = 0;

            foreach (var preset in customPresets)
            {
                if (preset == null || string.IsNullOrEmpty(preset.Name)) continue;

                string envColorHex = preset.EnvHex;
                if (PluginConfig.Instance?.QoL?.UsePreferredEnvColor == true && !string.IsNullOrEmpty(PluginConfig.Instance.QoL.PreferredEnvHex))
                {
                    envColorHex = PluginConfig.Instance.QoL.PreferredEnvHex;
                }

                if (!ColorUtility.TryParseHtmlString(preset.SaberLeftHex, out Color saberLeft) ||
                    !ColorUtility.TryParseHtmlString(preset.SaberRightHex, out Color saberRight) ||
                    !ColorUtility.TryParseHtmlString(envColorHex, out Color envColor) ||
                    !ColorUtility.TryParseHtmlString(preset.ObstacleHex, out Color obstacle))
                {
                    continue;
                }

                string targetId = $"Speecil_{preset.Name}";
                
                customSchemesList.RemoveAll(s => s != null && s.colorSchemeId == targetId);
                if (colorSchemesDict != null && colorSchemesDict.Contains(targetId))
                {
                    colorSchemesDict.Remove(targetId);
                }
                
                ColorScheme scheme = new ColorScheme(
                    targetId, preset.Name, true, preset.Name, false, true,
                    saberLeft, saberRight, true, envColor, envColor, envColor,
                    true, envColor, envColor, envColor, obstacle
                );

                customSchemesList.Insert(insertIndex + injectedCount, scheme);

                if (colorSchemesDict != null)
                {
                    colorSchemesDict.Add(targetId, scheme);
                }

                injectedCount++;
            }

            if (PluginConfig.Instance?.QoL != null && !string.IsNullOrEmpty(PluginConfig.Instance.QoL.SelectedPresetId))
            {
                string savedId = PluginConfig.Instance.QoL.SelectedPresetId;
                if (colorSchemesDict == null || colorSchemesDict.Contains(savedId))
                {
                    colorSchemesSettings.selectedColorSchemeId = savedId;
                }
            }
        }

        public static void RefreshActiveSettingsPanel()
        {
            var panelController = UnityEngine.Object.FindObjectOfType<ColorsOverrideSettingsPanelController>();
            if (panelController != null)
            {
                var playerDataModel = UnityEngine.Object.FindObjectOfType<PlayerDataModel>();
                if (playerDataModel?.playerData?.colorSchemesSettings != null)
                {
                    var colorSettings = playerDataModel.playerData.colorSchemesSettings;
                    InjectCustomPresets(colorSettings);
                    panelController.SetData(colorSettings);
                }
            }
        }

        public static void RemoveCustomPreset(ColorSchemesSettings colorSchemesSettings, string presetName)
        {
            if (colorSchemesSettings == null || string.IsNullOrEmpty(presetName)) return;

            string targetId = $"Speecil_{presetName}";
            var customSchemesList = GetCustomSchemesList(colorSchemesSettings);
            customSchemesList?.RemoveAll(s => s != null && s.colorSchemeId == targetId);

            var traverse = Traverse.Create(colorSchemesSettings);
            IDictionary? colorSchemesDict = traverse.Field("_colorSchemesDict")?.GetValue<IDictionary>()
                                          ?? traverse.Field("_overrideColorSchemesDict")?.GetValue<IDictionary>()
                                          ?? traverse.Field("_colorSchemes")?.GetValue<IDictionary>();

            if (colorSchemesDict != null && colorSchemesDict.Contains(targetId))
            {
                colorSchemesDict.Remove(targetId);
            }

            if (colorSchemesSettings.selectedColorSchemeId == targetId)
            {
                colorSchemesSettings.selectedColorSchemeId = "User3";
            }
        }
    }
}
