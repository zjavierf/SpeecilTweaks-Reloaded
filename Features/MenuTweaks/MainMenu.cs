using HarmonyLib;
using SpeecilTweaks.Configuration;
using UnityEngine;

namespace SpeecilTweaks.Features.MenuTweaks
{
    [HarmonyPatch(typeof(MainMenuViewController), "DidActivate")]
    public static class MainMenuViewControllerDidActivate
    {
        static void Postfix(MainMenuViewController __instance, bool firstActivation)
        {
            var color = new Color32((byte)PluginConfig.Instance.Colors.SoloButtonR, (byte)PluginConfig.Instance.Colors.SoloButtonG, (byte)PluginConfig.Instance.Colors.SoloButtonB, 255);
            var transform = __instance.transform;

            var buttonTransform = FindRecursive(transform, "SoloButton");
            if (buttonTransform != null)
            {
                var text = buttonTransform.GetComponentInChildren<HMUI.CurvedTextMeshPro>(true);
                if (text != null)
                {
                    text.color = color;
                    text.text = PluginConfig.Instance.Text.SoloText;
                    
                    foreach (var comp in buttonTransform.GetComponentsInChildren<Component>(true))
                    {
                        if (comp.GetType().Name.Contains("Localize"))
                        {
                            Object.Destroy(comp);
                        }
                    }
                }
            }
        }

        private static Transform? FindRecursive(Transform parent, string name)
        {
            foreach (Transform child in parent)
            {
                if (child == null) continue;
                if (child.name == name) return child;
                var result = FindRecursive(child, name);
                if (result != null) return result;
            }
            return null;
        }
    }
}