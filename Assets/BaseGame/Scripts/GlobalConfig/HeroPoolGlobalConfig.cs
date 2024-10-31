using System;
using System.Collections.Generic;
using System.Linq;
using Core;
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
    [field: SerializeField] public List<HeroConfigData> HeroConfigDataList { get; private set; }
    [field: SerializeField] public HeroPoolLevelConfig HeroPoolLevelConfig { get; private set; }

    private Dictionary<Hero.Rarity, List<HeroConfigData>> HeroPrefabDictionaryCache { get; set; }
    private Dictionary<Hero.Rarity, List<HeroConfigData>> HeroPrefabDictionary => HeroPrefabDictionaryCache ??= InitHeroDictionary();
    

    public HeroConfigData GetRandomHeroPrefab(int poolLevel)
    {
        return HeroConfigDataList.GetRandomElement();
    }

    private Dictionary<Hero.Rarity, List<HeroConfigData>> InitHeroDictionary()
    {
        Dictionary<Hero.Rarity, List<HeroConfigData>> heroDictionary = new Dictionary<Hero.Rarity, List<HeroConfigData>>();
        foreach (Hero.Rarity rarity in Enum.GetValues(typeof(Hero.Rarity)))
        {
            heroDictionary.Add(rarity, new List<HeroConfigData>());
        }

        foreach (HeroConfigData heroStatData in HeroConfigDataList)
        {
            heroDictionary[heroStatData.HeroRarity].Add(heroStatData);
        }

        return heroDictionary;
    }
    public HeroConfigData GetRandomHeroConfigDataUpgrade(Hero.Rarity heroRarity)
    {
        return HeroPrefabDictionary[heroRarity].GetRandomElement();
    }

#if UNITY_EDITOR
    [Button]
    public void GetAllHeroPrefab()
    {
        EditorUtility.SetDirty(this);
        HeroConfigDataList = AssetDatabase.FindAssets("t:HeroConfigData")
            .Select(AssetDatabase.GUIDToAssetPath)
            .Select(AssetDatabase.LoadAssetAtPath<HeroConfigData>)
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