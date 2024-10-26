using System;
using System.Collections;
using System.Collections.Generic;
using TW.Reactive.CustomComponent;
using TW.UGUI.Core.Modals;
using TW.UGUI.Core.Views;
using TW.Utility.DesignPattern;
using UnityEngine;

public class HeroManager : Singleton<HeroManager>
{
    [field: SerializeField] public List<ReactiveValue<HeroSave>> heroSaves { get; set; } = new();
    [field: SerializeField] public ReactiveValue<HeroConfigData> currentHeroChoose { get; set; } = new();
    [field: SerializeField] public ReactiveValue<HeroSave> currentHeroSave { get; set; } = new();

    private void Start()
    {
        LoadData();
    }

    void LoadData() {
        heroSaves = InGameDataManager.Instance.InGameData.heroDataSave.heroSaves;
    }

    public bool IsHaveHero(string heroName)
    {
        for (int i = 0; i < heroSaves.Count; i++)
        {
            if (heroSaves[i].Value.heroName == heroName)
                return heroSaves[i].Value.level > 0;
        }
        return false;
    }

    public void ChooseHero(HeroConfigData heroConfigData) {
        currentHeroChoose.Value = heroConfigData;
        currentHeroSave.Value = GetHeroSaveData(heroConfigData.Name);
        ViewOptions options = new ViewOptions(nameof(ModalHeroInfor));
        ModalContainer.Find(ContainerKey.Modals).PushAsync(options);
    }


    public void UpgradeHero() {
        currentHeroSave.Value.UpgradeHero();
        InGameDataManager.Instance.SaveData();
    }

    public HeroSave GetHeroSaveData(string heroName)
    {
        for (int i = 0; i < heroSaves.Count; i++)
        {
            if (heroSaves[i].Value.heroName == heroName)
                return heroSaves[i].Value;
        }
        HeroSave hero = new();
        hero.heroName = heroName;
        hero.level = new(0);
        hero.piece = new(0);
        ReactiveValue<HeroSave> heroSave = new();
        heroSave.Value = hero;

        heroSaves.Add(heroSave);

        return hero;
    }

    public bool IsCanUpgradeHero(string heroName) {
        // for (int i = 0; i < heroSaves.Count; i++)
        // {
        //     if (heroSaves[i].Value.heroName == heroName)
        //         return BaseHeroGenerateGlobalConfig.Instance.GetHeroStatDataConfig(heroName, heroSaves[i].Value.piece) != null;
        // }
        return false;
    }

}
