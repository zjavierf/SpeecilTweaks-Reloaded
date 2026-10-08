using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Components.Settings;
using BeatSaberMarkupLanguage.Parser;
using BeatSaberMarkupLanguage.ViewControllers;
using HMUI;
using SpeecilTweaks.Configuration;
using SpeecilTweaks.Features.QoL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;
using Zenject;

namespace SpeecilTweaks.UI;

[HotReload(RelativePathToLayout = @"./settings.bsml")]
[ViewDefinition("SpeecilTweaks.UI.settings.bsml")]
public class SpeecilSettingsViewController : BSMLAutomaticViewController, INotifyPropertyChanged
{
    public new event PropertyChangedEventHandler? PropertyChanged;
    
    [UIComponent("create-preset-env")]
    private readonly ColorSetting _createPresetEnv = null!;

    [UIParams]
    private BSMLParserParams parserParams = null!;

    // --- EDIT MODAL COMPONENT REFERENCES ---
    [UIComponent("preset-settings-modal")]
    private readonly ModalView _presetSettingsModal = null!;

    [UIComponent("preset-settings-name")]
    private readonly StringSetting _presetSettingsName = null!;

    [UIComponent("preset-settings-saber-left")]
    private readonly ColorSetting _presetSettingsSaberLeft = null!;

    [UIComponent("preset-settings-saber-right")]
    private readonly ColorSetting _presetSettingsSaberRight = null!;

    [UIComponent("preset-settings-env")]
    private readonly ColorSetting _presetSettingsEnv = null!;

    [UIComponent("preset-settings-obstacle")]
    private readonly ColorSetting _presetSettingsObstacle = null!;

    private string _editingPresetOriginalName = string.Empty;

    private void SaveAndFlush()
    {
        Plugin.SaveConfig();
    }

    [UIValue("play-text")]
    public string PlayText
    {
        get => PluginConfig.Instance?.Text?.PlayText ?? "";
        set { if (PluginConfig.Instance?.Text != null) { PluginConfig.Instance.Text.PlayText = value; SaveAndFlush(); } }
    }

    [UIValue("practice-text")]
    public string PracticeText
    {
        get => PluginConfig.Instance?.Text?.PracticeText ?? "";
        set { if (PluginConfig.Instance?.Text != null) { PluginConfig.Instance.Text.PracticeText = value; SaveAndFlush(); } }
    }

    [UIValue("result-text")]
    public string ResultText
    {
        get => PluginConfig.Instance?.Text?.ResultText ?? "";
        set { if (PluginConfig.Instance?.Text != null) { PluginConfig.Instance.Text.ResultText = value; SaveAndFlush(); } }
    }

    [UIValue("result-fail-text")]
    public string ResultFailText
    {
        get => PluginConfig.Instance?.Text?.ResultFailText ?? "";
        set { if (PluginConfig.Instance?.Text != null) { PluginConfig.Instance.Text.ResultFailText = value; SaveAndFlush(); } }
    }
    
    [UIValue("solo-text")]
    public string SoloText
    {
        get => PluginConfig.Instance?.Text?.SoloText ?? "";
        set { if (PluginConfig.Instance?.Text != null) { PluginConfig.Instance.Text.SoloText = value; SaveAndFlush(); } }
    }

    [UIValue("hide-menu-logo")]
    public bool HideMenuLogo
    {
        get => PluginConfig.Instance?.Text?.HideMenuLogo ?? false;
        set
        {
            if (PluginConfig.Instance?.Text != null)
            {
                PluginConfig.Instance.Text.HideMenuLogo = value;
                Features.MenuTweaks.MenuLogoTweaks.ApplyLogoTweaks();
                SaveAndFlush();
            }
        }
    }

    [UIValue("enable-custom-logo")]
    public bool EnableCustomMenuLogoText
    {
        get => PluginConfig.Instance?.Text?.EnableCustomMenuLogoText ?? false;
        set
        {
            if (PluginConfig.Instance?.Text != null)
            {
                PluginConfig.Instance.Text.EnableCustomMenuLogoText = value;
                Features.MenuTweaks.MenuLogoTweaks.ApplyLogoTweaks();
                SaveAndFlush();
            }
        }
    }

    [UIValue("custom-logo-text")]
    public string CustomMenuLogoText
    {
        get => PluginConfig.Instance?.Text?.CustomMenuLogoText ?? "BEAT SABER";
        set
        {
            if (PluginConfig.Instance?.Text != null)
            {
                PluginConfig.Instance.Text.CustomMenuLogoText = value;
                Features.MenuTweaks.MenuLogoTweaks.ApplyLogoTweaks();
                SaveAndFlush();
            }
        }
    }
    
    [UIValue("custom-logo-colour")]
    public Color CustomLogoColour
    {
        get => PluginConfig.Instance?.Colors != null 
            ? new Color32((byte)PluginConfig.Instance.Colors.CustomLogoColorR, (byte)PluginConfig.Instance.Colors.CustomLogoColorG, (byte)PluginConfig.Instance.Colors.CustomLogoColorB, 255)
            : Color.white;
        set
        {
            if (PluginConfig.Instance?.Colors != null)
            {
                PluginConfig.Instance.Colors.CustomLogoColorR = (int)(value.r * 255f);
                PluginConfig.Instance.Colors.CustomLogoColorG = (int)(value.g * 255f);
                PluginConfig.Instance.Colors.CustomLogoColorB = (int)(value.b * 255f);
                Features.MenuTweaks.MenuLogoTweaks.ApplyLogoTweaks();
                SaveAndFlush();
            }
        }
    }
    
    [UIValue("enable-asset-purge")]
    public bool EnableAssetPurge
    {
        get => PluginConfig.Instance?.Performance?.EnableAssetPurge ?? false;
        set { if (PluginConfig.Instance?.Performance != null) { PluginConfig.Instance.Performance.EnableAssetPurge = value; SaveAndFlush(); } }
    }
    
    [UIValue("enable-physics-opt")]
    public bool EnablePhysicsOpt
    {
        get => PluginConfig.Instance?.Performance?.EnablePhysicsOptimization ?? false;
        set
        {
            if (PluginConfig.Instance?.Performance != null)
            {
                PluginConfig.Instance.Performance.EnablePhysicsOptimization = value;
                if (value)
                {
                    SpeecilTweaks.Features.Performance.SpeecilPerformanceManager.ApplyPhysicsOptimization(value);
                }
                else
                {
                    Time.fixedDeltaTime = 0.02f;
                    Plugin.Log?.Info("[PhysicsOptimizer] Physics optimizations disabled, reset fixedDeltaTime to default (0.02).");
                }
                SaveAndFlush();
            }
        }
    }

    [UIValue("enable-texture-opt")]
    public bool EnableTextureOpt
    {
        get => PluginConfig.Instance?.Performance?.EnableTextureOptimization ?? false;
        set
        {
            if (PluginConfig.Instance?.Performance != null)
            {
                PluginConfig.Instance.Performance.EnableTextureOptimization = value;
                QualitySettings.globalTextureMipmapLimit = value ? 1 : 0;
                Plugin.Log?.Info($"[TextureOptimizer] Mipmap limit updated. Enabled: {value}");
                SaveAndFlush();
            }
        }
    }

    [UIValue("enable-high-priority")]
    public bool EnableHighPriority
    {
        get => PluginConfig.Instance?.Performance?.EnableHighPriority ?? false;
        set
        {
            if (PluginConfig.Instance?.Performance != null)
            {
                PluginConfig.Instance.Performance.EnableHighPriority = value;
                try
                {
                    using var currentProcess = System.Diagnostics.Process.GetCurrentProcess();
                    currentProcess.PriorityClass = value ? System.Diagnostics.ProcessPriorityClass.High : System.Diagnostics.ProcessPriorityClass.Normal;
                    Plugin.Log?.Info($"[ProcessPriorityManager] Priority set to: {currentProcess.PriorityClass}");
                }
                catch (Exception ex)
                {
                    Plugin.Log?.Error($"[ProcessPriorityManager] Failed to toggle process priority: {ex.Message}");
                }
                SaveAndFlush();
            }
        }
    }
    
    [UIValue("play-button-colour")]
    public Color PlayButtonColour
    {
        get => PluginConfig.Instance?.Colors != null 
            ? new Color32((byte)PluginConfig.Instance.Colors.PlayButtonR, (byte)PluginConfig.Instance.Colors.PlayButtonG, (byte)PluginConfig.Instance.Colors.PlayButtonB, 255)
            : Color.white;
        set
        {
            if (PluginConfig.Instance?.Colors != null)
            {
                PluginConfig.Instance.Colors.PlayButtonR = (int)(value.r * 255f);
                PluginConfig.Instance.Colors.PlayButtonG = (int)(value.g * 255f);
                PluginConfig.Instance.Colors.PlayButtonB = (int)(value.b * 255f);
                SaveAndFlush();
            }
        }
    }

    [UIValue("practice-button-colour")]
    public Color PracticeButtonColour
    {
        get => PluginConfig.Instance?.Colors != null
            ? new Color32((byte)PluginConfig.Instance.Colors.PracticeButtonR, (byte)PluginConfig.Instance.Colors.PracticeButtonG, (byte)PluginConfig.Instance.Colors.PracticeButtonB, 255)
            : Color.white;
        set
        {
            if (PluginConfig.Instance?.Colors != null)
            {
                PluginConfig.Instance.Colors.PracticeButtonR = (int)(value.r * 255f);
                PluginConfig.Instance.Colors.PracticeButtonG = (int)(value.g * 255f);
                PluginConfig.Instance.Colors.PracticeButtonB = (int)(value.b * 255f);
                SaveAndFlush();
            }
        }
    }
    
    [UIValue("solo-button-colour")]
    public Color SoloButtonColour
    {
        get => PluginConfig.Instance?.Colors != null
            ? new Color32((byte)PluginConfig.Instance.Colors.SoloButtonR, (byte)PluginConfig.Instance.Colors.SoloButtonG, (byte)PluginConfig.Instance.Colors.SoloButtonB, 255)
            : Color.white;
        set
        {
            if (PluginConfig.Instance?.Colors != null)
            {
                PluginConfig.Instance.Colors.SoloButtonR = (int)(value.r * 255f);
                PluginConfig.Instance.Colors.SoloButtonG = (int)(value.g * 255f);
                PluginConfig.Instance.Colors.SoloButtonB = (int)(value.b * 255f);
                SaveAndFlush();
            }
        }
    }

    [UIValue("pass-bg-colour")]
    public Color PassBgColour
    {
        get => PluginConfig.Instance?.Colors != null
            ? new Color32((byte)PluginConfig.Instance.Colors.PassBgR, (byte)PluginConfig.Instance.Colors.PassBgG, (byte)PluginConfig.Instance.Colors.PassBgB, 255)
            : Color.white;
        set
        {
            if (PluginConfig.Instance?.Colors != null)
            {
                PluginConfig.Instance.Colors.PassBgR = (int)(value.r * 255f);
                PluginConfig.Instance.Colors.PassBgG = (int)(value.g * 255f);
                PluginConfig.Instance.Colors.PassBgB = (int)(value.b * 255f);
                SaveAndFlush();
            }
        }
    }

    [UIValue("fail-bg-colour")]
    public Color FailBgColour
    {
        get => PluginConfig.Instance?.Colors != null
            ? new Color32((byte)PluginConfig.Instance.Colors.FailBgR, (byte)PluginConfig.Instance.Colors.FailBgG, (byte)PluginConfig.Instance.Colors.FailBgB, 255)
            : Color.white;
        set
        {
            if (PluginConfig.Instance?.Colors != null)
            {
                PluginConfig.Instance.Colors.FailBgR = (int)(value.r * 255f);
                PluginConfig.Instance.Colors.FailBgG = (int)(value.g * 255f);
                PluginConfig.Instance.Colors.FailBgB = (int)(value.b * 255f);
                SaveAndFlush();
            }
        }
    }

    // --- PREFERRED ENV SETTINGS ---
    [UIValue("use-preferred-env-color")]
    public bool UsePreferredEnvColor
    {
        get => PluginConfig.Instance?.QoL?.UsePreferredEnvColor ?? false;
        set
        {
            if (PluginConfig.Instance?.QoL != null)
            {
                PluginConfig.Instance.QoL.UsePreferredEnvColor = value;
                SaveAndFlush();
                MoreColorPresetsPatch.RefreshActiveSettingsPanel();
            }
        }
    }

    [UIValue("preferred-env-color")]
    public Color PreferredEnvColor
    {
        get => PluginConfig.Instance?.QoL != null && ColorUtility.TryParseHtmlString(PluginConfig.Instance.QoL.PreferredEnvHex, out var col) ? col : Color.red;
        set
        {
            if (PluginConfig.Instance?.QoL != null)
            {
                PluginConfig.Instance.QoL.PreferredEnvHex = "#" + ColorUtility.ToHtmlStringRGB(value);
                SaveAndFlush();
                MoreColorPresetsPatch.RefreshActiveSettingsPanel();
            }
        }
    }

    // --- CREATE MODAL STATE ---
    [UIValue("new-preset-name")]
    public string NewPresetName { get; set; } = "My Custom Scheme";

    [UIValue("new-saber-left")]
    public Color NewSaberLeft { get; set; } = Color.red;

    [UIValue("new-saber-right")]
    public Color NewSaberRight { get; set; } = Color.blue;

    [UIValue("new-env")]
    public Color NewEnv { get; set; } = Color.red;

    [UIValue("new-obstacle")]
    public Color NewObstacle { get; set; } = Color.red;

    [UIValue("preset-list-data")]
    public List<PresetCellData> PresetListData { get; set; } = new List<PresetCellData>();
    
    [UIComponent("presets-list")]
    public CustomCellListTableData PresetsList = null!;

    public class PresetCellData
    {
        [UIValue("preset-title")]
        public string Title { get; set; }

        public Action<PresetCellData>? OnEditSelected;
        public Action<PresetCellData>? OnDeleteSelected;

        public PresetCellData(string title)
        {
            Title = title;
        }

        [UIAction("edit-clicked")]
        private void EditClicked()
        {
            OnEditSelected?.Invoke(this);
        }

        [UIAction("delete-clicked")]
        private void DeleteClicked()
        {
            OnDeleteSelected?.Invoke(this);
        }
    }

    private SpeecilFlowCoordinator? _flowCoordinator;

    [Inject]
    public void Construct(SpeecilFlowCoordinator flowCoordinator)
    {
        _flowCoordinator = flowCoordinator;
    }

    [UIAction("#post-parse")]
    void PostParse()
    {
        if (PresetListData == null)
        {
            PresetListData = new List<PresetCellData>();
        }
        RefreshPresetsList();
    }

    public void OpenEditor(PluginConfig.CustomPresetData preset)
    {
        if (preset == null) return;

        _editingPresetOriginalName = preset.Name;

        if (_presetSettingsName != null)
            _presetSettingsName.Text = preset.Name;

        if (ColorUtility.TryParseHtmlString(preset.SaberLeftHex, out var sLeft) && _presetSettingsSaberLeft != null)
            _presetSettingsSaberLeft.CurrentColor = sLeft;

        if (ColorUtility.TryParseHtmlString(preset.SaberRightHex, out var sRight) && _presetSettingsSaberRight != null)
            _presetSettingsSaberRight.CurrentColor = sRight;

        if (ColorUtility.TryParseHtmlString(preset.EnvHex, out var env) && _presetSettingsEnv != null)
            _presetSettingsEnv.CurrentColor = env;

        if (ColorUtility.TryParseHtmlString(preset.ObstacleHex, out var obs) && _presetSettingsObstacle != null)
            _presetSettingsObstacle.CurrentColor = obs;

        _presetSettingsModal?.Show(true);
    }

    [UIAction("open-create-preset-modal")]
    void OpenCreatePresetModal()
    {
        NewPresetName = "My Custom Scheme";
        NewSaberLeft = Color.red;
        NewSaberRight = Color.blue;
        NewObstacle = Color.red;

        if (PluginConfig.Instance?.QoL?.UsePreferredEnvColor == true && 
            !string.IsNullOrEmpty(PluginConfig.Instance.QoL.PreferredEnvHex) && 
            ColorUtility.TryParseHtmlString(PluginConfig.Instance.QoL.PreferredEnvHex, out var parsedColor))
        {
            NewEnv = parsedColor;
        }
        else
        {
            NewEnv = Color.red;
        }

        // Explicitly update the BSML color-setting component's visual state
        if (_createPresetEnv != null)
        {
            _createPresetEnv.CurrentColor = NewEnv;
        }

        NotifyPropertyChanged(nameof(NewPresetName));
        NotifyPropertyChanged(nameof(NewSaberLeft));
        NotifyPropertyChanged(nameof(NewSaberRight));
        NotifyPropertyChanged(nameof(NewEnv));
        NotifyPropertyChanged(nameof(NewObstacle));

        parserParams?.EmitEvent("show-create-modal");
    }

    [UIAction("cancel-create-preset")]
    void CancelCreatePreset()
    {
        parserParams?.EmitEvent("hide-create-modal");
    }

    [UIAction("save-new-preset")]
    void SaveNewPreset()
    {
        if (string.IsNullOrWhiteSpace(NewPresetName)) return;

        var presets = PluginConfig.Instance?.QoL?.Presets;
        if (presets == null || presets.Any(p => p.Name == NewPresetName)) return;

        var newPreset = new PluginConfig.CustomPresetData
        {
            Name = NewPresetName,
            SaberLeftHex = "#" + ColorUtility.ToHtmlStringRGB(NewSaberLeft),
            SaberRightHex = "#" + ColorUtility.ToHtmlStringRGB(NewSaberRight),
            EnvHex = "#" + ColorUtility.ToHtmlStringRGB(NewEnv),
            ObstacleHex = "#" + ColorUtility.ToHtmlStringRGB(NewObstacle)
        };

        presets.Add(newPreset);
        if (PluginConfig.Instance?.QoL != null)
        {
            PluginConfig.Instance.QoL.SelectedPresetId = $"Speecil_{newPreset.Name}";
        }
        
        SaveAndFlush();

        MoreColorPresetsPatch.RefreshActiveSettingsPanel();

        parserParams?.EmitEvent("hide-create-modal");
        RefreshPresetsList();
        NotifyPropertyChanged(nameof(PresetListData));
    }

    [UIAction("save-edited-preset")]
    void SaveEditedPreset()
    {
        var presets = PluginConfig.Instance?.QoL?.Presets;
        var preset = presets?.Find(p => p.Name == _editingPresetOriginalName);

        if (preset != null)
        {
            string newName = _presetSettingsName != null ? _presetSettingsName.Text : _editingPresetOriginalName;
            
            if (newName != _editingPresetOriginalName)
            {
                var playerDataModel = Resources.FindObjectsOfTypeAll<PlayerDataModel>().FirstOrDefault();
                var colorSettings = playerDataModel?.playerData?.colorSchemesSettings;
                if (colorSettings != null)
                {
                    MoreColorPresetsPatch.RemoveCustomPreset(colorSettings, _editingPresetOriginalName);
                }
            }

            preset.Name = newName;
            
            if (_presetSettingsSaberLeft != null)
                preset.SaberLeftHex = "#" + ColorUtility.ToHtmlStringRGB(_presetSettingsSaberLeft.CurrentColor);
            if (_presetSettingsSaberRight != null)
                preset.SaberRightHex = "#" + ColorUtility.ToHtmlStringRGB(_presetSettingsSaberRight.CurrentColor);
            if (_presetSettingsEnv != null)
                preset.EnvHex = "#" + ColorUtility.ToHtmlStringRGB(_presetSettingsEnv.CurrentColor);
            if (_presetSettingsObstacle != null)
                preset.ObstacleHex = "#" + ColorUtility.ToHtmlStringRGB(_presetSettingsObstacle.CurrentColor);

            if (PluginConfig.Instance?.QoL != null)
            {
                PluginConfig.Instance.QoL.SelectedPresetId = $"Speecil_{preset.Name}";
            }
            
            SaveAndFlush();

            MoreColorPresetsPatch.RefreshActiveSettingsPanel();
            
            _editingPresetOriginalName = preset.Name;

            _presetSettingsModal?.Hide(true);
            RefreshPresetsList();
            NotifyPropertyChanged(nameof(PresetListData));
        }
    }

    [UIAction("cancel-edit-preset")]
    void CancelEditPreset()
    {
        _presetSettingsModal?.Hide(true);
    }

    public void RefreshPresetsList()
    {
        PresetListData.Clear();

        var presets = PluginConfig.Instance?.QoL?.Presets;
        if (presets == null) return;

        foreach (var preset in presets)
        {
            if (preset == null || string.IsNullOrEmpty(preset.Name)) continue;

            var cellData = new PresetCellData(preset.Name);

            cellData.OnEditSelected += (data) => 
            {
                var targetPreset = presets.Find(p => p.Name == data.Title);
                if (targetPreset != null) OpenEditor(targetPreset);
            };

            cellData.OnDeleteSelected += (data) => 
            {
                DeletePresetInternal(data);
            };

            PresetListData.Add(cellData);
        }

        PresetsList?.TableView?.ReloadData();
    }

    private void DeletePresetInternal(PresetCellData cellData)
    {
        if (cellData == null) return;

        var presets = PluginConfig.Instance?.QoL?.Presets;
        if (presets == null) return;

        var presetToRemove = presets.Find(p => p.Name == cellData.Title);
        if (presetToRemove != null)
        {
            presets.Remove(presetToRemove);

            var playerDataModel = Resources.FindObjectsOfTypeAll<PlayerDataModel>().FirstOrDefault();
            var colorSettings = playerDataModel?.playerData?.colorSchemesSettings;
            if (colorSettings != null)
            {
                Features.QoL.MoreColorPresetsPatch.RemoveCustomPreset(colorSettings, cellData.Title);
            }

            if (PluginConfig.Instance?.QoL != null && PluginConfig.Instance.QoL.SelectedPresetId == cellData.Title)
            {
                var remainingPreset = presets.FirstOrDefault();
                PluginConfig.Instance.QoL.SelectedPresetId = remainingPreset?.Name ?? "";
            }
            
            SaveAndFlush();
            
            RefreshPresetsList();
            NotifyPropertyChanged(nameof(PresetListData));
        }
    }

    private new void NotifyPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    
    [UIAction("BackButton")]
    void OnBackPressed() => _flowCoordinator?.DismissSelf();
    
    [UIAction("Github")]
    void OpenGitHub() => System.Diagnostics.Process.Start("https://github.com/zjavierf/SpeecilTweaks-Reloaded");
    
    [UIAction("FCSplashRepo")]
    void OpenFcSplashRepo() => System.Diagnostics.Process.Start("https://github.com/zjavierf/BS_FCSplash");
    
    [UIAction("WeatherModRepo")]
    void OpenWeatherModRepo() => System.Diagnostics.Process.Start("https://github.com/zjavierf/Beat-Saber-WeatherReloaded");
    
    [UIAction("MapInfoCounterRepo")]
    void OpenMapInfoCounterRepo() => System.Diagnostics.Process.Start("https://github.com/zjavierf/MapInfoCounter");
}
