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
using TW.UGUI.Core.Modals;
using TW.Utility.CustomType;

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
        [field: SerializeField] public ReactiveValue<BigNumber> recruitRecipe { get; private set; } = new();
        [field: SerializeField] public int currentRewardMoveDone;

        [field: SerializeField] public int lastRecruitAmount;

        public UniTask Initialize(Memory<object> args)
        {
            recruitRewards = RecruitManager.Instance.recruitRewards;
            totalRewardCanGetThisTurn = RecruitManager.Instance.totalRewardNeedGetThisTurn;
            recruitRecipe = RecruitManager.Instance.recruitRecipe;
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
        [field: SerializeField] public Button btnContinueRecruit {get; private set;}
        [field: SerializeField] public ButtonNotice btnRecruit { get; private set; }
        [field: SerializeField] public ButtonNotice btnRecruitX10 {get; private set;}  
        [field: SerializeField] public GameObject objButtonRecruit {get; private set;}  
        [field: SerializeField] public MainContentRecruitReward rewardMove {get; private set;}  
        [field: SerializeField] public MainContentRecruitReward rewardShow {get; private set;}  
        [field: SerializeField] public TextMeshProUGUI txtCountCurrentRewardGet {get; private set;}  
        [field: SerializeField] public TextMeshProUGUI txtRecruitRecipe {get; private set;}  
        [field: SerializeField] public TextMeshProUGUI txtRecruitRecipeX10 {get; private set;}  
        
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

        public void ChangeCountRewardText(int currentRewardEarned, int totalReward) {
            txtCountCurrentRewardGet.text = currentRewardEarned.ToString() + "/" + totalReward.ToString();
        }

        public void StartMoveReward()
        {
            rewardMove.SlotRewardRunNow();
        }

        public void ShowReward(int slotIndex)
        {
            rewardShow.ShowReward(slotIndex);
        }

        public void ChangeRecruitRecipe(BigNumber amount)
        {
            txtRecruitRecipe.text = $"{amount}/30";
            txtRecruitRecipeX10.text = $"{amount}/300";
            btnRecruit.SetInteract(amount >= 30);
            btnRecruitX10.SetInteract(amount >= 300);

            btnRecruit.ChangeShowNotice(amount >= 30);
            btnRecruitX10.ChangeShowNotice(amount >= 300);
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
            View.btnContinueRecruit.onClick.AddListener(ContinueRecruit);

            View.btnRecruit.SetButtonOnClick(Recruit);
            View.btnRecruitX10.SetButtonOnClick(RecruitX10);

            View.rewardMove.SetActionCallbackMoveDone(ActionCallbackMoveDone);

            View.btnContinueRecruit.gameObject.SetActive(false);

            Model.recruitRewards.ObservableList.ObserveChanged().Subscribe(ChangeListRewards).AddTo(View.MainView);
            Model.totalRewardCanGetThisTurn.ReactiveProperty.Subscribe(ChangeTotalRewardNeedEarn).AddTo(View.MainView);
            Model.recruitRecipe.ReactiveProperty.Subscribe(ChangeRecruitRecipe).AddTo(View.MainView);
            Events.LastSlotMoveDone = LastSlotMoveDone;
        }

        void ChangeRecruitRecipe(BigNumber amount) {
            View.ChangeRecruitRecipe(amount);
        }

        void ChangeTotalRewardNeedEarn(int totalRewardNeedEarn) {
            View.ChangeCountRewardText(Model.currentRewardMoveDone, Model.totalRewardCanGetThisTurn);
        }

        void ContinueRecruit()
        {
            Model.currentRewardMoveDone = 0;
            View.ChangeCountRewardText(Model.currentRewardMoveDone, Model.totalRewardCanGetThisTurn);
            RecruitManager.Instance.InitData(Model.lastRecruitAmount);
            View.btnContinueRecruit.gameObject.SetActive(false);
        }

        void LastSlotMoveDone() {
            View.objButtonRecruit.gameObject.SetActive(true);
            bool isCanContinue = RecruitManager.Instance.IsCanContinue();
            View.btnRecruit.gameObject.SetActive(!isCanContinue);
            View.btnRecruitX10.gameObject.SetActive(!isCanContinue);
            View.btnContinueRecruit.gameObject.SetActive(isCanContinue);
        }

        void OnClose() {
            View.rewardMove.CleanAnimation();
            View.rewardShow.CleanAnimation();
            ScreenContainer.Find(ContainerKey.Screens).PopAsync(true);

            if (RecruitManager.Instance.IsHaveHeroReward())
            {
                ViewOptions options = new ViewOptions(nameof(ScreensClaimHeroes));
                ScreenContainer.Find(ContainerKey.Screens).PushAsync(options);
            }
            else
            {
                ViewOptions options = new ViewOptions(nameof(ScreensDefault));
                ScreenContainer.Find(ContainerKey.MidleScreens).PushAsync(options);
            }
        }

        void Recruit() {
            Model.lastRecruitAmount = 1;
            Model.currentRewardMoveDone = 0;
            RecruitManager.Instance.ResetTurn();
            RecruitManager.Instance.InitData(1);
            View.objButtonRecruit.gameObject.SetActive(false);
        }

        void RecruitX10() {
            Model.lastRecruitAmount = 10;
            Model.currentRewardMoveDone = 0;
            RecruitManager.Instance.ResetTurn();
            RecruitManager.Instance.InitData(10);
            View.objButtonRecruit.gameObject.SetActive(false);
        }

        void ChangeListRewards(CollectionChangedEvent<RecruitReward> recruitReward) {
            View.InitDataRewardMove(Model.recruitRewards);
            View.InitDataRewardShow(Model.recruitRewards);
            View.ChangeCountRewardText(Model.currentRewardMoveDone, Model.totalRewardCanGetThisTurn);
            View.StartMoveReward();
        }

        void ActionCallbackMoveDone(SlotRecruitReward slot) {
            View.ShowReward(slot.transform.GetSiblingIndex() - 1);
            Model.currentRewardMoveDone++;
            View.ChangeCountRewardText(Model.currentRewardMoveDone, Model.totalRewardCanGetThisTurn);
        }
    }
}