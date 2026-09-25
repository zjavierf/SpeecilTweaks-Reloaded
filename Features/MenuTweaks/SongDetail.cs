using HarmonyLib;
using SpeecilTweaks.Configuration;
using UnityEngine;
using UnityEngine.UI;

namespace SpeecilTweaks.Features.MenuTweaks
{
    internal static class SongDetailsHelper
    {
        public static void ApplyTweaksToView(Transform viewTransform)
        {
            if (viewTransform == null) return;

            var buttons = viewTransform.GetComponentsInChildren<Button>(true);
            foreach (var button in buttons)
            {
                if (button == null) continue;

                var textComp = button.GetComponentInChildren<HMUI.CurvedTextMeshPro>(true);
                if (textComp == null) continue;

                string currentText = textComp.text?.Trim().ToUpperInvariant() ?? "";

                if (currentText == "PLAY" || currentText == "SG" || button.name.Contains("Action") || button.name.Contains("Play"))
                {
                    var color = new Color32(
                        (byte)PluginConfig.Instance.Colors.PlayButtonR, 
                        (byte)PluginConfig.Instance.Colors.PlayButtonG, 
                        (byte)PluginConfig.Instance.Colors.PlayButtonB, 
                        255
                    );
                    ApplyButtonTweaks(button, PluginConfig.Instance.Text.PlayText, color);
                }
                else if (currentText == "PRACTICE" || button.name.Contains("Practice"))
                {
                    var color = new Color32(
                        (byte)PluginConfig.Instance.Colors.PracticeButtonR, 
                        (byte)PluginConfig.Instance.Colors.PracticeButtonG, 
                        (byte)PluginConfig.Instance.Colors.PracticeButtonB, 
                        255
                    );
                    ApplyButtonTweaks(button, PluginConfig.Instance.Text.PracticeText, color);
                }
            }
        }

        private static void ApplyButtonTweaks(Button button, string customText, Color targetColor)
        {
            if (button == null) return;

            var textComp = button.GetComponentInChildren<HMUI.CurvedTextMeshPro>(true);
            if (textComp != null)
            {
                textComp.SetText(customText);
                
                textComp.enableVertexGradient = false;
                textComp.color = targetColor;
                
                if (textComp.fontMaterial != null)
                {
                    textComp.fontMaterial.SetColor("_FaceColor", targetColor);
                }

                textComp.SetAllDirty();
            }
            
            foreach (var comp in button.GetComponentsInChildren<Component>(true))
            {
                if (comp == null) continue;
                string typeName = comp.GetType().Name;
                if (typeName.Contains("TextTransition") || 
                    typeName.Contains("ColorTransition") || 
                    typeName.Contains("GraphicTransition") ||
                    typeName.Contains("StateTransition") ||
                    typeName.Contains("Localize") ||
                    typeName.Contains("ButtonSpriteSwap") ||
                    typeName.Contains("ToggleBinder"))
                {
                    Object.Destroy(comp);
                }
            }
        }
    }

    [HarmonyPatch(typeof(StandardLevelDetailView), "RefreshContent")]
    public static class StandardLevelDetailViewRefreshContent
    {
        static void Postfix(StandardLevelDetailView __instance)
        {
            if (__instance != null)
                SongDetailsHelper.ApplyTweaksToView(__instance.transform);
        }
    }

    [HarmonyPatch(typeof(StandardLevelDetailViewController), "DidActivate")]
    public static class StandardLevelDetailViewControllerDidActivate
    {
        static void Postfix(StandardLevelDetailViewController __instance)
        {
            if (__instance != null)
                SongDetailsHelper.ApplyTweaksToView(__instance.transform);
        }
    }

    [HarmonyPatch(typeof(StandardLevelDetailView), "SetData")]
    public static class StandardLevelDetailViewSetData
    {
        static void Postfix(StandardLevelDetailView __instance)
        {
            if (__instance != null)
                SongDetailsHelper.ApplyTweaksToView(__instance.transform);
        }
    }
}
