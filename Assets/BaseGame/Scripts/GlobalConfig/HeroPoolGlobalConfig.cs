using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using TW.Utility.CustomType;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "HeroPoolGlobalConfig", menuName = "GlobalConfigs/HeroPoolGlobalConfig")]
[GlobalConfig("Assets/Resources/GlobalConfig/")]
public class HeroPoolGlobalConfig : GlobalConfig<HeroPoolGlobalConfig>
{
    [field: SerializeField] public List<Hero> HeroPrefabList {get; private set;}
    [field: SerializeField] public HeroPoolLevelConfig HeroPoolLevelConfig {get; private set;}

    private Hero[,,] _heroPrefabArray;
    private Hero[,,] HeroPrefabArray => _heroPrefabArray ??= InitHeroArray();
    public Hero GetHeroPrefab(Hero.Rarity rarity, Hero.Trait trait, Hero.Race race)
    {
        return HeroPrefabArray[(int)rarity, (int)trait, (int)race];
    }
    public Hero GetRandomHeroPrefab(int poolLevel)
    {
        Hero.Rarity rarity = HeroPoolLevelConfig.ProbabilityRarity.GetRandomItem();
        Hero.Trait trait = (Hero.Trait)UnityEngine.Random.Range(0, 4);
        Hero.Race race = (Hero.Race)UnityEngine.Random.Range(0, 4);

        return HeroPrefabArray[(int)rarity, (int)trait, (int)race];
    }
    
    private Hero[,,] InitHeroArray()
    {
        Hero[,,] heroArray = new Hero[5, 4, 4];
        foreach (var hero in HeroPrefabList)
        {
            heroArray[(int)hero.HeroStatData.HeroRarity, (int)hero.HeroStatData.HeroTrait, (int)hero.HeroStatData.HeroRace] = hero;
        }
        return heroArray;
    }
    
#if UNITY_EDITOR
    [Button]
    private void GetAllHeroPrefab()
    {
        HeroPrefabList = AssetDatabase.FindAssets("t:Prefab", new string[] {"Assets/BaseGame/Prefabs/Hero"})
            .Select(AssetDatabase.GUIDToAssetPath)
            .Select(AssetDatabase.LoadAssetAtPath<Hero>)
            .ToList();
    }
#endif
}

[Serializable]
public class HeroPoolLevelConfig
{
    [field: SerializeField] public int Level { get; set; }
    [field: SerializeField] public Probability<Hero.Rarity> ProbabilityRarity { get; set; }
}

