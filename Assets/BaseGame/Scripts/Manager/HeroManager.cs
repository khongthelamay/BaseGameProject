using Sirenix.OdinInspector;
using System.Collections.Generic;
using TW.Reactive.CustomComponent;
using TW.UGUI.Core.Modals;
using TW.UGUI.Core.Views;
using TW.Utility.CustomType;
using TW.Utility.DesignPattern;
using UnityEngine;

public class HeroManager : Singleton<HeroManager>
{
    [field: SerializeField] public List<ReactiveValue<HeroSave>> heroSaves { get; set; } = new();
    [field: SerializeField] public ReactiveValue<HeroConfigData> currentHeroChoose { get; set; } = new();
    [field: SerializeField] public ReactiveValue<HeroSave> currentHeroSave { get; set; } = new();
    [field: SerializeField] public List<HeroConfigData> heroList { get; private set; }
    [field: SerializeField] public ReactiveValue<BigNumber> summonRecipe { get; private set; } = new(0);

    private void Start()
    {
        LoadData();
    }

    void LoadData() {
        heroSaves = InGameDataManager.Instance.InGameData.heroDataSave.heroSaves;
        heroList = HeroPoolGlobalConfig.Instance.HeroConfigDataList;
        summonRecipe = InGameDataManager.Instance.InGameData.playerResourceDataSave.GetResourceValue(ResourceType.SummonRecipe);
    }

    public bool IsHaveHero(string heroName)
    {
        for (int i = 0; i < heroSaves.Count; i++)
        {
            if (heroSaves[i].Value.heroName == heroName)
            {
                return heroSaves[i].Value.level.Value > 0;
            }
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

    [Button]
    public void AddPieces(string heroName, int amount){
        for (int i = 0; i < heroSaves.Count; i++)
        {
            if (heroSaves[i].Value.heroName == heroName) {
                heroSaves[i].Value.piece.Value += amount;
                InGameDataManager.Instance.SaveData();
                break;
            }   
        }
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
        for (int i = 0; i < heroSaves.Count; i++)
        {
            if (heroSaves[i].Value.heroName == heroName)
                return IsEnoughPieces(heroName, heroSaves[i].Value.piece.Value, heroSaves[i].Value.level.Value);
        }
        return false;
    }

    public bool IsEnoughPieces(string heroName, int pieces, int level) {
        return true;
    }

}
