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
        [field: SerializeField] public List<HunterPass> hunterPassesConfig { get; private set; }
        [field: SerializeField] public ReactiveList<HunterPassData> hunterPasses { get; private set; }


        public UniTask Initialize(Memory<object> args)
        {
            hunterPassesConfig = HunterPassGlobalConfig.Instance.hunterPasses;
            hunterPasses = HunterPassManager.Instance.hunterPassDataSave;
            return UniTask.CompletedTask;
        }
    }
    
    [HideLabel]
    [Serializable]
    public class UIView : IAView
    {
        [field: Title(nameof(UIView))]
        [field: SerializeField] public CanvasGroup MainView {get; private set;}  
        [field: SerializeField] public MainContentHuntPass mainContentHuntPass {get; private set;}  
        [field: SerializeField] public Button btnClose {get; private set;}  
        
        public UniTask Initialize(Memory<object> args)
        {
            return UniTask.CompletedTask;
        }

        public void InitData(List<HunterPass> hunterPasses) { mainContentHuntPass.InitData(hunterPasses); }

        public void SetActionClaimCommond(UnityAction<SlotBase<HunterPass>> claimCommond)
        {
            mainContentHuntPass.SetActionSlotCallBack(claimCommond);
        }

        public void SetActionClaimPremium(UnityAction<SlotBase<HunterPass>> claimPremieum)
        {
            mainContentHuntPass.SetActionClaimPremium(claimPremieum);
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

            Model.hunterPasses.ObservableList.ObserveChanged().Subscribe(ChangeDataHunterPass).AddTo(View.MainView);

            View.btnClose.onClick.AddListener(CloseScreen);

            View.SetActionClaimCommond(ClaimCommond);
            View.InitData(Model.hunterPassesConfig);
            View.SetActionClaimPremium(SetActionClaimPremium);
        }

        void ChangeDataHunterPass(CollectionChangedEvent<HunterPassData> element)
        {
            Debug.Log(element.NewItem.level);
        }

        void SetActionClaimPremium(SlotBase<HunterPass> slotBase)
        {
            HunterPassManager.Instance.ClaimHuntCommondPass(slotBase.slotData.level);
        }

        void ClaimCommond(SlotBase<HunterPass> slotBase) {
            HunterPassManager.Instance.ClaimHuntPremieumPass(slotBase.slotData.level);
        }

        void CloseScreen()
        {
            ScreenContainer.Find(ContainerKey.Screens).Pop(true);

            ViewOptions options = new ViewOptions(nameof(ScreensDefault));
            ScreenContainer.Find(ContainerKey.MidleScreens).PushAsync(options);

        }
    }
}