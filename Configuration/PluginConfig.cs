using System.Collections.Generic;
using IPA.Config.Stores;
using IPA.Config.Stores.Attributes;
using IPA.Config.Stores.Converters;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo(GeneratedStore.AssemblyVisibilityTarget)]
namespace SpeecilTweaks.Configuration
{
    public class PluginConfig
    {
        public static PluginConfig Instance { get; set; } = null!;
        public virtual TextSettings Text { get; set; } = new();
        public virtual ColorSettings Colors { get; set; } = new();
        public virtual PerformanceSettings Performance { get; set; } = new();
        public virtual QualityOfLifeSettings QoL { get; set; } = new();

        public class TextSettings
        {
            public virtual string SoloText { get; set; } = "Solo";
            public virtual string PlayText { get; set; } = "Speecil!";
            public virtual string PracticeText { get; set; } = "Git Gud";
            public virtual string ResultText { get; set; } = "Well Done!";
            public virtual string ResultFailText { get; set; } = "You Suck!";
            public virtual bool HideMenuLogo { get; set; } = false;
            public virtual string CustomMenuLogoText { get; set; } = "BEAT SABER";
            public virtual bool EnableCustomMenuLogoText { get; set; } = false;
            
        }

        public class ColorSettings
        {
            public virtual int SoloButtonR { get; set; } = 0;
            public virtual int SoloButtonG { get; set; } = 255;
            public virtual int SoloButtonB { get; set; } = 255;

            public virtual int PlayButtonR { get; set; } = 255;
            public virtual int PlayButtonG { get; set; } = 255;
            public virtual int PlayButtonB { get; set; } = 255;

            public virtual int PracticeButtonR { get; set; } = 255;
            public virtual int PracticeButtonG { get; set; } = 165;
            public virtual int PracticeButtonB { get; set; } = 0;

            public virtual int PassBgR { get; set; } = 0;
            public virtual int PassBgG { get; set; } = 255;
            public virtual int PassBgB { get; set; } = 0;

            public virtual int FailBgR { get; set; } = 255;
            public virtual int FailBgG { get; set; } = 0;
            public virtual int FailBgB { get; set; } = 0;
            
            public virtual int CustomLogoColorR { get; set; } = 255;
            public virtual int CustomLogoColorG { get; set; } = 255;
            public virtual int CustomLogoColorB { get; set; } = 255;
        }

        public class PerformanceSettings
        {
            public virtual bool EnableGarbageCollectionControl { get; set; } = false;
            public virtual bool EnableObstacleOptimization { get; set; } = false;
            public virtual bool EnableAssetPurge { get; set; } = false;
            public virtual bool EnablePhysicsOptimization { get; set; } = false;
            public virtual bool EnableTextureOptimization { get; set; } = false;
            public virtual bool EnableAudioOptimization { get; set; } = false;
            public virtual bool EnableHighPriority { get; set; } = false;
        }

        public class QualityOfLifeSettings
        {
            public virtual string SelectedPresetName { get; set; } = "";

            [UseConverter(typeof(ListConverter<CustomPresetData>))]
            [NonNullable]
            public virtual List<CustomPresetData> Presets { get; set; } = new();
        }

        public class CustomPresetData
        {
            public virtual string Id { get; set; } = System.Guid.NewGuid().ToString();
            public virtual string Name { get; set; } = "Custom Scheme";
            public virtual string SaberLeftHex { get; set; } = "#FF0000";
            public virtual string SaberRightHex { get; set; } = "#0000FF";
            public virtual string EnvLeftHex { get; set; } = "#FF0000";
            public virtual string EnvRightHex { get; set; } = "#0000FF";
            public virtual string ObstacleHex { get; set; } = "#FF0000";
        }

        public virtual void CopyFrom(PluginConfig other)
        {
            this.Text = other.Text;
            this.Colors = other.Colors;
            this.Performance = other.Performance;
            this.QoL = other.QoL;
        }
    }
}