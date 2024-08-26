using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using TW.Utility.CustomType;
using TW.Utility.Extension;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "HeroPoolGlobalConfig", menuName = "GlobalConfigs/HeroPoolGlobalConfig")]
[GlobalConfig("Assets/Resources/GlobalConfig/")]
public class HeroPoolGlobalConfig : GlobalConfig<HeroPoolGlobalConfig>
{
    [field: SerializeField] public List<Hero> HeroPrefabList { get; private set; }
    [field: SerializeField] public HeroPoolLevelConfig HeroPoolLevelConfig { get; private set; }

    private Dictionary<Hero.Rarity, List<Hero>> m_HeroPrefabDictionary;
    private Dictionary<Hero.Rarity, List<Hero>> HeroPrefabDictionary => m_HeroPrefabDictionary ??= InitHeroDictionary();

    public Hero GetHeroPrefab(string heroName)
    {
        foreach (Hero hero in HeroPrefabList)
        {
            if (hero.HeroStatData.Name == heroName)
            {
                return hero;
            }
        }

        return null;
    }

    public Hero GetRandomHeroPrefab(int poolLevel)
    {
        Hero.Rarity rarity = HeroPoolLevelConfig.ProbabilityRarity.GetRandomItem();
        return HeroPrefabDictionary[rarity].GetRandomElement();
    }

    private Dictionary<Hero.Rarity, List<Hero>> InitHeroDictionary()
    {
        Dictionary<Hero.Rarity, List<Hero>> heroDictionary = new Dictionary<Hero.Rarity, List<Hero>>();
        foreach (Hero.Rarity rarity in Enum.GetValues(typeof(Hero.Rarity)))
        {
            heroDictionary.Add(rarity, new List<Hero>());
        }

        foreach (Hero hero in HeroPrefabList)
        {
            heroDictionary[hero.HeroStatData.HeroRarity].Add(hero);
        }

        return heroDictionary;
    }
    public Hero GetRandomHeroUpgradePrefab(Hero.Rarity heroRarity)
    {
        return m_HeroPrefabDictionary[heroRarity].GetRandomElement();
    }

#if UNITY_EDITOR
    [Button]
    private void GetAllHeroPrefab()
    {
        EditorUtility.SetDirty(this);
        HeroPrefabList = AssetDatabase.FindAssets("t:Prefab", new string[] { "Assets/BaseGame/Prefabs/Hero" })
            .Select(AssetDatabase.GUIDToAssetPath)
            .Select(AssetDatabase.LoadAssetAtPath<Hero>)
            .ToList();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
#endif

}

[Serializable]
public class HeroPoolLevelConfig
{
    [field: SerializeField] public int Level { get; set; }
    [field: SerializeField] public Probability<Hero.Rarity> ProbabilityRarity { get; set; }
}