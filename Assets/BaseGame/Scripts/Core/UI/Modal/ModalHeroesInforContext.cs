using System;
using Cysharp.Threading.Tasks;
using TW.UGUI.MVPPattern;
using UnityEngine;
using R3;
using Sirenix.OdinInspector;
using TW.Reactive.CustomComponent;
using UGUI.Core.Modals;
using TMPro;
using UnityEngine.UI;
using TW.UGUI.Core.Modals;
using Core;
using Spine.Unity;
using System.Collections.Generic;

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
        [field: SerializeField] public ReactiveValue<Hero> currentHeroChoose { get; private set; }

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
        [field: SerializeField] public SkeletonGraphic imgHeroIcon {get; private set;}  
        [field: SerializeField] public Button btnExit {get; private set;}  
        [field: SerializeField] public Button btnUpgrade {get; private set;}  
        [field: SerializeField] public MainContentAbility mainContentHeroAbility { get; private set;}  
        [field: SerializeField] public MainContentHeroJob mainContentHeroJob {get; private set;}

        List<Hero.Job> jobs;

        public UniTask Initialize(Memory<object> args)
        {
            return UniTask.CompletedTask;
        }

        public void InitData(Hero heroData, HeroSave heroDataSave) {
            imgHeroIcon.skeletonDataAsset = heroData.HeroStatData.HeroSkeletonDataAsset;
            imgHeroIcon.Initialize(true);
            imgHeroIcon.AnimationState.SetAnimation(0, "Idle", true);
            txtLevel.text = "Lv. " + heroDataSave.level.Value.ToString();
            txtName.text = heroData.HeroStatData.Name;
            txtAtk.text = heroData.HeroStatData.BaseAttackDamage.ToString();
            txtSpeed.text = heroData.HeroStatData.BaseAttackSpeed.ToString();
            piecesProgress.ChangeProgress(0);
            piecesProgress.ChangeTextProgress("0/0");
            mainContentHeroAbility.InitData(heroData.HeroStatData.HeroAbilities);
            jobs = new(heroData.HeroStatData.HeroJob);
            mainContentHeroJob.InitData(jobs);
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

            Model.currentHeroSave.ReactiveProperty
                .CombineLatest(Model.currentHeroSave.Value.level.ReactiveProperty, (herosave, level)=>(herosave, level))
                .CombineLatest(Model.currentHeroSave.Value.piece.ReactiveProperty, (herosave, piece) => (herosave, piece))
                .Subscribe(ChangeData)
                .AddTo(View.MainView);

            View.btnExit.onClick.AddListener(Exit);
            View.btnUpgrade.onClick.AddListener(Upgrade);

            View.AnimOpen();
        }

        private void ChangeData(((HeroSave herosave, int level) herosave, int piece) obj)
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
    }
}