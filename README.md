# Speecil Tweaks - Reloaded 1.40.8 (PC)

An expanded set of tweaks and customisations for **Beat Saber**.

---
##  Screenshots

<details>
<summary><b>Click to expand and view the gallery</b></summary>

### Main Menu & Customization

**Custom Logo & Main Menu**
<img width="800" alt="mainLogo" src="https://github.com/user-attachments/assets/e4b18143-3fd7-47c3-8029-9cc67ae9a907" />

**Solo Text & Buttons**
<img width="800" alt="soloText" src="https://github.com/user-attachments/assets/3ac7eae0-a98b-40c0-91b9-5b4669a94cef" />

**Main Menu Tab 1**
<img width="800" alt="mainMenuTab1" src="https://github.com/user-attachments/assets/515ce7b4-3214-4301-9a81-cdb4d5f4f7fc" />

**Main Menu Tab 2**
<img width="800" alt="mainMenuTab2" src="https://github.com/user-attachments/assets/1987fdcc-b00e-4f72-a551-81b6bf0d24ab" />

**Play & Practice Buttons**
<img width="800" alt="playPracButtons" src="https://github.com/user-attachments/assets/518290e5-ce3f-46c7-b797-604fa1fcfa86" />

**Song List Tab**
<img width="800" alt="songListTab" src="https://github.com/user-attachments/assets/a115e7d5-04bb-4a77-b588-dc86cae3180a" />

---

### Color Preset Manager

**Color Preset Overview**
<img width="800" alt="colorPresetTab" src="https://github.com/user-attachments/assets/b46f087d-418a-4082-814a-5cf5c9db2623" />

**Preset Dropdown**
<img width="800" alt="colorPresetDropdown" src="https://github.com/user-attachments/assets/02bb3d48-9b07-48d6-a262-2b59839fcd3e" />

**Preset Editor**
<img width="800" alt="colorPresetEdit" src="https://github.com/user-attachments/assets/650cd01b-b466-48c6-9ec0-965c32135330" />

**Custom Colors Active**
<img width="800" alt="colorPreset" src="https://github.com/user-attachments/assets/10d2cbf0-2b2e-4e04-8054-1d349f811078" />

---

### Performance & Settings Tabs

**Performance Tab 1**
<img width="800" alt="performanceTab" src="https://github.com/user-attachments/assets/684a67e7-319c-49c9-8a39-c2f387e65cc8" />

**Performance Tab 2**
<img width="800" alt="performanceTab2" src="https://github.com/user-attachments/assets/75115caa-d529-421a-9384-82ef2caf85f0" />

**Results Screen Tab**
<img width="800" alt="resultsScreenTab" src="https://github.com/user-attachments/assets/0c8b1d97-f055-4105-8266-02640a0d4efd" />

**Other Mods Tab**
<img width="800" alt="otherModsTab" src="https://github.com/user-attachments/assets/a03039d0-afe8-4b66-a81a-ee97d4a5c822" />

</details>

---
## Features

### Menu Tweaks
* **Main Menu Customisation**:
  * Customize or rename the Main Menu **Solo**, **Play**, and **Practice** button text.
  * Adjust custom text colors for buttons.
  * Hide or reveal the main menu sky logo.
  * Enable and customize **Custom Logo Text** with custom colors.
    
* **Results Screen Tweaks**:
  * Change pass and fail result text.
  * Customize pass and fail banner background colors.

### Performance Optimizations
* **GC Control**: Temporarily disables automatic garbage collection during songs to prevent micro-stutters, with safety cleanup thresholds (>250MB).
* **Fast Obstacle Optimization**: Dynamic head-wall collision detector to optimize performance on wall-heavy map streams.
* **Automatic Asset Purge**: Automatically triggers garbage collection and unloads unused assets during menu loading transitions.
* **Physics Optimization**: Adjusts `fixedDeltaTime` dynamically based on your display refresh rate (FPFC) or headset refresh rate.
* **Texture Mipmap Optimization**: Forces texture mipmap limits to reduce VRAM consumption.
* **Audio Buffer Size Optimization**: Lowers the DSP buffer size (512) to minimize audio-to-motion latency.
* **High Process Priority**: Option to force Beat Saber's CPU process priority to High.

### Quality Of Life & Color Presets
* **Custom Color Preset Manager**: 
  * Create, edit, save, and delete custom color presets directly inside the in-game UI.
  * Fully configure custom hex values for **Left Saber**, **Right Saber**, **Environment Left/Right**, and **Obstacles**.
* **In-Game Settings**: All settings can be configured in the mod menu in-game.

## In-Game Settings UI
Press the **Speecil Tweaks** button in the main menu to open the fully interactive settings menu, categorized into:
1. **Menu Tweaks** (Main Menu, Song List Screen, Results Screen)
2. **Gameplay / Performance** (All optimization toggles)
3. **Quality Of Life** (Custom Color Presets manager and modal editor)
4. **Other Mods** (Quick links to my other mods [*FCSplash*](https://github.com/unknownjwly/BS_FCSplash) and [*WeatherModReloaded*](https://github.com/zjavierf/Beat-Saber-WeatherReloaded))

## Credits & Links
* Built and maintained by **zjavierf**.
* Forked from [Speecil](https://github.com/speecil/SpeecilTweaks-PC) 
* Features inspired from other mods
