using HarmonyLib;
using SpeecilTweaks.Features.Performance;

namespace SpeecilTweaks.Features.MenuTweaks
{
    [HarmonyPatch(typeof(StandardLevelGameplayManager), "OnDestroy")]
    public class LevelEndPatch
    {
        private static void Prefix()
        {
            SpeecilPerformanceManager.PurgeUnusedAssets();
        }
    }
}
