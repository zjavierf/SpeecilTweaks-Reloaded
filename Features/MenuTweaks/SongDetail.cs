using HarmonyLib;
using SpeecilTweaks.Configuration;
using UnityEngine;
using UnityEngine.UI;

namespace SpeecilTweaks.Features.MenuTweaks
{
    internal static class SongDetailsHelper
    {
        public static void ApplyTweaks(Button actionButton, Button practiceButton)
        {
            if (actionButton != null)
            {
                var textComp = actionButton.GetComponentInChildren<HMUI.CurvedTextMeshPro>(true);
                if (textComp != null)
                {
                    textComp.SetText(PluginConfig.Instance.Text.PlayText);
                    var targetColor = new Color32(
                        (byte)PluginConfig.Instance.Colors.PlayButtonR, 
                        (byte)PluginConfig.Instance.Colors.PlayButtonG, 
                        (byte)PluginConfig.Instance.Colors.PlayButtonB, 
                        255
                    );
                    textComp.color = targetColor;

                    var watcher = actionButton.GetComponent<ButtonColorEnforcer>();
                    if (watcher == null) watcher = actionButton.gameObject.AddComponent<ButtonColorEnforcer>();
                    watcher.TargetColor = targetColor;
                    watcher.TargetText = textComp;
                    watcher.enabled = true;
                }

                foreach (var comp in actionButton.GetComponentsInChildren<Component>(true))
                {
                    string typeName = comp.GetType().Name;
                    if (typeName.Contains("TextTransition") || typeName.Contains("ColorTransition"))
                    {
                        Object.Destroy(comp);
                    }
                }
            }

            if (practiceButton != null)
            {
                var textComp = practiceButton.GetComponentInChildren<HMUI.CurvedTextMeshPro>(true);
                if (textComp != null)
                {
                    textComp.SetText(PluginConfig.Instance.Text.PracticeText);
                    var targetColor = new Color32(
                        (byte)PluginConfig.Instance.Colors.PracticeButtonR, 
                        (byte)PluginConfig.Instance.Colors.PracticeButtonG, 
                        (byte)PluginConfig.Instance.Colors.PracticeButtonB, 
                        255
                    );
                    textComp.color = targetColor;

                    var watcher = practiceButton.GetComponent<ButtonColorEnforcer>();
                    if (watcher == null) watcher = practiceButton.gameObject.AddComponent<ButtonColorEnforcer>();
                    watcher.TargetColor = targetColor;
                    watcher.TargetText = textComp;
                    watcher.enabled = true;
                }

                foreach (var comp in practiceButton.GetComponentsInChildren<Component>(true))
                {
                    string typeName = comp.GetType().Name;
                    if (typeName.Contains("TextTransition") || typeName.Contains("ColorTransition"))
                    {
                        Object.Destroy(comp);
                    }
                }
            }
        }
    }

    [HarmonyPatch(typeof(StandardLevelDetailView), "RefreshContent")]
    public static class StandardLevelDetailViewRefreshContent
    {
        static void Postfix(ref Button ____actionButton, ref Button ____practiceButton)
        {
            SongDetailsHelper.ApplyTweaks(____actionButton, ____practiceButton);
        }
    }

    [HarmonyPatch(typeof(StandardLevelDetailViewController), "DidActivate")]
    public static class StandardLevelDetailViewControllerDidActivate
    {
        static void Postfix(StandardLevelDetailViewController __instance)
        {
            var detailView = Traverse.Create(__instance).Field("_standardLevelDetailView").GetValue<StandardLevelDetailView>();
            if (detailView != null)
            {
                var actionButton = Traverse.Create(detailView).Field("_actionButton").GetValue<Button>();
                var practiceButton = Traverse.Create(detailView).Field("_practiceButton").GetValue<Button>();
                SongDetailsHelper.ApplyTweaks(actionButton, practiceButton);
            }
        }
    }

    [HarmonyPatch(typeof(StandardLevelDetailViewController), "DidDeactivate")]
    public static class StandardLevelDetailViewControllerDidDeactivate
    {
        static void Prefix(StandardLevelDetailViewController __instance)
        {
            var detailView = Traverse.Create(__instance).Field("_standardLevelDetailView").GetValue<StandardLevelDetailView>();
            if (detailView != null)
            {
                var actionButton = Traverse.Create(detailView).Field("_actionButton").GetValue<Button>();
                var practiceButton = Traverse.Create(detailView).Field("_practiceButton").GetValue<Button>();

                if (actionButton != null)
                {
                    var watcher = actionButton.GetComponent<ButtonColorEnforcer>();
                    if (watcher != null) watcher.enabled = false;
                }

                if (practiceButton != null)
                {
                    var watcher = practiceButton.GetComponent<ButtonColorEnforcer>();
                    if (watcher != null) watcher.enabled = false;
                }
            }
        }
    }

    public class ButtonColorEnforcer : MonoBehaviour
    {
        public Color TargetColor;
        public HMUI.CurvedTextMeshPro? TargetText;

        private void Update()
        {
            if (TargetText != null && TargetText.color != TargetColor)
            {
                TargetText.color = TargetColor;
            }
        }
    }
}