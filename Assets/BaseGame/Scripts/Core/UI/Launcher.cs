using Cysharp.Threading.Tasks;
using TW.UGUI.Core.Screens;
using TW.UGUI.Core.Views;
using ZBase.UnityScreenNavigator.Core;

public class Launcher : UnityScreenNavigatorLauncher
{
    protected override void Start()
    {
        base.Start();
        ShowPanelTest();
    }

    async UniTaskVoid ShowPanelTest() {
        ViewOptions options = new ViewOptions(nameof(ScreensDefault));
        await ScreenContainer.Find(ContainerKey.MidleScreens).PushAsync(options);
    }


    // private async UniTaskVoid ShowLoadingScreen()
    // {
    //     //ViewOptions options = new ViewOptions(nameof(ScreenTitle), false, loadAsync: false);
    //     ViewOptions options = new ViewOptions(nameof(ScreenMainMenu));
    //     await ScreenContainer.Find(ContainerKey.Screens).PushAsync(options);
    // }
}
