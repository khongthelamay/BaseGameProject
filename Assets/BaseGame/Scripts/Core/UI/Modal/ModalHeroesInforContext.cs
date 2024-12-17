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
using TW.UGUI.Core.Views;
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
        [field: SerializeField] public ReactiveValue<HeroSave> currentHeroSave { get; set; }
        [field: SerializeField] public ReactiveValue<HeroConfigData> currentHeroChoose { get; private set; }

        public UniTask Initialize(Memory<object> args)
        {
            currentHeroChoose = HeroManager.Instance.currentHeroChoose;
            currentHeroSave = HeroManager.Instance.currentHeroSave;
            return UniTask.CompletedTask;
        }
    }
    
    [HideLabel]
    [Serializable]
    public class UIView : IAView
    {
        [field: Title(nameof(UIView))]
        [field: SerializeField] public CanvasGroup MainView {get; private set;}
        [field: SerializeField] public Transform mainContent { get; private set; }
        [field: SerializeField] public TextMeshProUGUI txtLevel {get; private set;}  
        [field: SerializeField] public TextMeshProUGUI txtName {get; private set;}  
        [field: SerializeField] public TextMeshProUGUI txtAtk {get; private set;}  
        [field: SerializeField] public TextMeshProUGUI txtSpeed {get; private set;}  
        [field: SerializeField] public ProgressBar piecesProgress {get; private set;}  
        [field: SerializeField] public Animator heroAnimator {get; private set;}  
        [field: SerializeField] public Button btnExit {get; private set;}  
        [field: SerializeField] public Button btnUpgrade {get; private set;}  
        [field: SerializeField] public GameObject objUpgrade {get; private set;}  
        [field: SerializeField] public GameObject objSummondRecipe {get; private set;}  
        [field: SerializeField] public MainContentAbility mainContentHeroAbility { get; private set;}  
        [field: SerializeField] public MainContentHeroJob mainContentHeroJob {get; private set;}
        [field: SerializeField] public RectTransform mainRect {get; private set;}
        [field: SerializeField] public AbilityContent abilityContent {get; private set;}
        [field: SerializeField] public Vector2 abilityOffset {get; private set;}

        List<Hero.Job> jobs;

        public UniTask Initialize(Memory<object> args)
        {
            return UniTask.CompletedTask;
        }
        public void InitData(HeroConfigData heroData, HeroSave heroDataSave) {
            Debug.Log("change data");
            heroAnimator.runtimeAnimatorController = heroData.ImageAnimatorController;
            txtLevel.text = $"Lv. {heroDataSave.level.Value}";
            txtName.text = heroData.Name;
            txtAtk.text = heroData.BaseAttackDamage.ToString();
            txtSpeed.text = heroData.BaseAttackSpeed.ToString();
            piecesProgress.ChangeProgress(heroDataSave.piece.Value/10f);
            piecesProgress.ChangeTextProgress($"{heroDataSave.piece.Value}/10");
            objUpgrade.SetActive(heroDataSave.piece.Value / 10f == 1f);
            objSummondRecipe.SetActive(heroData.HeroRarity == Hero.Rarity.Mythic);
            
            mainContentHeroAbility.gameObject.SetActive(heroData.HeroAbilities.Count > 0);
            if (heroData.HeroAbilities.Count > 0)
                mainContentHeroAbility.InitData(heroData.HeroAbilities);
            
            jobs = new(heroData.HeroJob);
            mainContentHeroJob.InitData(jobs);
            btnUpgrade.interactable = HeroManager.Instance.IsCanUpgradeHero(heroData.Name);
            
            LayoutRebuilder.ForceRebuildLayoutImmediate(mainRect);
        }

        public void AnimOpen()
        {
            UIAnimation.ModalOpen(MainView, mainContent);
        }

        public void AnimClose()
        {
            UIAnimation.BasicButton(btnExit.transform);
            UIAnimation.ModalClose(MainView, mainContent, () => {
                ModalContainer.Find(ContainerKey.Modals).Pop(true);
            });
        }
        
        public void SetActionMainAbilityCallBack(UnityAction<SlotBase<Ability>> action)
        {
            mainContentHeroAbility.SetActionSlotCallBack(action);
        }

        public void ShowAbilityInfor(SlotBase<Ability> slotBase)
        {
            RectTransform slotRect = slotBase.GetComponent<RectTransform>(); 
            RectTransform rect = abilityContent.GetComponent<RectTransform>();
            Vector2 slotAnchoredPosition = slotRect.anchoredPosition + abilityOffset;
            float sizeRect = mainRect.rect.width/2 - rect.rect.width/2;
            slotAnchoredPosition.x = Mathf.Clamp(slotAnchoredPosition.x, sizeRect * 2, sizeRect * 4);
            rect.anchoredPosition = slotAnchoredPosition;
            abilityContent.gameObject.SetActive(true);
            abilityContent.InitData(slotBase.slotData);
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
            Model.currentHeroSave.ReactiveProperty
                .CombineLatest(Model.currentHeroSave.Value.level.ReactiveProperty, (heroSave, level)=>(herosave: heroSave, level))
                .CombineLatest(Model.currentHeroSave.Value.piece.ReactiveProperty, (level, piece) => (level.herosave, level.level, piece))
                .Subscribe(ChangeData)
                .AddTo(View.MainView);
            View.btnExit.onClick.AddListener(Exit);
            View.btnUpgrade.onClick.AddListener(Upgrade);

            View.AnimOpen();
        }
        
        void ActionAbilityCallBack(SlotBase<Ability> slotBase) {
            View.ShowAbilityInfor(slotBase);
        }

        private void ChangeData((HeroSave heroSave, int level, int piece) Value)
        {
            View.InitData(Model.currentHeroChoose, Model.currentHeroSave);
        }

        void Exit() {
            UIAnimation.BasicButton(View.btnExit.transform);
            View.AnimClose();
        }
        void Upgrade() {
            UIAnimation.BasicButton(View.btnUpgrade.transform);
            HeroManager.Instance.UpgradeHero();
        }

        UniTask IModalLifecycleEvent.Cleanup(Memory<object> args)
        {
            View.piecesProgress.ClearAnimation();
            return UniTask.CompletedTask;
        }
    }
}