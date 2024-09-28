using Cysharp.Threading.Tasks;
using TW.UGUI.Core.Modals;
using TW.UGUI.Core.Screens;
using TW.UGUI.Core.Views;
using TW.UGUI.Core.Windows;

using ZBase.UnityScreenNavigator.Core;

public class Launcher : UnityScreenNavigatorLauncher
{
    protected override void Start()
    {
        base.Start();
        // ShowLoadingScreen().Forget();
        ShowPanelTest();


    }

    async UniTaskVoid ShowPanelTest() {
        //ViewOptions options = new ViewOptions(nameof(ScreensArtifact));
        //await ScreenContainer.Find(ContainerKey.Screens).PushAsync(options);
    }


    // private async UniTaskVoid ShowLoadingScreen()
    // {
    //     //ViewOptions options = new ViewOptions(nameof(ScreenTitle), false, loadAsync: false);
    //     ViewOptions options = new ViewOptions(nameof(ScreenMainMenu));
    //     await ScreenContainer.Find(ContainerKey.Screens).PushAsync(options);
    // }
}
