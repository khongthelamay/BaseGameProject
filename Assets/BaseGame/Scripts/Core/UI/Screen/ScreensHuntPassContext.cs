using System;
using Cysharp.Threading.Tasks;
using TW.UGUI.MVPPattern;
using UnityEngine;
using R3;
using Sirenix.OdinInspector;
using TW.Reactive.CustomComponent;
using TW.UGUI.Core.Screens;
using UnityEngine.UI;
using TW.UGUI.Core.Views;
using System.Collections.Generic;
using UnityEngine.Events;
using ObservableCollections;
using TMPro;

[Serializable]
public class ScreensHuntPassContext 
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
        [field: SerializeField] public List<HuntPass> huntPassesConfig { get;  set; }
        [field: SerializeField] public HuntPass currentHuntPassConfig { get;  set; }
        [field: SerializeField] public ReactiveValue<float> huntPoint { get;  set; }
        public ReactiveValue<int> huntLevel;

        public UniTask Initialize(Memory<object> args)
        {
            huntPassesConfig = HuntPassGlobalConfig.Instance.huntPasses;
            huntPoint = HuntPassManager.Instance.huntPoint;
            huntLevel = HuntPassManager.Instance.huntLevel;
            return UniTask.CompletedTask;
        }
    }

    [HideLabel]
    [Serializable]
    public class UIView : IAView
    {
        [field: Title(nameof(UIView))]
        [field: SerializeField] public CanvasGroup MainView { get; private set; }
        [field: SerializeField] public MainContentHuntPass mainContentHuntPass { get; set; }
        [field: SerializeField] public ProgressBar levelHuntPass { get;  set; }
        [field: SerializeField] public Button btnClose { get; set; }
        
        public TextMeshProUGUI txtCurrentLevel;
        public TextMeshProUGUI txtNextLevel;
        
        public GameObject objNextLevel;
        
        public UniTask Initialize(Memory<object> args)
        {
            return UniTask.CompletedTask;
        }

        public void InitData(List<HuntPass> hunterPasses) { mainContentHuntPass.InitData(hunterPasses); }

        public void SetActionClaimCommond(UnityAction<SlotBase<HuntPass>> claimCommond)
        {
            mainContentHuntPass.SetActionSlotCallBack(claimCommond);
        }

        public void SetActionClaimPremium(UnityAction<SlotBase<HuntPass>> claimPremieum)
        {
            mainContentHuntPass.SetActionClaimPremium(claimPremieum);
        }

        public void ChangeHuntPassExp(float exp, HuntPass huntPass) {
            Debug.Log($"{exp}/{huntPass.expRequire}");
            levelHuntPass.ChangeProgress(exp/huntPass.expRequire);
            levelHuntPass.ChangeTextProgress($"{exp}/{huntPass.expRequire}");
        }

        public void ChangeTextLevel(int level)
        {
            txtCurrentLevel.text = level.ToString();
            bool isMaxLevel = level >= HuntPassManager.Instance.huntPasses.Count;
            objNextLevel.SetActive(!isMaxLevel);
            txtNextLevel.text = !isMaxLevel? (level + 1).ToString() : "";
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

            View.btnClose.onClick.AddListener(CloseScreen);

            View.SetActionClaimCommond(ActionClaimCommond);
            View.InitData(Model.huntPassesConfig);
            View.SetActionClaimPremium(ActionClaimPremium);

            Model.huntPoint.ReactiveProperty.Subscribe(ChangeExpHuntPass).AddTo(View.MainView);
            Model.huntLevel.ReactiveProperty.Subscribe(ChangeTextLevel).AddTo(View.MainView);

            ScrollToClaimableSlot();
        }
        
        void ChangeTextLevel(int level) {
            View.ChangeTextLevel(level);
        }

        void ChangeExpHuntPass(float exp) {
            Model.currentHuntPassConfig = HuntPassManager.Instance.currentPassConfig;
            View.ChangeHuntPassExp(exp, Model.currentHuntPassConfig);
        }

        void ActionClaimPremium(SlotBase<HuntPass> slotBase)
        {
            HuntPassManager.Instance.ClaimHuntPremieumPass(slotBase.slotData.level);

        }

        void ActionClaimCommond(SlotBase<HuntPass> slotBase) {
            HuntPassManager.Instance.ClaimHuntCommondPass(slotBase.slotData.level);
        }

        void CloseScreen()
        {
            //View.levelHuntPass.ClearAnimation();
            ScreenContainer.Find(ContainerKey.Screens).Pop(true);

            ViewOptions options = new ViewOptions(nameof(ScreensDefault));
            ScreenContainer.Find(ContainerKey.MidleScreens).PushAsync(options);

        }
        [Button]
        void ScrollToClaimableSlot()
        {
            foreach (SlotBase<HuntPass> slot in View.mainContentHuntPass.slots)
            {
                if (slot.slotData.level == HuntPassManager.Instance.huntLevel.Value)
                {
                    Debug.Log(slot.slotData.level);
                    View.mainContentHuntPass.ScrollToSlot(slot);
                    break;
                }
            }
        }
    }
}