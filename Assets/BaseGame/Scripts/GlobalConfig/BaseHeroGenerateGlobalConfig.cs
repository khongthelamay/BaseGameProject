using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    [field: SerializeField] public List<HeroStatData> HeroStatDataList {get; private set;}
    [field: SerializeField] public List<Hero> HeroPrefabList {get; private set;}
    [field: SerializeField] public Hero BaseHero {get; private set;}

    [field: SerializeField] public Sprite[] ShapeArray {get; private set;}
    [field: SerializeField] public Sprite[] NumberArray {get; private set;}
    [field: SerializeField] public Color[] ColorArray {get; private set;}
    
    [Button]
    public void GenerateAllHeroStatData()
    {
#if UNITY_EDITOR
        EditorUtility.SetDirty(this);
        HeroStatDataList = AssetDatabase.FindAssets("t:HeroStatData")
            .Select(AssetDatabase.GUIDToAssetPath)
            .Select(AssetDatabase.LoadAssetAtPath<HeroStatData>)
            .ToList();
        Hero.Rarity[] rarities = (Hero.Rarity[])System.Enum.GetValues(typeof(Hero.Rarity));
        Hero.Trait[] traits = (Hero.Trait[])System.Enum.GetValues(typeof(Hero.Trait));
        Hero.Race[] races = (Hero.Race[])System.Enum.GetValues(typeof(Hero.Race));
        rarities.ForEach(rarity =>
        {
            traits.ForEach(trait =>
            {
                races.ForEach(race =>
                {
                    if (HeroStatDataList.Any(d => d.HeroRarity == rarity && d.HeroTrait == trait && d.HeroRace == race)) return;
                    HeroStatData heroStatData = CreateInstance<HeroStatData>();
                    heroStatData.Name = $"{rarity}_{trait}_{race}";
                    heroStatData.HeroRarity = rarity;
                    heroStatData.HeroTrait = trait;
                    heroStatData.HeroRace = race;
                    
                    
                    HeroStatDataList.Add(heroStatData);
                    AssetDatabase.CreateAsset(heroStatData, $"Assets/BaseGame/ScriptableObjects/HeroStatData/{heroStatData.Name}.asset");
                    AssetDatabase.SaveAssets();
                    
                });
            });
        });
        
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

            PrefabUtility.SaveAsPrefabAsset(hero.gameObject, $"Assets/BaseGame/Prefabs/Hero/{hero.name}.prefab");
            AssetDatabase.SaveAssets();
            
            DestroyImmediate(hero.gameObject);
        });
        // AssetDatabase.Refresh();
        HeroPrefabList = AssetDatabase.FindAssets("t:Prefab", new string[] {"Assets/BaseGame/Prefabs/Hero"})
            .Select(AssetDatabase.GUIDToAssetPath)
            .Select(AssetDatabase.LoadAssetAtPath<Hero>)
            .ToList();
        
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
#endif
    }
    
}
