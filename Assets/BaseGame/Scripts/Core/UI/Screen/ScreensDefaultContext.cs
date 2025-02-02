using System;
using Cysharp.Threading.Tasks;
using TW.UGUI.MVPPattern;
using UnityEngine;
using R3;
using Sirenix.OdinInspector;
using TW.Reactive.CustomComponent;
using System.Collections.Generic;
using TW.UGUI.Core.Screens;
using TW.UGUI.Core.Views;
using TMPro;
using TW.UGUI.Shared;

[Serializable]
public class ScreensDefaultContext 
{
    public static class Events
    {
        public static Subject<Unit> SampleEvent { get; set; } = new();
        public static Event OpenModal { get; set; } = new();
        public static Event CloseModal { get; set; } = new();
        public static Action<float> ChangeTimeRemaining { get; set; }
        public static Action TurnOffTimeRemaining { get; set; }
    }

    [HideLabel]
    [Serializable]
    public class UIModel : IAModel
    {
        [field: Title(nameof(UIModel))]
        [field: SerializeField] public ReactiveValue<int> SampleValue { get; private set; } = new(0);
        [field: SerializeField] public ReactiveValue<int> level { get; private set; } = new(0);
        [field: SerializeField] public ReactiveValue<float> exp { get; private set; } = new(0);

        public UniTask Initialize(Memory<object> args)
        {
            level = PlayerResourceManager.Instance.level;
            exp = PlayerResourceManager.Instance.exp;
            return UniTask.CompletedTask;
        }

    }
    
    [HideLabel]
    [Serializable]
    public class UIView : IAView
    {
        [field: Title(nameof(UIView))]
        [field: SerializeField] public CanvasGroup MainView {get; private set; }

        public List<SlotTabMainMenu> tabs = new();
        SlotTabMainMenu currentTab;

        [field: SerializeField] public TextMeshProUGUI txtLevel;
        [field: SerializeField] public TextMeshProUGUI txtTimeRemaining;
        [field: SerializeField] public ProgressBar progressLevel;

        [field: SerializeField] public UIResource coinResource;
        [field: SerializeField] public UIResource energyResource;
        [field: SerializeField] public UIResource gemResource;
        public UniTask Initialize(Memory<object> args)
        {
            for (int i = 0; i < tabs.Count; i++)
            {
                tabs[i].InitData((TabType)i, OnChooseTab);
            }

            coinResource.SetResourceType(ResourceType.Coin);
            energyResource.SetResourceType(ResourceType.Energy);
            gemResource.SetResourceType(ResourceType.Gem);

            return UniTask.CompletedTask;
        }

        public void ChangeLevel(int level)
        {
            for (int i = 0; i < tabs.Count; i++)
            {
                tabs[i].UnlockLevel(level);
            }
            txtLevel.text = level.ToString();
        }

        public async void OnChooseTab(SlotTabMainMenu tabChoose)
        {
            if (currentTab != tabChoose)
            {
                if (currentTab != null)
                    if (currentTab.tabType != TabType.TabCommingsoon)
                       await ScreenContainer.Find(ContainerKey.Screens).PopAsync(true);

                currentTab = tabChoose;
                currentTab.SelectMode();

                for (int i = 0; i < tabs.Count; i++)
                {
                    if (currentTab != tabs[i])
                        tabs[i].OnDeSelect();
                }
                switch (currentTab.tabType)
                {
                    case TabType.TabShop:
                        TabShop();
                        break;
                    case TabType.TabHeroes:
                        TabHeroes();
                        break;
                    case TabType.TabBattle:
                        TabBattle();
                        break;
                    case TabType.TabArtiFact:
                        TabArtifact();
                        break;
                    case TabType.TabCommingsoon:
                        TabCommingSoon();
                        break;
                }
            }
        }

        async void TabShop()
        {
            ViewOptions options = new ViewOptions(nameof(ScreensShop));
            await ScreenContainer.Find(ContainerKey.Screens).PushAsync(options);
        }
        async void TabHeroes()
        {
            ViewOptions options = new ViewOptions(nameof(ScreensHeroes));
            await ScreenContainer.Find(ContainerKey.Screens).PushAsync(options);
        }
        async void TabBattle()
        {
            ViewOptions options = new ViewOptions(nameof(ScreensBattle));
            await ScreenContainer.Find(ContainerKey.Screens).PushAsync(options);
        }
        async void TabArtifact()
        {
            ViewOptions options = new ViewOptions(nameof(ScreensArtifact));
            await ScreenContainer.Find(ContainerKey.Screens).PushAsync(options);
        }
        void TabCommingSoon()
        {
            Debug.Log("Comming Soon");
        }

        public void ChangeExp(float value)
        {
            progressLevel.ChangeProgress(value / 100);
        }

        public void ChangeTimeRemaining(string timeRemaining) {
            if (!txtTimeRemaining.gameObject.activeSelf)
                txtTimeRemaining.gameObject.SetActive(true);
            txtTimeRemaining.text = timeRemaining;
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
            View.OnChooseTab(View.tabs[2]);
            Model.level.ReactiveProperty.Subscribe(ChangeLevel).AddTo(View.MainView);
            Model.exp.ReactiveProperty.Subscribe(ChangeExp).AddTo(View.MainView);

            Events.ChangeTimeRemaining = ChangeTimeRemaining;
            Events.TurnOffTimeRemaining = TurnOffTimeRemaining;
        }

        void TurnOffTimeRemaining()
        {
            View.txtTimeRemaining.gameObject.SetActive(false);
        }

        void ChangeTimeRemaining(float timeRemaining) {
            if (timeRemaining > 0)
                View.ChangeTimeRemaining(TimeUtil.TimeToString(timeRemaining));
        }

        void ChangeExp(float value)
        {
            View.ChangeExp(value);
            // Model.level.ReactiveProperty.Subscribe(ChangeLevel).AddTo(View.MainView);
        }

        void ChangeLevel(int level) {
            View.ChangeLevel(level);
        }

        UniTask IScreenLifecycleEvent.Cleanup(Memory<object> args)
        {
            View.progressLevel.ClearAnimation();
            Events.ChangeTimeRemaining = null;
            Events.TurnOffTimeRemaining = null;
            return UniTask.CompletedTask;
        }

    }
}