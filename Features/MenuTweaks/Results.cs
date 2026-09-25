using System.Linq;
using HarmonyLib;
using SpeecilTweaks.Configuration;
using UnityEngine;

namespace SpeecilTweaks.Features.MenuTweaks
{
    [HarmonyPatch(typeof(ResultsViewController), "SetDataToUI")]
    public static class ResultsViewControllerSetDataToUI
    {
        static void Postfix(ResultsViewController __instance)
        {
            if (__instance == null) return;
            var transform = __instance.transform;

            var clearedBanner = FindRecursive(transform, "ClearedBanner") ?? FindRecursive(transform, "ClearBanner") ?? FindRecursive(transform, "ResultBanner");
            var failedBanner = FindRecursive(transform, "FailedBanner") ?? FindRecursive(transform, "FailBanner");

            if (clearedBanner != null)
            {
                var text = clearedBanner.GetComponentInChildren<HMUI.CurvedTextMeshPro>(true);
                if (text != null)
                {
                    text.SetText(PluginConfig.Instance.Text.ResultText);
                    RemoveLocalizeComponents(clearedBanner.gameObject);
                }

                var img = clearedBanner.GetComponentsInChildren<HMUI.ImageView>().FirstOrDefault();
                if (img != null)
                {
                    img.color = new Color32(
                        (byte)PluginConfig.Instance.Colors.PassBgR, 
                        (byte)PluginConfig.Instance.Colors.PassBgG, 
                        (byte)PluginConfig.Instance.Colors.PassBgB, 
                        255
                    );
                }
            }

            if (failedBanner != null)
            {
                var textFail = failedBanner.GetComponentInChildren<HMUI.CurvedTextMeshPro>(true);
                if (textFail != null)
                {
                    textFail.SetText(PluginConfig.Instance.Text.ResultFailText);
                    textFail.color = Color.white;
                    RemoveLocalizeComponents(failedBanner.gameObject);
                }

                var imgFail = failedBanner.GetComponentsInChildren<HMUI.ImageView>().FirstOrDefault();
                if (imgFail != null)
                {
                    imgFail.color = new Color32(
                        (byte)PluginConfig.Instance.Colors.FailBgR, 
                        (byte)PluginConfig.Instance.Colors.FailBgG, 
                        (byte)PluginConfig.Instance.Colors.FailBgB, 
                        255
                    );
                }
            }
        }

        private static void RemoveLocalizeComponents(GameObject obj)
        {
            foreach (var comp in obj.GetComponentsInChildren<Component>(true))
            {
                if (comp != null && comp.GetType().Name.Contains("Localize"))
                {
                    Object.Destroy(comp);
                }
            }
        }

        private static Transform? FindRecursive(Transform parent, string name)
        {
            foreach (Transform child in parent)
            {
                if (child == null) continue;
                if (child.name.ToLower().Contains(name.ToLower())) return child;
                var result = FindRecursive(child, name);
                if (result != null) return result;
            }
            return null;
        }
    }
}
