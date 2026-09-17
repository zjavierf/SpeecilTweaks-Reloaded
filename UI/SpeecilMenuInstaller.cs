using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.MenuButtons;
using SpeecilTweaks.Features;
using Zenject;

namespace SpeecilTweaks.UI;

public class SpeecilMenuInstaller : Installer
{
    public override void InstallBindings()
    {
        Container.BindInterfacesTo<TweaksInitializer>().AsSingle();
        Container.BindInterfacesAndSelfTo<SpeecilMenuManager>().AsSingle();
        Container.BindInterfacesAndSelfTo<SpeecilSettingsViewController>().FromNewComponentAsViewController().AsSingle();
        Container.Bind<SpeecilFlowCoordinator>().FromNewComponentOnNewGameObject().AsSingle();
    }
}

public class SpeecilMenuManager : IInitializable, System.IDisposable
{
    [Inject] private readonly SpeecilFlowCoordinator _speecilFlowCoordinator = null!;
    private MenuButton? _menuButton;

    public void Initialize()
    {
        _menuButton = new MenuButton("Speecil Tweaks", "Customize your game!", ShowFlowCoordinator);
        MenuButtons.Instance.RegisterButton(_menuButton);
    }

    public void Dispose()
    {
        if (_menuButton != null && MenuButtons.Instance != null)
        {
            MenuButtons.Instance.UnregisterButton(_menuButton);
        }
    }

    private void ShowFlowCoordinator()
    {
        if (_speecilFlowCoordinator == null)
        {
            Plugin.Log.Error("SpeecilFlowCoordinator is null!");
            return;
        }

        if (BeatSaberUI.MainFlowCoordinator == null)
        {
            Plugin.Log.Error("MainFlowCoordinator is null!");
            return;
        }

        BeatSaberUI.MainFlowCoordinator.PresentFlowCoordinator(_speecilFlowCoordinator);
    }
}