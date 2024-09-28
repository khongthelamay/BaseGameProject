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
public class ScreensMenuContext 
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
        [field: SerializeField] public CanvasGroup MainView {get; private set;}
        [SerializeField] List<SlotTabMainMenu> tabs = new();
        SlotTabMainMenu currentTab;
        public UniTask Initialize(Memory<object> args)
        {
            Debug.Log("Init screen");
            for (int i = 0; i < tabs.Count; i++)
            {
                tabs[i].InitData((TabType)i, OnChooseTab);
            }
            OnChooseTab(tabs[2]);
            return UniTask.CompletedTask;
        }

        void OnChooseTab(SlotTabMainMenu tabChoose) {
            if (currentTab != tabChoose)
            {
                currentTab = tabChoose;
                currentTab.AnimOnSelect();
                for (int i = 0; i < tabs.Count; i++)
                {
                    if (currentTab != tabs[i])
                        tabs[i].OnDeSelect();
                }
            }
           
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