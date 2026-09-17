using HarmonyLib;
using System.Collections;

namespace SpeecilTweaks.Patches
{
    [HarmonyPatch(typeof(ColorSchemesSettings))]
    public class ColorSchemeSafetyPatch
    {
        [HarmonyPatch(nameof(ColorSchemesSettings.GetSelectedColorScheme))]
        [HarmonyPatch("GetOverrideColorScheme")]
        public static void Prefix(ColorSchemesSettings __instance)
        {
            try
            {
                var traverse = Traverse.Create(__instance);
                IDictionary? colorSchemesDict = traverse.Field("_colorSchemesDict")?.GetValue<IDictionary>()
                                                ?? traverse.Field("_overrideColorSchemesDict")?.GetValue<IDictionary>()
                                                ?? traverse.Field("_colorSchemes")?.GetValue<IDictionary>();

                var currentId = __instance.selectedColorSchemeId;

                if (!string.IsNullOrEmpty(currentId) && colorSchemesDict != null && !colorSchemesDict.Contains(currentId))
                {
                    __instance.selectedColorSchemeId = "User3";
                }
            }
            catch
            {
                // Swallow reflection exceptions during startup
            }
        }
    }
}