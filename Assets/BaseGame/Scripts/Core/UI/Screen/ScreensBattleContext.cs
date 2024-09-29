using System;
using Cysharp.Threading.Tasks;
using TW.UGUI.MVPPattern;
using UnityEngine;
using R3;
using Sirenix.OdinInspector;
using TW.Reactive.CustomComponent;
using TW.UGUI.Core.Screens;
using System.Collections.Generic;
using TW.UGUI.Core.Views;
using UnityEngine.UI;
using TW.UGUI.Core.Modals;

[Serializable]
public class ScreensBattleContext 
{
    public static class Events
    {
        public static Subject<Unit> SampleEvent { get; set; } = new();
    }
    
    [HideLabel]
    [Serializable]
    public class UIModel : IAModel
    {
        [field: Title(nameof(UIModel))]
        [field: SerializeField] public ReactiveValue<int> SampleValue { get; private set; }

        public UniTask Initialize(Memory<object> args)
        {
            return UniTask.CompletedTask;
        }
    }

    [HideLabel]
    [Serializable]
    public class UIView : IAView
    {
        [field: Title(nameof(UIView))]
        [field: SerializeField] public CanvasGroup MainView { get; private set; }

        [field: SerializeField] public ButtonNotice btnQuest;
        [field: SerializeField] public Button btnRecruit;
        [field: SerializeField] public Button btnSpecialShop;

        public UniTask Initialize(Memory<object> args)
        {
            btnQuest.SetButtonOnClick(ShowModalQuest);
            return UniTask.CompletedTask;
        }

        void ShowModalQuest()
        {
            ViewOptions options = new ViewOptions(nameof(ModalQuest));
            ModalContainer.Find(ContainerKey.Modals).PushAsync(options);
        }
    }

    [HideLabel]
    [Serializable]
    public class UIPresenter : IAPresenter, IScreenLifecycleEventSimple
    {
        [field: SerializeField] public UIModel Model {get; private set;} = new();
        [field: SerializeField] public UIView View { get; set; } = new();        

        public async UniTask Initialize(Memory<object> args)
        {
            await Model.Initialize(args);
            await View.Initialize(args);
        }      

    }
}