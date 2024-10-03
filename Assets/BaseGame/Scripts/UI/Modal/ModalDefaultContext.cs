using System;
using Cysharp.Threading.Tasks;
using TW.UGUI.MVPPattern;
using UnityEngine;
using R3;
using Sirenix.OdinInspector;
using TW.Reactive.CustomComponent;
using UGUI.Core.Modals;
using System.Collections.Generic;
using TW.UGUI.Core.Screens;
using TW.UGUI.Core.Views;
using TW.UGUI.Core.Modals;

[Serializable]
public class ModalDefaultContext 
{
    public static class Events
    {
        public static Subject<Unit> SampleEvent { get; set; } = new();
        public static Event OpenModal { get; set; } = new();
        public static Event CloseModal { get; set; } = new();
    }

    [HideLabel]
    [Serializable]
    public class UIModel : IAModel
    {
        [field: Title(nameof(UIModel))]
        [field: SerializeField] public ReactiveValue<int> SampleValue { get; private set; }
        
        public List<SlotTabMainMenu> tabs = new();
        SlotTabMainMenu currentTab;
        public UniTask Initialize(Memory<object> args)
        {
            for (int i = 0; i < tabs.Count; i++)
            {
                tabs[i].InitData((TabType)i, OnChooseTab);
            }
            
            return UniTask.CompletedTask;
        }

        public void OnChooseTab(SlotTabMainMenu tabChoose)
        {
            if (currentTab != tabChoose)
            {
                if (currentTab != null)
                    if (currentTab.tabType != TabType.TabCommingsoon)
                        ScreenContainer.Find(ContainerKey.Screens).Pop(true);

                currentTab = tabChoose;
                currentTab.AnimOnSelect();
                
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

        void TabShop()
        {
            ViewOptions options = new ViewOptions(nameof(ScreensArtifact));
            ScreenContainer.Find(ContainerKey.Screens).PushAsync(options);
        }
        void TabHeroes()
        {
            ViewOptions options = new ViewOptions(nameof(ScreensHeroes));
            ScreenContainer.Find(ContainerKey.Screens).PushAsync(options);
        }
        void TabBattle()
        {
            ViewOptions options = new ViewOptions(nameof(ScreensBattle));
            ScreenContainer.Find(ContainerKey.Screens).PushAsync(options);
        }
        void TabArtifact()
        {
            ViewOptions options = new ViewOptions(nameof(ScreensArtifact));
            ScreenContainer.Find(ContainerKey.Screens).PushAsync(options);
        }
        void TabCommingSoon()
        {
            Debug.Log("Comming Soon");
        }
    }
    
    [HideLabel]
    [Serializable]
    public class UIView : IAView
    {
        [field: Title(nameof(UIView))]
        [field: SerializeField] public CanvasGroup MainView {get; private set;}  
        
        public UniTask Initialize(Memory<object> args)
        {
            return UniTask.CompletedTask;
        }
    }

    [HideLabel]
    [Serializable]
    public class UIPresenter : IAPresenter, IModalLifecycleEventSimple
    {
        [field: SerializeField] public UIModel Model {get; private set;} = new();
        [field: SerializeField] public UIView View { get; set; } = new();        

        public async UniTask Initialize(Memory<object> args)
        {
            await Model.Initialize(args);
            await View.Initialize(args);

            Model.OnChooseTab(Model.tabs[2]);
        }      
    }
}