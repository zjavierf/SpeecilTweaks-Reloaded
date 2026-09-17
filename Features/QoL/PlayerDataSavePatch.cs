using HarmonyLib;
using SpeecilTweaks.Configuration;

namespace SpeecilTweaks.Features.QoL
{
    [HarmonyPatch(typeof(PlayerDataModel))]
    public static class PlayerDataSavePatch
    {
        private static string? _tempSavedCustomId;

        [HarmonyPatch("Save")]
        [HarmonyPrefix]
        static void Save_Prefix(PlayerDataModel __instance)
        {
            var colorSettings = __instance?.playerData?.colorSchemesSettings;
            if (colorSettings == null) return;

            string currentId = colorSettings.selectedColorSchemeId;

            if (!string.IsNullOrEmpty(currentId) && currentId.StartsWith("Speecil_"))
            {
                _tempSavedCustomId = currentId;
                
                string cleanName = currentId.Substring("Speecil_".Length);
                if (PluginConfig.Instance?.QoL != null)
                {
                    PluginConfig.Instance.QoL.SelectedPresetName = cleanName;
                }

                colorSettings.selectedColorSchemeId = "User3";
            }
        }

        [HarmonyPatch("Save")]
        [HarmonyPostfix]
        static void Save_Postfix(PlayerDataModel __instance)
        {
            var colorSettings = __instance?.playerData?.colorSchemesSettings;
            if (colorSettings == null || string.IsNullOrEmpty(_tempSavedCustomId)) return;

            colorSettings.selectedColorSchemeId = _tempSavedCustomId;
            _tempSavedCustomId = null;
        }
    }
}