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
using UnityEditor.Animations;
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
    [field: SerializeField] public Color[] RarityColorArray { get; private set; }

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
        List<Dictionary<string, string>> csvData = ACsvReader.ReadDataFromString(result);
        HeroStatData heroStatData = null;
        for (var i = 0; i < csvData.Count; i++)
        {
            Dictionary<string, string> data = csvData[i];
            data = data.ToDictionary(d => d.Key, d => d.Value.Replace(" ", ""));
            if (data["ID"].IsNullOrWhitespace()) continue;
            heroStatData = heroStatDataList.FirstOrDefault(d => d.Name == data["Name"]);
            if (heroStatData == null)
            {
                heroStatData = CreateInstance<HeroStatData>();
                heroStatData.Name = data["Name"];
                heroStatData.name = data["Name"];
                AssetDatabase.CreateAsset(heroStatData,
                    $"Assets/BaseGame/ScriptableObjects/HeroStatData/{heroStatData.Name}.asset");
                AssetDatabase.SaveAssets();
                heroStatData =
                    AssetDatabase.LoadAssetAtPath<HeroStatData>(
                        $"Assets/BaseGame/ScriptableObjects/HeroStatData/{heroStatData.Name}.asset");
            }

            EditorUtility.SetDirty(heroStatData);
            GenerateBaseData(data, heroStatData);
            
            EditorUtility.SetDirty(heroStatData);
            heroStatData.HeroAbilities = new List<Ability>();

            NormalAttackAbility normalAttackAbility = GenerateNormalAttackAbility(data, heroStatData);
            heroStatData.HeroAbilities.Add(normalAttackAbility);

            for (int j = i; j < csvData.Count; j++)
            {
                if (csvData[j]["AbilityName"].IsNullOrWhitespace()) break;
                Ability ability = GenerateAbility(csvData[j], heroStatData);
                heroStatData.HeroAbilities.Add(ability);
            }

            heroStatData.HeroAbilities = heroStatData.HeroAbilities.Where(a => a != null).ToList();
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

    private static void GenerateBaseData(Dictionary<string, string> data, HeroStatData heroStatData)
    {
        GenerateHeroJob(data, heroStatData);
        GenerateHeroClass(data, heroStatData);
        GenerateBaseStat(data, heroStatData);
        GenerateSkeletonData(data, heroStatData);
        GenerateIdleAnimData(data, heroStatData);
        GenerateAttackAnimData(data, heroStatData);
        GenerateSkillAnimData(data, heroStatData);
        GenerateAnimatorData(data, heroStatData);
    }

    private static void GenerateSkeletonData(Dictionary<string, string> data, HeroStatData heroStatData)
    {
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
    }
    private static void GenerateIdleAnimData(Dictionary<string, string> data, HeroStatData heroStatData)
    {
        try
        {
            EditorUtility.SetDirty(heroStatData);
            heroStatData.HeroSprite = AssetDatabase.LoadAssetAtPath<Sprite>(
                AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets($"t:Sprite {data["Name"]}-Idle")[0]));
        }
        catch (Exception e)
        {
            UnityEngine.Debug.Log(e);
        }

        AnimationClip idleAnim;
        try
        {
            string animIdleGuid = AssetDatabase.FindAssets($"t:AnimationClip {data["Name"]}-Idle-Animation")[0];
            idleAnim = AssetDatabase.LoadAssetAtPath<AnimationClip>(AssetDatabase.GUIDToAssetPath(animIdleGuid));
        }
        catch (Exception e)
        {
            idleAnim = null;
        }

        string[] spriteAnim = AssetDatabase.FindAssets($"t:Sprite {data["Name"]}-Idle");
        List<Sprite> spriteList = spriteAnim.Select(AssetDatabase.GUIDToAssetPath)
            .Select(AssetDatabase.LoadAssetAtPath<Sprite>)
            .ToList();
        if (idleAnim == null)
        {
            if (spriteAnim.Length == 0) return;
            idleAnim = new AnimationClip
            {
                name = $"{data["Name"]}-Idle-Animation",
                frameRate = 30
            };
            if (!AssetDatabase.IsValidFolder($"Assets/BaseGame/Animations/Hero/{data["Name"]}"))
            {
                AssetDatabase.CreateFolder("Assets/BaseGame/Animations/Hero", data["Name"]);
            }

            AssetDatabase.CreateAsset(idleAnim,
                $"Assets/BaseGame/Animations/Hero/{data["Name"]}/{data["Name"]}-Idle-Animation.anim");
            AssetDatabase.SaveAssets();
        }
        else
        {
            idleAnim.ClearCurves();
        }


        EditorCurveBinding curveBinding = new EditorCurveBinding
        {
            type = typeof(SpriteRenderer),
            path = "",
            propertyName = "m_Sprite"
        };
        ObjectReferenceKeyframe[] keyFrames = new ObjectReferenceKeyframe[spriteList.Count];
        for (int i = 0; i < spriteList.Count; i++)
        {
            keyFrames[i] = new ObjectReferenceKeyframe
            {
                time = i / idleAnim.frameRate,
                value = spriteList[i]
            };
        }

        AnimationUtility.SetObjectReferenceCurve(idleAnim, curveBinding, keyFrames);

        idleAnim = AssetDatabase.LoadAssetAtPath<AnimationClip>(
            $"Assets/BaseGame/Animations/Hero/{data["Name"]}/{data["Name"]}-Idle-Animation.anim");
        // set loop animation
        AnimationClipSettings clipSettings = AnimationUtility.GetAnimationClipSettings(idleAnim);
        clipSettings.loopTime = true;
        AnimationUtility.SetAnimationClipSettings(idleAnim, clipSettings);
        
    }
    private static void GenerateAttackAnimData(Dictionary<string, string> data, HeroStatData heroStatData)
    {
        AnimationClip attackAnim;
        try
        {
            string animAttackGuid = AssetDatabase.FindAssets($"t:AnimationClip {data["Name"]}-Attack-Animation")[0];
            attackAnim = AssetDatabase.LoadAssetAtPath<AnimationClip>(AssetDatabase.GUIDToAssetPath(animAttackGuid));
        }
        catch (Exception e)
        {
            attackAnim = null;
        }

        string[] spriteAnim = AssetDatabase.FindAssets($"t:Sprite {data["Name"]}-Attack");
        List<Sprite> spriteList = spriteAnim.Select(AssetDatabase.GUIDToAssetPath)
            .Select(AssetDatabase.LoadAssetAtPath<Sprite>)
            .ToList();
        if (attackAnim == null)
        {
            if (spriteAnim.Length == 0) return;
            attackAnim = new AnimationClip
            {
                name = $"{data["Name"]}-Attack-Animation",
                frameRate = 30
            };
            if (!AssetDatabase.IsValidFolder($"Assets/BaseGame/Animations/Hero/{data["Name"]}"))
            {
                AssetDatabase.CreateFolder("Assets/BaseGame/Animations/Hero", data["Name"]);
            }

            AssetDatabase.CreateAsset(attackAnim,
                $"Assets/BaseGame/Animations/Hero/{data["Name"]}/{data["Name"]}-Attack-Animation.anim");
            AssetDatabase.SaveAssets();
        }
        else
        {
            attackAnim.ClearCurves();
        }

        EditorCurveBinding curveBinding = new EditorCurveBinding
        {
            type = typeof(SpriteRenderer),
            path = "",
            propertyName = "m_Sprite"
        };
        ObjectReferenceKeyframe[] keyFrames = new ObjectReferenceKeyframe[spriteList.Count];
        for (int i = 0; i < spriteList.Count; i++)
        {
            keyFrames[i] = new ObjectReferenceKeyframe
            {
                time = i / attackAnim.frameRate,
                value = spriteList[i]
            };
        }

        AnimationUtility.SetObjectReferenceCurve(attackAnim, curveBinding, keyFrames);

        attackAnim = AssetDatabase.LoadAssetAtPath<AnimationClip>(
            $"Assets/BaseGame/Animations/Hero/{data["Name"]}/{data["Name"]}-Attack-Animation.anim");
    }
    private static void GenerateSkillAnimData(Dictionary<string, string> data, HeroStatData heroStatData)
    {
        AnimationClip skillAnim;
        try
        {
            string animSkillGuid = AssetDatabase.FindAssets($"t:AnimationClip {data["Name"]}-Skill-Animation")[0];
            skillAnim = AssetDatabase.LoadAssetAtPath<AnimationClip>(AssetDatabase.GUIDToAssetPath(animSkillGuid));
        }
        catch (Exception e)
        {
            skillAnim = null;
        }

        string[] spriteAnim = AssetDatabase.FindAssets($"t:Sprite {data["Name"]}-Skill");
        List<Sprite> spriteList = spriteAnim.Select(AssetDatabase.GUIDToAssetPath)
            .Select(AssetDatabase.LoadAssetAtPath<Sprite>)
            .ToList();
        if (skillAnim == null)
        {
            if (spriteAnim.Length == 0) return;
            skillAnim = new AnimationClip
            {
                name = $"{data["Name"]}-Skill-Animation",
                frameRate = 30
            };
            if (!AssetDatabase.IsValidFolder($"Assets/BaseGame/Animations/Hero/{data["Name"]}"))
            {
                AssetDatabase.CreateFolder("Assets/BaseGame/Animations/Hero", data["Name"]);
            }

            AssetDatabase.CreateAsset(skillAnim,
                $"Assets/BaseGame/Animations/Hero/{data["Name"]}/{data["Name"]}-Skill-Animation.anim");
            AssetDatabase.SaveAssets();
        }
        else
        {
            skillAnim.ClearCurves();
        }

        EditorCurveBinding curveBinding = new EditorCurveBinding
        {
            type = typeof(SpriteRenderer),
            path = "",
            propertyName = "m_Sprite"
        };
        ObjectReferenceKeyframe[] keyFrames = new ObjectReferenceKeyframe[spriteList.Count];
        for (int i = 0; i < spriteList.Count; i++)
        {
            keyFrames[i] = new ObjectReferenceKeyframe
            {
                time = i / skillAnim.frameRate,
                value = spriteList[i]
            };
        }

        AnimationUtility.SetObjectReferenceCurve(skillAnim, curveBinding, keyFrames);

        skillAnim = AssetDatabase.LoadAssetAtPath<AnimationClip>(
            $"Assets/BaseGame/Animations/Hero/{data["Name"]}/{data["Name"]}-Skill-Animation.anim");
    }
    private static void GenerateAnimatorData(Dictionary<string, string> data, HeroStatData heroStatData)
    {
        AnimatorController animatorController;
        try
        {
            string animControllerGuid = AssetDatabase.FindAssets($"t:AnimatorController {data["Name"]}-AnimatorController", 
                new []{$"Assets/BaseGame/Animations/Hero/{data["Name"]}"})[0];
            animatorController = AssetDatabase.LoadAssetAtPath<AnimatorController>(AssetDatabase.GUIDToAssetPath(animControllerGuid));
        }
        catch (Exception e)
        {
            animatorController = null;
        }

        if (animatorController == null)
        {
            animatorController = new AnimatorController
            {
                name = $"{data["Name"]}-AnimatorController"
            };
            if (!AssetDatabase.IsValidFolder($"Assets/BaseGame/Animations/Hero/{data["Name"]}"))
            {
                AssetDatabase.CreateFolder("Assets/BaseGame/Animations/Hero", data["Name"]);
            }

            AssetDatabase.CreateAsset(animatorController,
                $"Assets/BaseGame/Animations/Hero/{data["Name"]}/{data["Name"]}-AnimatorController.controller");
            AssetDatabase.SaveAssets();
        }
        EditorUtility.SetDirty(heroStatData);
        heroStatData.AnimatorController = AssetDatabase.LoadAssetAtPath<AnimatorController>(
            $"Assets/BaseGame/Animations/Hero/{data["Name"]}/{data["Name"]}-AnimatorController.controller");
        animatorController.layers = Array.Empty<AnimatorControllerLayer>();
        animatorController.AddLayer("Base Layer");
        AnimatorStateMachine stateMachine = animatorController.layers[0].stateMachine;
        animatorController.parameters = Array.Empty<AnimatorControllerParameter>();
        animatorController.AddParameter("TickRate", AnimatorControllerParameterType.Float);
        animatorController.parameters[0].defaultFloat = 1;
        if (data["Name"] == "Mage") Debug.Log(animatorController.name);

        AnimationClip idleClip = AssetDatabase.LoadAssetAtPath<AnimationClip>(
            $"Assets/BaseGame/Animations/Hero/{data["Name"]}/{data["Name"]}-Idle-Animation.anim");
        AnimatorState idleState = stateMachine.AddState("Idle");
        idleState.motion = idleClip;
        
        AnimationClip attackClip = AssetDatabase.LoadAssetAtPath<AnimationClip>(
            $"Assets/BaseGame/Animations/Hero/{data["Name"]}/{data["Name"]}-Attack-Animation.anim");
        AnimatorState attackState = stateMachine.AddState("Attack");
        attackState.motion = attackClip;
        attackState.speedParameterActive = true;
        attackState.speedParameter = "TickRate";
        AnimatorStateTransition attackToIdle = attackState.AddTransition(idleState);
        attackToIdle.hasExitTime = true;
        attackToIdle.exitTime = 1;
        attackToIdle.hasFixedDuration = false;
        attackToIdle.duration = 0;
        attackToIdle.offset = 0;
        
        // attackToIdle;
        
        
        AnimationClip skillClip = AssetDatabase.LoadAssetAtPath<AnimationClip>(
            $"Assets/BaseGame/Animations/Hero/{data["Name"]}/{data["Name"]}-Skill-Animation.anim");
        if (skillClip != null)
        {
            AnimatorState skillState = stateMachine.AddState("Skill");
            skillState.motion = skillClip;
            skillState.speedParameterActive = true;
            skillState.speedParameter = "TickRate";
            AnimatorStateTransition skillToIdle = skillState.AddTransition(idleState);
            skillToIdle.hasExitTime = true;
            skillToIdle.exitTime = 1;
            skillToIdle.hasFixedDuration = false;
            skillToIdle.duration = 0;
            skillToIdle.offset = 0;
        }
    }
    private static void GenerateBaseStat(Dictionary<string, string> data, HeroStatData heroStatData)
    {
        if (int.TryParse(data["Tier"], out int tier))
        {
            heroStatData.HeroRarity = (Hero.Rarity)(tier - 1);
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
    }

    private static void GenerateHeroClass(Dictionary<string, string> data, HeroStatData heroStatData)
    {
        if (Enum.TryParse(typeof(Hero.Class), data["Class"], out object class1))
        {
            heroStatData.HeroClass = (Hero.Class)class1;
        }
    }

    private static void GenerateHeroJob(Dictionary<string, string> data, HeroStatData heroStatData)
    {
        List<Hero.Job> jobList = new List<Hero.Job>();
        if (Enum.TryParse(typeof(Hero.Job), data["MainRace"], out object job1))
        {
            jobList.Add((Hero.Job)job1);
        }

        if (Enum.TryParse(typeof(Hero.Job), data["SubRace1"], out object job2))
        {
            jobList.Add((Hero.Job)job2);
        }

        if (Enum.TryParse(typeof(Hero.Job), data["SubRace2"], out object job3))
        {
            jobList.Add((Hero.Job)job3);
        }

        heroStatData.HeroJob = jobList.ToArray();
    }

    private NormalAttackAbility GenerateNormalAttackAbility(Dictionary<string, string> data, HeroStatData heroStatData)
    {
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
            AssetDatabase.CreateAsset(normalAttackAbility,
                $"Assets/BaseGame/ScriptableObjects/Ability/{data["Name"]}_NormalAttackAbility.asset");
            AssetDatabase.SaveAssets();

            normalAttackAbility = AssetDatabase.LoadAssetAtPath<NormalAttackAbility>(
                $"Assets/BaseGame/ScriptableObjects/Ability/{data["Name"]}_NormalAttackAbility.asset");
        }

        EditorUtility.SetDirty(normalAttackAbility);
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
            PrefabUtility.SaveAsPrefabAsset(projectile.gameObject,
                $"Assets/BaseGame/Prefabs/Projectile/{data["Name"]}_NormalAttackProjectile.prefab");
            AssetDatabase.SaveAssets();
            DestroyImmediate(projectile.gameObject);

            projectile =
                AssetDatabase.LoadAssetAtPath<Projectile>(
                    $"Assets/BaseGame/Prefabs/Projectile/{data["Name"]}_NormalAttackProjectile.prefab");
        }

        normalAttackAbility.Projectile = projectile;
        normalAttackAbility.AbilityTarget = Ability.Target.EnemyInRange;
        normalAttackAbility.AbilityTrigger = Ability.Trigger.NormalAttack;
        return normalAttackAbility;
    }

    private Ability GenerateAbility(Dictionary<string, string> data, HeroStatData heroStatData)
    {
        if (data["ActiveType"] == "Passive")
        {
            return GeneratePassiveAbility(data, heroStatData);
        }

        if (data["ActiveType"] == "Active")
        {
            return GenerateActiveAbility(data, heroStatData);
        }

        Debug.Log($"Not found ability type {data["ActiveType"]}");
        return null;
    }

    private PassiveAbility GeneratePassiveAbility(Dictionary<string, string> data, HeroStatData heroStatData)
    {
        PassiveAbility passiveAbility = null;
        try
        {
            passiveAbility = AssetDatabase.LoadAssetAtPath<PassiveAbility>(
                AssetDatabase.GUIDToAssetPath(
                    AssetDatabase.FindAssets($"t:PassiveAbility {data["Name"]}_{data["AbilityName"]}")[0]));
        }
        catch (Exception e)
        {
            passiveAbility = CreateInstance<PassiveAbility>();
            passiveAbility.name = $"{data["Name"]}_{data["AbilityName"]}";
            AssetDatabase.CreateAsset(passiveAbility,
                $"Assets/BaseGame/ScriptableObjects/Ability/{data["Name"]}_{data["AbilityName"]}.asset");
            AssetDatabase.SaveAssets();

            passiveAbility = AssetDatabase.LoadAssetAtPath<PassiveAbility>(
                $"Assets/BaseGame/ScriptableObjects/Ability/{data["Name"]}_{data["AbilityName"]}.asset");
        }

        EditorUtility.SetDirty(passiveAbility);
        ProbabilityAttackAbilityBehavior probabilityAttackAbilityBehavior = new ProbabilityAttackAbilityBehavior();
        Projectile projectile = null;
        try
        {
            projectile = AssetDatabase.LoadAssetAtPath<Projectile>(
                AssetDatabase.GUIDToAssetPath(
                    AssetDatabase.FindAssets($"t:Prefab {heroStatData.Name}_{data["AbilityName"]}_Projectile")[0]));
        }
        catch (Exception e)
        {
            projectile = Instantiate(BaseProjectile);
            projectile.name = $"{data["Name"]}_ProbabilityAttackProjectile";
            PrefabUtility.SaveAsPrefabAsset(projectile.gameObject,
                $"Assets/BaseGame/Prefabs/Projectile/{heroStatData.Name}_{data["AbilityName"]}_Projectile.prefab");
            AssetDatabase.SaveAssets();
            DestroyImmediate(projectile.gameObject);

            projectile = AssetDatabase.LoadAssetAtPath<Projectile>(
                $"Assets/BaseGame/Prefabs/Projectile/{heroStatData.Name}_{data["AbilityName"]}_Projectile.prefab");
        }

        probabilityAttackAbilityBehavior.Projectile = projectile;
        passiveAbility.PassiveAbilityBehavior = probabilityAttackAbilityBehavior;
        passiveAbility.AbilityTarget = Ability.Target.EnemyInRange;
        passiveAbility.AbilityTrigger = Ability.Trigger.Passive;
        return passiveAbility;
    }

    private ActiveAbility GenerateActiveAbility(Dictionary<string, string> data, HeroStatData heroStatData)
    {
        ActiveAbility activeAbility = null;
        try
        {
            activeAbility = AssetDatabase.LoadAssetAtPath<ActiveAbility>(
                AssetDatabase.GUIDToAssetPath(
                    AssetDatabase.FindAssets($"t:ActiveAbility {heroStatData.Name}_{data["AbilityName"]}")[0]));
        }
        catch (Exception e)
        {
            activeAbility = CreateInstance<ActiveAbility>();
            activeAbility.name = $"{data["Name"]}_{data["AbilityName"]}";
            AssetDatabase.CreateAsset(activeAbility,
                $"Assets/BaseGame/ScriptableObjects/Ability/{heroStatData.Name}_{data["AbilityName"]}.asset");
            AssetDatabase.SaveAssets();

            activeAbility = AssetDatabase.LoadAssetAtPath<ActiveAbility>(
                $"Assets/BaseGame/ScriptableObjects/Ability/{heroStatData.Name}_{data["AbilityName"]}.asset");
        }

        return activeAbility;
    }
}