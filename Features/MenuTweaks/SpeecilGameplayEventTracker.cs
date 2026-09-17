using HarmonyLib;
using SpeecilTweaks.Features.Performance;

namespace SpeecilTweaks.Features.MenuTweaks
{
    [HarmonyPatch(typeof(StandardLevelGameplayManager), "Start")]
    public class LevelStartPatch
    {
        private static void Postfix()
        {
            SpeecilPerformanceManager.SetGameStarted(true);
        }
    }

    [HarmonyPatch(typeof(StandardLevelGameplayManager), "OnDestroy")]
    public class LevelEndPatch
    {
        private static void Prefix()
        {
            SpeecilPerformanceManager.SetGameStarted(false);
            SpeecilPerformanceManager.PurgeUnusedAssets();
        }
    }
}