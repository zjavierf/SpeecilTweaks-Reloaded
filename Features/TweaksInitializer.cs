using Zenject;
using SpeecilTweaks.Configuration;
using SpeecilTweaks.Features.Performance;
using SpeecilTweaks.Features.QoL;

namespace SpeecilTweaks.Features
{
    public class TweaksInitializer : IInitializable
    {
        [Inject] private readonly PlayerDataModel _playerDataModel = null!;

        public void Initialize()
        {
            Plugin.Log?.Info("[SpeecilTweaks] Main Menu loaded. Initializing active tweaks...");

            var colorSettings = _playerDataModel?.playerData?.colorSchemesSettings;

            if (colorSettings != null)
            {
                MoreColorPresetsPatch.InjectCustomPresets(colorSettings);

                var selectedName = PluginConfig.Instance?.QoL?.SelectedPresetName;
                var customPresets = PluginConfig.Instance?.QoL?.Presets;

                if (!string.IsNullOrEmpty(selectedName))
                {
                    string targetId = $"Speecil_{selectedName}";
                    colorSettings.selectedColorSchemeId = targetId;
                }
                else if (customPresets != null && customPresets.Count > 0)
                {
                    string fallbackId = $"Speecil_{customPresets[0].Name}";
                    colorSettings.selectedColorSchemeId = fallbackId;
                }
            }
            else
            {
                Plugin.Log?.Warn("[SpeecilTweaks] Could not find ColorSchemesSettings on PlayerDataModel during initialization.");
            }

            var config = PluginConfig.Instance?.Performance;
            if (config == null)
            {
                Plugin.Log?.Warn("[SpeecilTweaks] PluginConfig.Instance.Performance is null. Skipping performance tweaks.");
                return;
            }
            
            SpeecilPerformanceManager.ApplyAllOptimizations();
            
            if (config.EnableAssetPurge || config.EnableGarbageCollectionControl)
            {
                SpeecilPerformanceManager.Init();
            }
        }
    }
}