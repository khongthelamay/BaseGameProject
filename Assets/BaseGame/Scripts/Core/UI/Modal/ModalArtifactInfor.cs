using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using TW.UGUI.Core.Modals;
using UnityEngine;
using R3;

public class ModalArtifactInfor : Modal
{
    [field: SerializeField] public ModalArtifactInforContext.UIPresenter UIPresenter { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        // The lifecycle event of the view will be added with priority 0.
        // Presenters should be processed after the view so set the priority to 1.
        AddLifecycleEvent(UIPresenter, 1);
    }

    public override async UniTask Initialize(Memory<object> args)
    {
        await base.Initialize(args);
    }
}
