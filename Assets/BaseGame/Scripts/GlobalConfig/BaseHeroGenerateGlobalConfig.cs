using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using Spine.Unity;
using TW.Utility.Extension;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "BaseHeroGenerateGlobalConfig", menuName = "GlobalConfigs/BaseHeroGenerateGlobalConfig")]
[GlobalConfig("Assets/Resources/GlobalConfig/")]
public class BaseHeroGenerateGlobalConfig : GlobalConfig<BaseHeroGenerateGlobalConfig>
{
    [field: SerializeField] public List<HeroStatData> SheetHeroStatDataList { get; private set; }
    [field: SerializeField] public List<HeroStatData> HeroStatDataList { get; private set; }
    [field: SerializeField] public List<Hero> HeroPrefabList { get; private set; }
    [field: SerializeField] public Hero BaseHero { get; private set; }
    [field: SerializeField] public Projectile BaseProjectile { get; private set; }

    [field: SerializeField] public Sprite[] GraphicArray { get; private set; }
    [field: SerializeField] public Sprite[] RarityArray { get; private set; }
    [field: SerializeField] public Color[] RarityColorArray {get; private set;}

    [Button]
    public async UniTask GenerateHeroStatData()
    {
        string sheetId = "1-HkinUwSW4A4SkuiLGtl0Tm8771jFPVZB5ZpLs5pxz4";

        List<HeroStatData> heroStatDataList = AssetDatabase.FindAssets("t:HeroStatData")
            .Select(AssetDatabase.GUIDToAssetPath)
            .Select(AssetDatabase.LoadAssetAtPath<HeroStatData>)
            .ToList();
        
        SheetHeroStatDataList = new List<HeroStatData>();
        string result = await ABakingSheet.GetCsv(sheetId, "UnitAbility");
        List<Dictionary<string, string>> allData = ACsvReader.ReadDataFromString(result);
        HeroStatData heroStatData = null;
        foreach (Dictionary<string, string> data in allData)
        {
            if (!data["ID"].IsNullOrWhitespace())
            {
                Debug.Log(data["ID"]);
                heroStatData = heroStatDataList.FirstOrDefault(d => d.Name == data["Name"]);
                if (heroStatData == null)
                {
                    heroStatData = CreateInstance<HeroStatData>();
                    heroStatData.Name = data["Name"];
                    heroStatData.name = data["Name"];
                    AssetDatabase.CreateAsset(heroStatData, $"Assets/BaseGame/ScriptableObjects/HeroStatData/{heroStatData.Name}.asset");
                    AssetDatabase.SaveAssets();
                    
                    heroStatData = AssetDatabase.LoadAssetAtPath<HeroStatData>($"Assets/BaseGame/ScriptableObjects/HeroStatData/{heroStatData.Name}.asset");
                    
                }
                EditorUtility.SetDirty(heroStatData);
                List<Hero.Job> jobList = new List<Hero.Job>();
                if (Enum.TryParse(typeof(Hero.Job), data["MainRace"], out object job1))
                {
                    jobList.Add((Hero.Job) job1);
                }
                if (Enum.TryParse(typeof(Hero.Job), data["SubRace1"], out object job2))
                {
                    jobList.Add((Hero.Job) job2);
                }
                if (Enum.TryParse(typeof(Hero.Job), data["SubRace2"], out object job3))
                {
                    
                    jobList.Add((Hero.Job) job3);
                }
                heroStatData.HeroJob = jobList.ToArray();
                if (Enum.TryParse(typeof(Hero.Class), data["Class"], out object class1))
                {
                    heroStatData.HeroClass = (Hero.Class) class1;
                }

                if (int.TryParse(data["Tier"], out int tier))
                {
                    heroStatData.HeroRarity = (Hero.Rarity) (tier-1);
                }
                if (int.TryParse(data["AttackDamage"], out int attackDamage))
                {
                    heroStatData.BaseAttackDamage = attackDamage;
                }
                if (float.TryParse(data["AttackSpeed"], out float attackSpeed))
                {
                    heroStatData.BaseAttackSpeed = attackSpeed;
                }
                if (float.TryParse(data["AttackRange"], out float attackRange))
                {
                    heroStatData.BaseAttackRange = attackRange;
                }
                if (float.TryParse(data["UpgradePercentage"], out float upgradePercentage))
                {
                    heroStatData.UpgradePercentage = upgradePercentage;
                }

                try
                {
                    heroStatData.HeroSkeletonDataAsset = AssetDatabase.LoadAssetAtPath<SkeletonDataAsset>(
                        AssetDatabase.GUIDToAssetPath(
                            AssetDatabase.FindAssets($"t:SkeletonDataAsset {data["Name"]}_SkeletonData")[0]));
                }
                catch (Exception e)
                {
                    Debug.Log($"SkeletonDataAsset {data["Name"]} not found");
                }
                List<Ability> abilities = new List<Ability>();
                NormalAttackAbility normalAttackAbility = null;
                try
                {
                    normalAttackAbility = AssetDatabase.LoadAssetAtPath<NormalAttackAbility>(
                        AssetDatabase.GUIDToAssetPath(
                            AssetDatabase.FindAssets($"t:NormalAttackAbility {data["Name"]}_NormalAttackAbility")[0]));
                }
                catch (Exception e)
                {
                    normalAttackAbility = CreateInstance<NormalAttackAbility>();
                    normalAttackAbility.name = $"{data["Name"]}_NormalAttackAbility";
                    AssetDatabase.CreateAsset(normalAttackAbility, $"Assets/BaseGame/ScriptableObjects/Ability/{data["Name"]}_NormalAttackAbility.asset");
                    AssetDatabase.SaveAssets();
                    
                    normalAttackAbility = AssetDatabase.LoadAssetAtPath<NormalAttackAbility>($"Assets/BaseGame/ScriptableObjects/Ability/{data["Name"]}_NormalAttackAbility.asset");
                }
                EditorUtility.SetDirty(normalAttackAbility);
                if (heroStatData.HeroClass == Hero.Class.Range)
                {
                    Projectile projectile = null;
                    try
                    {
                        projectile = AssetDatabase.LoadAssetAtPath<Projectile>(
                            AssetDatabase.GUIDToAssetPath(
                                AssetDatabase.FindAssets($"t:Prefab {data["Name"]}_NormalAttackProjectile")[0]));
                    }
                    catch (Exception e)
                    {
                        projectile = Instantiate(BaseProjectile);
                        projectile.name = $"{data["Name"]}_NormalAttackProjectile";
                        PrefabUtility.SaveAsPrefabAsset(projectile.gameObject, $"Assets/BaseGame/Prefabs/Projectile/{data["Name"]}_NormalAttackProjectile.prefab");
                        AssetDatabase.SaveAssets();
                        DestroyImmediate(projectile.gameObject);
                        
                        projectile = AssetDatabase.LoadAssetAtPath<Projectile>($"Assets/BaseGame/Prefabs/Projectile/{data["Name"]}_NormalAttackProjectile.prefab");
                    }
                    normalAttackAbility.Projectile = projectile;
                }
                normalAttackAbility.AbilityTarget = Ability.Target.EnemyInRange;
                normalAttackAbility.AbilityTrigger = Ability.Trigger.NormalAttack;
                abilities.Add(normalAttackAbility);
                
                heroStatData.HeroAbilities = abilities;
            }
        }

        SheetHeroStatDataList = AssetDatabase.FindAssets("t:HeroStatData")
            .Select(AssetDatabase.GUIDToAssetPath)
            .Select(AssetDatabase.LoadAssetAtPath<HeroStatData>)
            .Where(hsd => hsd.HeroSkeletonDataAsset != null)
            .ToList();
        
        HeroPrefabList = AssetDatabase.FindAssets("t:Prefab", new string[] { "Assets/BaseGame/Prefabs/Hero" })
            .Select(AssetDatabase.GUIDToAssetPath)
            .Select(AssetDatabase.LoadAssetAtPath<Hero>)
            .ToList();
        
        
        SheetHeroStatDataList.ForEach(hsd =>
        {
            if (HeroPrefabList.Any(p => p.HeroStatData == hsd)) return;
            Hero hero = Instantiate(BaseHero);
            hero.HeroStatData = hsd;
            hero.name = hero.HeroStatData.Name;
            hero.InitHero();
            PrefabUtility.SaveAsPrefabAsset(hero.gameObject, $"Assets/BaseGame/Prefabs/Hero/{hero.name}.prefab");
            AssetDatabase.SaveAssets();

            DestroyImmediate(hero.gameObject);
        });
        
        HeroPrefabList = AssetDatabase.FindAssets("t:Prefab", new string[] { "Assets/BaseGame/Prefabs/Hero" })
            .Select(AssetDatabase.GUIDToAssetPath)
            .Select(AssetDatabase.LoadAssetAtPath<Hero>)
            .Where(h => h.HeroStatData.HeroSkeletonDataAsset != null)
            .ToList();

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        HeroPoolGlobalConfig.Instance.GetAllHeroPrefab();
        
    }
    
    public void GenerateAllHeroStatData()
    {
#if UNITY_EDITOR
        EditorUtility.SetDirty(this);
        HeroStatDataList = AssetDatabase.FindAssets("t:HeroStatData")
            .Select(AssetDatabase.GUIDToAssetPath)
            .Select(AssetDatabase.LoadAssetAtPath<HeroStatData>)
            .ToList();


        HeroPrefabList = AssetDatabase.FindAssets("t:Prefab", new string[] { "Assets/BaseGame/Prefabs/Hero" })
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

        HeroPrefabList = AssetDatabase.FindAssets("t:Prefab", new string[] { "Assets/BaseGame/Prefabs/Hero" })
            .Select(AssetDatabase.GUIDToAssetPath)
            .Select(AssetDatabase.LoadAssetAtPath<Hero>)
            .ToList();

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        HeroPoolGlobalConfig.Instance.GetAllHeroPrefab();
#endif
    }
}