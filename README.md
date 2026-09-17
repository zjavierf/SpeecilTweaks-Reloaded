# Speecil Tweaks - Reloaded 1.40.8 (PC)

An expanded, feature-rich set of tweaks and customisations for **Beat Saber**.

## Features

### 🎨 Menu Tweaks
* **Main Menu Customisation**:
  * Customize or rename the Main Menu **Solo**, **Play**, and **Practice** button text.
  * Adjust custom text colors for buttons.
  * Hide or reveal the main menu sky logo.
  * Enable and customize **Custom Logo Text** with custom colors.
    
* **Results Screen Tweaks**:
  * Change pass and fail result text.
  * Customize pass and fail banner background colors.

### ⚡ Performance Optimizations
* **GC Control**: Temporarily disables automatic garbage collection during songs to prevent micro-stutters, with safety cleanup thresholds (>250MB).
* **Fast Obstacle Optimization**: Dynamic head-wall collision detector to optimize performance on wall-heavy map streams.
* **Automatic Asset Purge**: Automatically triggers garbage collection and unloads unused assets during menu loading transitions.
* **Physics Optimization**: Adjusts `fixedDeltaTime` dynamically based on your display refresh rate (FPFC) or headset refresh rate.
* **Texture Mipmap Optimization**: Forces texture mipmap limits to reduce VRAM consumption.
* **Audio Buffer Size Optimization**: Lowers the DSP buffer size (512) to minimize audio-to-motion latency.
* **High Process Priority**: Option to force Beat Saber's CPU process priority to High.

### 🛠️ Quality Of Life & Color Presets
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
