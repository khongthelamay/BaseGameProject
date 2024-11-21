using System;
using Cysharp.Threading.Tasks;
using TW.UGUI.MVPPattern;
using UnityEngine;
using R3;
using Sirenix.OdinInspector;
using TW.Reactive.CustomComponent;
using TW.UGUI.Core.Screens;
using System.Collections.Generic;
using ObservableCollections;
using UnityEngine.UI;
using TW.UGUI.Core.Views;

[Serializable]
public class ScreensClaimHeroesContext 
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
        [field: SerializeField] public ReactiveList<RecruitReward> rewards { get;  set; } = new();
        public UniTask Initialize(Memory<object> args)
        {
            rewards = RecruitManager.Instance.recruitHeroRewardEarned;
            return UniTask.CompletedTask;
        }
    }
    
    [HideLabel]
    [Serializable]
    public class UIView : IAView
    {
        [field: Title(nameof(UIView))]
        [field: SerializeField] public CanvasGroup MainView {get; private set;}  
        [field: SerializeField] public MainContentHeroReward mainContentHeroReward {get; private set;}  
        [field: SerializeField] public Button btnExit {get; private set;}  
        
        public UniTask Initialize(Memory<object> args)
        {
            return UniTask.CompletedTask;
        }

        public void AddSlot(RecruitReward reward){
            mainContentHeroReward.AddSlot(reward);
        }

        public void InitData(List<RecruitReward> rewards)
        {
            mainContentHeroReward.InitData(rewards);
            //mainContentHeroReward.AnimOpen();
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
            View.InitData(Model.rewards);
        }

        void OnClose() {
            RecruitManager.Instance.ClearHeroRewardEarned();
            ScreenContainer.Find(ContainerKey.Screens).PopAsync(true);
            ViewOptions options = new ViewOptions(nameof(ScreensDefault));
            ScreenContainer.Find(ContainerKey.MidleScreens).PushAsync(options);
            
        }
    }
}