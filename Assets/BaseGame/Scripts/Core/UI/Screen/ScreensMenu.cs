using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using Screen = TW.UGUI.Core.Screens.Screen;
using UnityEngine;
using System;

public class ScreensMenu : Screen
{
    [field: SerializeField] public ScreensMenuContext.UIPresenter UIPresenter { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        // The lifecycle event of the view will be added with priority 0.
        // Presenters should be processed after the view so set the priority to 1.
        AddLifecycleEvent(UIPresenter, 1);
    }

    protected override void OnEnable()
    {
        UIPresenter.Initialize(null);
    }

    public override async UniTask Initialize(Memory<object> args)
    {
        await base.Initialize(args);
    }
}
