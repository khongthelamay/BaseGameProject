using System;
using Cysharp.Threading.Tasks;
using TW.UGUI.MVPPattern;
using UnityEngine;
using R3;
using Sirenix.OdinInspector;
using TW.Reactive.CustomComponent;
using TW.UGUI.Core.Screens;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using TW.UGUI.Core.Views;
using ObservableCollections;

[Serializable]
public class ScreensRecruitContext 
{
    public static class Events
    {
        public static Subject<Unit> SampleEvent { get; set; } = new();
        public static Action LastSlotMoveDone;
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
        [field: SerializeField] public Button btnExit {get; private set;}
        [field: SerializeField] public ButtonNotice btnRecruit { get; private set; }
        [field: SerializeField] public ButtonNotice btnRecruitX10 {get; private set;}  
        [field: SerializeField] public GameObject objButtonRecruit {get; private set;}  
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

        public void ShowReward(int slotIndex)
        {
            rewardShow.ShowReward(slotIndex);
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
            View.btnExit.onClick.AddListener(OnClose);
            View.btnRecruit.SetButtonOnClick(Recruit);
            View.btnRecruitX10.SetButtonOnClick(RecruitX10);
            View.rewardMove.SetActionCallbackMoveDone(ActionCallbackMoveDone);
            Model.recruitRewards.ObservableList.ObserveChanged().Subscribe(ChangeListRewards).AddTo(View.MainView);
            Events.LastSlotMoveDone = LastSlotMoveDone;
        }

        void LastSlotMoveDone() {
            View.objButtonRecruit.gameObject.SetActive(true);
        }

        void OnClose() {
            View.rewardMove.CleanAnimation();
            View.rewardShow.CleanAnimation();

            ScreenContainer.Find(ContainerKey.Screens).PopAsync(true);

            ViewOptions options = new ViewOptions(nameof(ScreensDefault));
            ScreenContainer.Find(ContainerKey.MidleScreens).PushAsync(options);
        }

        void Recruit() {
            RecruitManager.Instance.InitData(1);
            View.objButtonRecruit.gameObject.SetActive(false);
        }

        void RecruitX10() {
            RecruitManager.Instance.InitData(10);
            View.objButtonRecruit.gameObject.SetActive(false);
        }

        void ChangeListRewards(CollectionChangedEvent<RecruitReward> recruitReward) {
            View.InitDataRewardMove(Model.recruitRewards);
            View.InitDataRewardShow(Model.recruitRewards);
            View.ChangeCountRewardText(Model.totalRewardCanGetThisTurn);
            View.StartMoveReward();
        }

        void ActionCallbackMoveDone(SlotRecruitReward slot) {
            View.ShowReward(slot.transform.GetSiblingIndex() - 1);
        }
    }
}