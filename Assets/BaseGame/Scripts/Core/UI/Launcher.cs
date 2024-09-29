using Cysharp.Threading.Tasks;
using System;
using TW.UGUI.Core.Modals;
using TW.UGUI.Core.Screens;
using TW.UGUI.Core.Views;
using TW.UGUI.Core.Windows;
using UnityEngine;
using ZBase.UnityScreenNavigator.Core;

public class Launcher : UnityScreenNavigatorLauncher
{
    public GameObject objModalDefault;
    protected override void Start()
    {
        base.Start();
        // ShowLoadingScreen().Forget();
        //ShowPanelTest();
        objModalDefault.SetActive(true);

    }

    async UniTaskVoid ShowPanelTest() {
        ViewOptions options = new ViewOptions(nameof(ScreensArtifact));
        await ScreenContainer.Find(ContainerKey.Screens).PushAsync(options);
    }


    // private async UniTaskVoid ShowLoadingScreen()
    // {
    //     //ViewOptions options = new ViewOptions(nameof(ScreenTitle), false, loadAsync: false);
    //     ViewOptions options = new ViewOptions(nameof(ScreenMainMenu));
    //     await ScreenContainer.Find(ContainerKey.Screens).PushAsync(options);
    // }
}
