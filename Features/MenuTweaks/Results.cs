using System.Linq;
using HarmonyLib;
using SpeecilTweaks.Configuration;
using UnityEngine;

namespace SpeecilTweaks.Features.MenuTweaks
{
    [HarmonyPatch(typeof(ResultsViewController), "SetDataToUI")]
    public static class ResultsViewControllerSetDataToUI
    {
        static void Postfix(ref GameObject ____clearedBannerGo, ref GameObject ____failedBannerGo)
        {
            if (____clearedBannerGo != null)
            {
                ____clearedBannerGo.GetComponentInChildren<HMUI.CurvedTextMeshPro>()?.SetText(PluginConfig.Instance.Text.ResultText);
                var img = ____clearedBannerGo.GetComponentsInChildren<HMUI.ImageView>().FirstOrDefault();
                if (img != null) img.color = new Color32((byte)PluginConfig.Instance.Colors.PassBgR, (byte)PluginConfig.Instance.Colors.PassBgG, (byte)PluginConfig.Instance.Colors.PassBgB, 255);
            }

            if (____failedBannerGo != null)
            {
                ____failedBannerGo.GetComponentInChildren<HMUI.CurvedTextMeshPro>()?.SetText(PluginConfig.Instance.Text.ResultFailText);
                var imgFail = ____failedBannerGo.GetComponentsInChildren<HMUI.ImageView>().FirstOrDefault();
                if (imgFail != null) imgFail.color = new Color32((byte)PluginConfig.Instance.Colors.FailBgR, (byte)PluginConfig.Instance.Colors.FailBgG, (byte)PluginConfig.Instance.Colors.FailBgB, 255);
                
                var textFail = ____failedBannerGo.GetComponentsInChildren<HMUI.CurvedTextMeshPro>().FirstOrDefault();
                if (textFail != null) textFail.color = Color.white;
            }
        }
    }
}