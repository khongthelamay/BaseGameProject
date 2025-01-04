using System;
using Cysharp.Threading.Tasks;
using TW.UGUI.MVPPattern;
using UnityEngine;
using R3;
using Sirenix.OdinInspector;
using TW.Reactive.CustomComponent;
using TMPro;
using UnityEngine.UI;
using TW.UGUI.Core.Modals;
using Core;
using System.Collections.Generic;
using UnityEngine.Events;

[Serializable]
public class ModalHeroesInforContext 
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
        [field: SerializeField] public ReactiveValue<HeroSave> CurrentHeroSave { get; set; }
        [field: SerializeField] public ReactiveValue<HeroConfigData> CurrentHeroChoose { get; private set; }

        public UniTask Initialize(Memory<object> args)
        {
            CurrentHeroChoose = HeroManager.Instance.CurrentHeroChoose;
            CurrentHeroSave = HeroManager.Instance.CurrentHeroSave;
            return UniTask.CompletedTask;
        }
    }
    
    [HideLabel]
    [Serializable]
    public class UIView : IAView
    {
        [field: Title(nameof(UIView))]
        [field: SerializeField] public CanvasGroup MainView {get; private set;}
        [field: SerializeField] public Transform MainContent { get; private set; }
        [field: SerializeField] public TextMeshProUGUI TxtLevel {get; private set;}  
        [field: SerializeField] public TextMeshProUGUI TxtName {get; private set;}  
        [field: SerializeField] public TextMeshProUGUI TxtAtk {get; private set;}  
        [field: SerializeField] public TextMeshProUGUI TxtSpeed {get; private set;}  
        [field: SerializeField] public ProgressBar PiecesProgress {get; private set;}  
        [field: SerializeField] public Animator HeroAnimator {get; private set;}  
        [field: SerializeField] public Button BtnExit {get; private set;}  
        [field: SerializeField] public Button BtnUpgrade {get; private set;}  
        [field: SerializeField] public GameObject ObjUpgrade {get; private set;}  
        [field: SerializeField] public GameObject ObjSummondRecipe {get; private set;}  
        [field: SerializeField] public MainContentAbility MainContentHeroAbility { get; private set;}  
        [field: SerializeField] public MainContentHeroJob MainContentHeroJob {get; private set;}
        [field: SerializeField] public RectTransform MainRect {get; private set;}
        [field: SerializeField] public AbilityContent AbilityContent {get; private set;}
        [field: SerializeField] public Vector2 AbilityOffset {get; private set;}
        [field: SerializeField] public RectTransform Narrow {get; private set;}

        List<Hero.Job> jobs;

        public UniTask Initialize(Memory<object> args)
        {
            return UniTask.CompletedTask;
        }
        public void InitData(HeroConfigData heroData, HeroSave heroDataSave) {
            Debug.Log("change data");
            HeroAnimator.runtimeAnimatorController = heroData.ImageAnimatorController;
            TxtLevel.text = $"Lv. {heroDataSave.level.Value}";
            TxtName.text = heroData.Name;
            TxtAtk.text = heroData.BaseAttackDamage.ToString();
            TxtSpeed.text = heroData.BaseAttackSpeed.ToString();
            PiecesProgress.ChangeProgress(heroDataSave.piece.Value/10f);
            PiecesProgress.ChangeTextProgress($"{heroDataSave.piece.Value}/10");
            ObjUpgrade.SetActive(heroDataSave.piece.Value / 10f == 1f);
            ObjSummondRecipe.SetActive(heroData.HeroRarity == Hero.Rarity.Mythic);
            
            MainContentHeroAbility.gameObject.SetActive(heroData.HeroAbilities.Count > 0);
            if (heroData.HeroAbilities.Count > 0)
                MainContentHeroAbility.InitData(heroData.HeroAbilities);
            
            jobs = new(heroData.HeroJob);
            MainContentHeroJob.InitData(jobs);
            BtnUpgrade.interactable = HeroManager.Instance.IsCanUpgradeHero(heroData.Name);
            
            LayoutRebuilder.ForceRebuildLayoutImmediate(MainRect);
        }

        public void AnimOpen()
        {
            UIAnimation.ModalOpen(MainView, MainContent);
        }

        public void AnimClose()
        {
            UIAnimation.BasicButton(BtnExit.transform);
            UIAnimation.ModalClose(MainView, MainContent, () => {
                ModalContainer.Find(ContainerKey.Modals).Pop(true);
            });
        }
        
        public void SetActionMainAbilityCallBack(UnityAction<SlotBase<Ability>> action)
        {
            MainContentHeroAbility.SetActionSlotCallBack(action);
        }

        public void ShowAbilityInfor(SlotBase<Ability> slotBase)
        {
            RectTransform slotRect = slotBase.GetComponent<RectTransform>(); 
            RectTransform rect = AbilityContent.GetComponent<RectTransform>();
            Vector2 slotAnchoredPosition = slotRect.anchoredPosition + AbilityOffset;
            Vector2 narrowPosition = slotAnchoredPosition;
            narrowPosition.y = 0;
            Narrow.anchoredPosition = narrowPosition;
            float sizeRect = MainRect.rect.width/2 - rect.rect.width/2;
            slotAnchoredPosition.x = Mathf.Clamp(slotAnchoredPosition.x, sizeRect * 2, sizeRect * 4);
            rect.anchoredPosition = slotAnchoredPosition;
            AbilityContent.gameObject.SetActive(true);
            AbilityContent.InitData(slotBase.slotData);
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
            View.SetActionMainAbilityCallBack(ActionAbilityCallBack);
            Model.CurrentHeroSave.ReactiveProperty
                .CombineLatest(Model.CurrentHeroSave.Value.level.ReactiveProperty, (heroSave, level)=>(herosave: heroSave, level))
                .CombineLatest(Model.CurrentHeroSave.Value.piece.ReactiveProperty, (level, piece) => (level.herosave, level.level, piece))
                .Subscribe(ChangeData)
                .AddTo(View.MainView);
            View.BtnExit.onClick.AddListener(Exit);
            View.BtnUpgrade.onClick.AddListener(Upgrade);
            View.AbilityContent.SetActionCallBack(DisAbleArrow);
            DisAbleArrow(false);
            View.AnimOpen();
        }

        public void DisAbleArrow(bool active)
        {
            View.Narrow.gameObject.SetActive(active);
        }

        void ActionAbilityCallBack(SlotBase<Ability> slotBase) {
            View.ShowAbilityInfor(slotBase);
        }

        private void ChangeData((HeroSave heroSave, int level, int piece) value)
        {
            View.InitData(Model.CurrentHeroChoose, Model.CurrentHeroSave);
        }

        void Exit() {
            UIAnimation.BasicButton(View.BtnExit.transform);
            View.AnimClose();
        }
        void Upgrade() {
            UIAnimation.BasicButton(View.BtnUpgrade.transform);
            HeroManager.Instance.UpgradeHero();
        }

        UniTask IModalLifecycleEvent.Cleanup(Memory<object> args)
        {
            View.PiecesProgress.ClearAnimation();
            View.AbilityContent.ClearAnimation();
            return UniTask.CompletedTask;
        }
    }
}