using System;
using System.IO;
using IPA;
using IPA.Logging;
using HarmonyLib;
using IPA.Config.Stores;
using Newtonsoft.Json.Linq;
using SiraUtil.Zenject;
using SpeecilTweaks.Configuration;
using SpeecilTweaks.UI;

namespace SpeecilTweaks
{
    [Plugin(RuntimeOptions.DynamicInit)]
    public class Plugin
    {
        public static Plugin Instance { get; private set; } = null!;
        public static Logger Log { get; private set; } = null!;
        private Harmony? _harmony;

        [Init]
        public void Init(Logger logger, Zenjector zenjector, IPA.Config.Config conf)
        {
            Instance = this;
            Log = logger;

            // Run sanitization immediately on construct before Zenject or BSAssetLib touch PlayerData.dat
            SanitizePlayerDataOnDisk();

            PluginConfig.Instance = conf.Generated<PluginConfig>();

            zenjector.UseLogger(logger);
            zenjector.Install<SpeecilMenuInstaller>(Location.Menu);

            _harmony = new Harmony("Speecil.BeatSaber.SpeecilTweaks");
        }

        [OnEnable]
        public void OnEnable()
        {
            _harmony?.PatchAll();
        }

        [OnDisable]
        public void OnDisable()
        {
            _harmony?.UnpatchSelf();
        }

        private void SanitizePlayerDataOnDisk()
        {
            try
            {
                string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                var parentDir = Directory.GetParent(localAppData);
        
                if (parentDir == null) return;

                string path = Path.Combine(
                    parentDir.FullName, "LocalLow",
                    "Hyperbolic Magnetism", "Beat Saber", "PlayerData.dat"
                );

                if (!File.Exists(path)) return;

                string json = File.ReadAllText(path);
                var token = JToken.Parse(json);
                bool modified = false;

                var colorSchemeToken = token.SelectToken("playerAllSettings.colorSchemesSettings.selectedColorSchemeId") 
                                       ?? token.SelectToken("colorSchemesSettings.selectedColorSchemeId")
                                       ?? token.SelectToken("selectedColorSchemeId");

                if (colorSchemeToken != null)
                {
                    string? value = colorSchemeToken.Value<string>();
                    if (value != null && !string.IsNullOrEmpty(value) && value.StartsWith("Speecil_"))
                    {
                        colorSchemeToken.Replace("User3");
                        modified = true;
                        Log?.Info($"[Sanitizer] Replaced invalid/early color scheme ID '{value}' with 'User3'.");
                    }
                }

                if (modified)
                {
                    string tempPath = path + ".tmp";
                    File.WriteAllText(tempPath, token.ToString(Newtonsoft.Json.Formatting.None));
            
                    if (File.Exists(path))
                    {
                        File.Delete(path);
                    }
                    File.Move(tempPath, path);

                    Log?.Info("[Sanitizer] Successfully sanitized PlayerData.dat atomically on early startup.");
                }
            }
            catch (Exception ex)
            {
                Log?.Error($"[Sanitizer] Failed to sanitize PlayerData.dat: {ex.Message}");
            }
        }
    }
}