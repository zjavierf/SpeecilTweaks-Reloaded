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
            if (!string.IsNullOrEmpty(selectedId) && selectedId.StartsWith("Speecil_"))
            {
                string cleanName = selectedId.Substring("Speecil_".Length);
                if (PluginConfig.Instance?.QoL != null)
                {
                    PluginConfig.Instance.QoL.SelectedPresetName = cleanName;
                    Plugin.Log?.Info($"[MoreColorPresets] User selected custom scheme '{cleanName}'. Saved to config.");
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

                if (!ColorUtility.TryParseHtmlString(preset.SaberLeftHex, out Color saberLeft) ||
                    !ColorUtility.TryParseHtmlString(preset.SaberRightHex, out Color saberRight) ||
                    !ColorUtility.TryParseHtmlString(preset.EnvLeftHex, out Color envLeft) ||
                    !ColorUtility.TryParseHtmlString(preset.EnvRightHex, out Color envRight) ||
                    !ColorUtility.TryParseHtmlString(preset.ObstacleHex, out Color obstacle))
                {
                    continue;
                }

                string targetId = $"Speecil_{preset.Name}";
                if (customSchemesList.Exists(s => s != null && s.colorSchemeId == targetId)) continue;

                ColorScheme scheme = new ColorScheme(
                    targetId, preset.Name, true, preset.Name, false, true,
                    saberLeft, saberRight, true, envLeft, envRight, envLeft,
                    true, envLeft, envRight, envLeft, obstacle
                );

                customSchemesList.Insert(insertIndex + injectedCount, scheme);

                if (colorSchemesDict != null)
                {
                    if (!colorSchemesDict.Contains(targetId))
                    {
                        colorSchemesDict.Add(targetId, scheme);
                    }
                    else
                    {
                        colorSchemesDict[targetId] = scheme; // Safely update existing key
                    }
                }

                injectedCount++;
            }
            Plugin.Log?.Info($"[MoreColorPresets] Injected {injectedCount} custom scheme(s) cleanly.");
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