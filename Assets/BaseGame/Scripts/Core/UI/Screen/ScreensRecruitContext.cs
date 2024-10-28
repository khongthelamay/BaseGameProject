using System;
using Cysharp.Threading.Tasks;
using TW.UGUI.MVPPattern;
using UnityEngine;
using R3;
using Sirenix.OdinInspector;
using TW.Reactive.CustomComponent;
using TW.UGUI.Core.Screens;
using System.Collections.Generic;
using System.Linq;
using ObservableCollections;
using TMPro;

[Serializable]
public class ScreensRecruitContext 
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
        [field: SerializeField] public ReactiveList<RecruitReward> recruitRewards { get; private set; } = new();
        [field: SerializeField] public ReactiveValue<int> totalRewardCanGetThisTurn { get; private set; } = new();

        public UniTask Initialize(Memory<object> args)
        {
            recruitRewards = RecruitManager.Instance.recruitRewards;
            totalRewardCanGetThisTurn = RecruitManager.Instance.totalRewardCanGetThisTurn;
            return UniTask.CompletedTask;
        }
    }
    
    [HideLabel]
    [Serializable]
    public class UIView : IAView
    {
        [field: Title(nameof(UIView))]
        [field: SerializeField] public CanvasGroup MainView {get; private set;}  
        [field: SerializeField] public MainRecruitRewardContent rewardMove {get; private set;}  
        [field: SerializeField] public MainRecruitRewardContent rewardShow {get; private set;}  
        [field: SerializeField] public TextMeshProUGUI txtCountCurrentRewardGet {get; private set;}  
        
        public UniTask Initialize(Memory<object> args)
        {
            return UniTask.CompletedTask;
        }

        public void InitDataRewardMove(List<RecruitReward> recruitRewards) {
            rewardMove.InitData(recruitRewards);
        }

        public void InitDataRewardShow(List<RecruitReward> recruitRewards) {
            rewardShow.InitData(recruitRewards);
        }

        public void ChangeCountRewardText(int totalReward) {
            txtCountCurrentRewardGet.text = totalReward.ToString();
        }

        public void StartMoveReward()
        {
            rewardMove.SlotRewardRunNow();
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

            Model.recruitRewards.ObservableList.ObserveChanged().Subscribe(ChangeListRewards).AddTo(View.MainView);

        }

        void ChangeListRewards(CollectionChangedEvent<RecruitReward> recruitReward) {
            View.InitDataRewardMove(Model.recruitRewards);
            View.InitDataRewardShow(Model.recruitRewards);
            View.ChangeCountRewardText(Model.totalRewardCanGetThisTurn);
            View.StartMoveReward();
        }
    }
}