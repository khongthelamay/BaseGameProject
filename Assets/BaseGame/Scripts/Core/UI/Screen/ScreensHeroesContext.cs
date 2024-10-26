﻿using System;
using Cysharp.Threading.Tasks;
using TW.UGUI.MVPPattern;
using UnityEngine;
using R3;
using Sirenix.OdinInspector;
using TW.Reactive.CustomComponent;
using TW.UGUI.Core.Screens;
using System.Collections.Generic;
using Core;
using UnityEngine.UI;
using TMPro;
using TW.Utility.CustomType;

[Serializable]
public class ScreensHeroesContext 
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
        [field: SerializeField] public List<ReactiveValue<HeroSave>> heroSaves { get; set; } = new();
        [field: SerializeField] public ReactiveValue<HeroSave> currentHeroSave { get; set; } = new();
        [field: SerializeField] public ReactiveValue<Hero> currentHeroConfig { get; set; } = new();
        [field: SerializeField] public ReactiveValue<BigNumber> summonRecipe { get; set; } = new(0);
        public UniTask Initialize(Memory<object> args)
        {
            heroSaves = HeroManager.Instance.heroSaves;
            currentHeroSave = HeroManager.Instance.currentHeroSave;
            currentHeroConfig = HeroManager.Instance.currentHeroChoose;
            summonRecipe = HeroManager.Instance.summonRecipe;
            return UniTask.CompletedTask;
        }
    }

    [HideLabel]
    [Serializable]
    public class UIView : IAView
    {
        [field: Title(nameof(UIView))]
        [field: SerializeField] public CanvasGroup MainView { get; private set; }
        [field: SerializeField] public MainContentHeroes mainContentHeroes { get; private set; }
        [field: SerializeField] public UIResource resourceMythicPieces { get; private set; }
        [field: SerializeField] public Button btnGuardian { get; private set; }
        [field: SerializeField] public Button btnMythic { get; private set; }
        [field: SerializeField] public Button btnTier { get; private set; }
        [field: SerializeField] public Button btnLevel { get; private set; }

        public UniTask Initialize(Memory<object> args)
        {
            return UniTask.CompletedTask;
        }

        public void InitData() {
            mainContentHeroes.InitData(HeroPoolGlobalConfig.Instance.HeroPrefabList);
        }

        public void ReloadData(Hero heroData)
        {
            mainContentHeroes.ReloadData(heroData);
        }

        public void ChangeUpgradeMysthicPieces(string value) { resourceMythicPieces.ChangeValue(value); }

        public void FilterLevel()
        {
            mainContentHeroes.FilterLevel();
        }

        public void FilterTier()
        {
            mainContentHeroes.FilterTier();
        }

        public void SelectMythic()
        {
            mainContentHeroes.SelectMythic();
        }

        public void SelectGuardian()
        {
            mainContentHeroes.SelectGuardian();
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

            for (int i = 0; i < Model.heroSaves.Count; i++)
            {
                Model.heroSaves[i].ReactiveProperty
                    .CombineLatest(Model.heroSaves[i].Value.level.ReactiveProperty, (heroData, heroLevel) => (heroData, heroLevel))
                    .CombineLatest(Model.heroSaves[i].Value.piece.ReactiveProperty, (heroSave, heroPieces) => (heroSave.heroData, heroSave.heroLevel, heroPieces))
                    .Subscribe(ChangeData)
                    .AddTo(View.MainView);
            }
            View.mainContentHeroes.SetActionSlotCallBack(ActionSlotHeroCallBack);
            View.InitData();

            View.btnGuardian.onClick.AddListener(SelectGuardian);
            View.btnMythic.onClick.AddListener(SelectMythic);
            View.btnTier.onClick.AddListener(FilterTier);
            View.btnLevel.onClick.AddListener(FilterLevel);

            Model.summonRecipe.ReactiveProperty.Subscribe(ChangeUpgradeMysthicPieces);
        }

        private void FilterLevel()
        {
            View.FilterLevel();
        }

        private void FilterTier()
        {
            View.FilterTier();
        }

        private void SelectMythic()
        {
            View.SelectMythic();
        }

        private void SelectGuardian()
        {
            View.SelectGuardian();
        }

        private void ChangeData((HeroSave heroData, int heroLevel, int heroPieces) data)
        {
            View.ReloadData(Model.currentHeroConfig.Value);
        }

        void ActionSlotHeroCallBack(SlotBase<Hero> slotBase)
        {
            HeroManager.Instance.ChooseHero(slotBase.slotData);
        }

        void ChangeUpgradeMysthicPieces(BigNumber value) {
            View.ChangeUpgradeMysthicPieces(value.ToString());
        }

        public async UniTask Cleanup(Memory<object> args)
        {
            View.mainContentHeroes.CleanAnimation();
        }
    }
}