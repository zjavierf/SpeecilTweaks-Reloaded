using BeatSaberMarkupLanguage;
using HMUI;
using Zenject;

namespace SpeecilTweaks.UI;

public class SpeecilFlowCoordinator : FlowCoordinator
{
    private SpeecilSettingsViewController _settingsViewController = null!;

    [Inject]
    public void Construct(SpeecilSettingsViewController settingsViewController)
    {
        _settingsViewController = settingsViewController;
    }

    protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
    {
        if (firstActivation)
        {
            showBackButton = false;
            ProvideInitialViewControllers(_settingsViewController);
        }
    }

    public void DismissSelf()
    {
        BeatSaberUI.MainFlowCoordinator.DismissFlowCoordinator(this);
    }

    protected override void BackButtonWasPressed(ViewController topViewController)
    {
        BeatSaberUI.MainFlowCoordinator.DismissFlowCoordinator(this);
    }
}