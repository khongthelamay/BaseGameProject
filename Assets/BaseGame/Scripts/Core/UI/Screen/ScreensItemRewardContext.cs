using System;
using Cysharp.Threading.Tasks;
using TW.UGUI.MVPPattern;
using UnityEngine;
using R3;
using Sirenix.OdinInspector;
using TW.Reactive.CustomComponent;
using TW.UGUI.Core.Screens;
using System.Collections.Generic;

[Serializable]
public class ScreensItemRewardContext 
{
    public static class Events
    {
        public static Subject<Unit> SampleEvent { get; set; } = new();
        public static Action<List<RecruitReward>> LoadData { get; set; }
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
        [field: SerializeField] public CanvasGroup MainView {get; private set;}
        [field: SerializeField] public MainContentItemReward mainContentItemReward { get; private set; }

        public UniTask Initialize(Memory<object> args)
        {
            return UniTask.CompletedTask;
        }

        public void LoadData(List<RecruitReward> recruitRewards)
        {
            mainContentItemReward.InitData(recruitRewards);
        }
    }

    [HideLabel]
    [Serializable]
    public class UIPresenter : IAPresenter, IScreenLifecycleEventSimple
    {
        [field: SerializeField] public UIModel Model { get; private set; } = new();
        [field: SerializeField] public UIView View { get; set; } = new();

        public async UniTask Initialize(Memory<object> args)
        {
            await Model.Initialize(args);
            await View.Initialize(args);

            Events.LoadData = LoadData;
        }

        void LoadData(List<RecruitReward> recruitRewards) {
            View.LoadData(recruitRewards);
        }

        public UniTask Cleanup(Memory<object> args)
        {
            Events.SampleEvent = null;
            Events.LoadData = null;
            return UniTask.CompletedTask;
        }
    }
}