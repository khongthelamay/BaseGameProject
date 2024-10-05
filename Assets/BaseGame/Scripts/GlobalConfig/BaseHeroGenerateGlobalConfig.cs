using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "BaseHeroGenerateGlobalConfig", menuName = "GlobalConfigs/BaseHeroGenerateGlobalConfig")]
[GlobalConfig("Assets/Resources/GlobalConfig/")]
public class BaseHeroGenerateGlobalConfig : GlobalConfig<BaseHeroGenerateGlobalConfig>
{
    [field: SerializeField] public List<HeroStatData> SheetHeroStatDataList {get; private set;}
    [field: SerializeField] public List<HeroStatData> HeroStatDataList {get; private set;}
    [field: SerializeField] public List<Hero> HeroPrefabList {get; private set;}
    [field: SerializeField] public Hero BaseHero {get; private set;}

    [field: SerializeField] public Sprite[] GraphicArray {get; private set;}
    [field: SerializeField] public Sprite[] RarityArray {get; private set;}
    
    [Button]
    public void GenerateHeroStatData()
    {
        SheetHeroStatDataList = AssetDatabase.FindAssets("t:HeroStatData")
            .Select(AssetDatabase.GUIDToAssetPath)
            .Select(AssetDatabase.LoadAssetAtPath<HeroStatData>)
            .ToList();
    }

    [Button]
    public void GenerateAllHeroStatData()
    {
#if UNITY_EDITOR
        EditorUtility.SetDirty(this);
        HeroStatDataList = AssetDatabase.FindAssets("t:HeroStatData")
            .Select(AssetDatabase.GUIDToAssetPath)
            .Select(AssetDatabase.LoadAssetAtPath<HeroStatData>)
            .ToList();
        
        
        HeroPrefabList = AssetDatabase.FindAssets("t:Prefab", new string[] {"Assets/BaseGame/Prefabs/Hero"})
            .Select(AssetDatabase.GUIDToAssetPath)
            .Select(AssetDatabase.LoadAssetAtPath<Hero>)
            .ToList();
        HeroStatDataList.ForEach(heroStatData =>
        {
            if (HeroPrefabList.Any(p => p.HeroStatData == heroStatData)) return;
            Hero hero = Instantiate(BaseHero);
            hero.HeroStatData = heroStatData;
            hero.name = hero.HeroStatData.Name;
            hero.InitHero();
            PrefabUtility.SaveAsPrefabAsset(hero.gameObject, $"Assets/BaseGame/Prefabs/Hero/{hero.name}.prefab");
            AssetDatabase.SaveAssets();
            
            DestroyImmediate(hero.gameObject);
        });

        HeroPrefabList = AssetDatabase.FindAssets("t:Prefab", new string[] {"Assets/BaseGame/Prefabs/Hero"})
            .Select(AssetDatabase.GUIDToAssetPath)
            .Select(AssetDatabase.LoadAssetAtPath<Hero>)
            .ToList();
        
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        HeroPoolGlobalConfig.Instance.GetAllHeroPrefab();
#endif
    }
    
}
