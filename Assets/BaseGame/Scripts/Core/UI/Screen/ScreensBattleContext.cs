using System;
using Cysharp.Threading.Tasks;
using TW.UGUI.MVPPattern;
using UnityEngine;
using R3;
using Sirenix.OdinInspector;
using TW.Reactive.CustomComponent;
using TW.UGUI.Core.Screens;
using Manager;
using TW.UGUI.Core.Activities;
using TW.UGUI.Core.Views;
using UnityEngine.UI;
using TW.UGUI.Core.Modals;
using System.Collections.Generic;
using TMPro;

[Serializable]
public class ScreensBattleContext 
{
    public static class Events
    {
        public static Subject<Unit> SampleEvent { get; set; } = new();
        public static Action<bool> ShowNoticeQuest { get; set; }
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

        [field: SerializeField] public MainContentRoundReward mainContentRoundReward { get; private set; }
        [field: SerializeField] public ScrollRect scrollRoundReward { get; private set; }

        [field: SerializeField] public ButtonNotice btnQuest;
        [field: SerializeField] public ButtonNotice btnHuntPass;

        [field: SerializeField] public Button btnRecruit;
        [field: SerializeField] public Button btnSpecialShop;
        [field: SerializeField] public Button btnMatch;



        public UniTask Initialize(Memory<object> args)
        {
            return UniTask.CompletedTask;
        }

        public void LoadRoundRewardData(List<RoundRewardConfig> datas) {
            mainContentRoundReward.InitData(datas);
            scrollRoundReward.verticalNormalizedPosition = 0f;
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

            View.btnQuest.SetButtonOnClick(ShowModalQuest);
            View.btnHuntPass.SetButtonOnClick(ShowScreenHuntPass, false);
            View.btnMatch.onClick.AddListener(Match);
            View.btnRecruit.onClick.AddListener(Recruit);

            View.LoadRoundRewardData(RoundRewardGlobalConfig.Instance.roundRewardConfigs);

            Events.ShowNoticeQuest = ShowNoticeQuestDone;

            QuestManager.Instance.CheckShowNoticeQuest();
        }

        async void ShowScreenHuntPass() {
            await ScreenContainer.Find(ContainerKey.Screens).PopAsync(true);
            
            ViewOptions options = new ViewOptions(nameof(ScreensHuntPass));
            await ScreenContainer.Find(ContainerKey.Screens).PushAsync(options);
            
            await ScreenContainer.Find(ContainerKey.MidleScreens).PopAsync(true);
        }

        async void ShowModalQuest()
        {
            ViewOptions options = new ViewOptions(nameof(ModalQuest));
            await ModalContainer.Find(ContainerKey.Modals).PushAsync(options);
        }

        async void Match() {
            await ScreenContainer.Find(ContainerKey.Screens).PopAsync(true);
            
            ViewOptions screenOptions = new ViewOptions(nameof(ScreensMatch));
            
            await ScreenContainer.Find(ContainerKey.Screens).PushAsync(screenOptions);
            await ScreenContainer.Find(ContainerKey.MidleScreens).PopAsync(true);
            
            ViewOptions actionOptions = new ViewOptions(nameof(ActivityHeroInfo));
            await ActivityContainer.Find(ContainerKey.Activities).ShowAsync(actionOptions);
            BattleManager.Instance.StartNewMatch();
        }

        async void Recruit() {
            await ScreenContainer.Find(ContainerKey.Screens).PopAsync(true);

            ViewOptions options = new ViewOptions(nameof(ScreensRecruit));
            await ScreenContainer.Find(ContainerKey.Screens).PushAsync(options);

            await ScreenContainer.Find(ContainerKey.MidleScreens).PopAsync(true);
        }

        void ShowNoticeQuestDone(bool active) {
            View.btnQuest.ChangeShowNotice(active);
        }

        public async UniTask Cleanup(Memory<object> args)
        {
            Events.ShowNoticeQuest = null;
        }

    }
}