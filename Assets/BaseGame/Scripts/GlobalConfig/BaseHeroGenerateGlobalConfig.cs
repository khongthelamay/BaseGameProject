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
    [field: SerializeField] public List<HeroStatData> LisHeroStatData {get; private set;}
    
    [field: SerializeField] public Sprite[] ShapeArray {get; private set;}
    [field: SerializeField] public Sprite[] NumberArray {get; private set;}
    [field: SerializeField] public Color[] ColorArray {get; private set;}
    
    [Button]
    public void GenerateAllHeroStatData()
    {
#if UNITY_EDITOR
        EditorUtility.SetDirty(this);
        LisHeroStatData = AssetDatabase.FindAssets("t:HeroStatData")
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
                    if (LisHeroStatData.Any(d => d.HeroRarity == rarity && d.HeroTrait == trait && d.HeroRace == race)) return;
                    HeroStatData heroStatData = CreateInstance<HeroStatData>();
                    heroStatData.Name = $"{rarity}_{trait}_{race}";
                    heroStatData.HeroRarity = rarity;
                    heroStatData.HeroTrait = trait;
                    heroStatData.HeroRace = race;
                    
                    
                    LisHeroStatData.Add(heroStatData);
                    AssetDatabase.CreateAsset(heroStatData, $"Assets/BaseGame/ScriptableObjects/HeroStatData/{heroStatData.Name}.asset");
                    AssetDatabase.SaveAssets();
                    
                });
            });
        });

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
#endif
    }
    
}
