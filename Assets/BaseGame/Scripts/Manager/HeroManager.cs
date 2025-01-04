using Sirenix.OdinInspector;
using System.Collections.Generic;
using Core;
using TW.Reactive.CustomComponent;
using TW.UGUI.Core.Modals;
using TW.UGUI.Core.Views;
using TW.Utility.CustomType;
using TW.Utility.DesignPattern;
using UnityEngine;

public class HeroManager : Singleton<HeroManager>
{
    [field: SerializeField] public List<ReactiveValue<HeroSave>> HeroSaves { get; set; } = new();
    [field: SerializeField] public ReactiveValue<HeroConfigData> CurrentHeroChoose { get; set; } = new();
    [field: SerializeField] public ReactiveValue<HeroSave> CurrentHeroSave { get; set; } = new();
    [field: SerializeField] public List<HeroConfigData> HeroList { get; private set; }
    [field: SerializeField] public ReactiveValue<BigNumber> SummonRecipe { get; private set; } = new(0);

    private void Start()
    {
        LoadData();
    }

    void LoadData() {
        HeroSaves = InGameDataManager.Instance.InGameData.heroDataSave.heroSaves;
        HeroList = HeroPoolGlobalConfig.Instance.HeroConfigDataList;
        //summonRecipe = InGameDataManager.Instance.InGameData.playerResourceDataSave.GetResourceValue(ResourceType.SummonRecipe);

        if (HeroSaves.Count == 0)
            InitHeroFirstData();
    }

    void InitHeroFirstData()
    {
        foreach (HeroConfigData heroConfig in HeroList)
        {
            if (heroConfig.HeroRarity != Hero.Rarity.Mythic)
            {
                AddHero(heroConfig.Name, 1);
            }
        }
    }

    ReactiveValue<HeroSave> AddHero(string heroName, int levelInit = 0)
    {
        HeroSave hero = new();
        hero.heroName = heroName;
        hero.level = new(levelInit);
        hero.piece = new(0);
        ReactiveValue<HeroSave> heroSave = new();
        heroSave.Value = hero;

        HeroSaves.Add(heroSave);
        return heroSave;
    }

    public bool IsHaveHero(string heroName)
    {
        for (int i = 0; i < HeroSaves.Count; i++)
        {
            if (HeroSaves[i].Value.heroName == heroName)
            {
                return HeroSaves[i].Value.level.Value > 0;
            }
        }
        return false;
    }
    public void ChooseHero(HeroConfigData heroConfigData) {
        CurrentHeroChoose.Value = heroConfigData;
        CurrentHeroSave.Value = GetHeroSaveData(heroConfigData.Name);
        ViewOptions options = new ViewOptions(nameof(ModalHeroInfor));
        ModalContainer.Find(ContainerKey.Modals).PushAsync(options);
    }


    public void UpgradeHero() {
        CurrentHeroSave.Value.UpgradeHero(10);
        InGameDataManager.Instance.SaveData();
    }

    [Button]
    public void AddPieces(string heroName, int amount){
        for (int i = 0; i < HeroSaves.Count; i++)
        {
            if (HeroSaves[i].Value.heroName == heroName) {
                HeroSaves[i].Value.piece.Value += amount;
                InGameDataManager.Instance.SaveData();
                break;
            }   
        }
    }

    public HeroSave GetHeroSaveData(string heroName)
    {
        for (int i = 0; i < HeroSaves.Count; i++)
        {
            if (HeroSaves[i].Value.heroName == heroName)
                return HeroSaves[i].Value;
        }
        return AddHero(heroName);
    }

    public bool IsCanUpgradeHero(string heroName) {
        for (int i = 0; i < HeroSaves.Count; i++)
        {
            if (HeroSaves[i].Value.heroName == heroName)
                return IsEnoughPieces(heroName, HeroSaves[i].Value.piece.Value, HeroSaves[i].Value.level.Value);
        }
        return false;
    }

    public bool IsEnoughPieces(string heroName, int pieces, int level) {
        return pieces >= 10;
    }

    public HeroConfigData GetHeroConfigData(string heroName) {
        foreach (HeroConfigData hero in HeroList)
        {
            if (hero.Name == heroName) return hero;
        }
        return null;
    }

    public bool CurrentHeroAbilityIsUnlock(Ability data)
    {
        return CurrentHeroSave.Value.level.Value >= data.LevelUnlock;
    }
}
