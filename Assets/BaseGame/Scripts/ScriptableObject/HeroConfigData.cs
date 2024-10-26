using System.Collections.Generic;
using Core;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using System.Linq;
using TW.Utility.Extension;
using UnityEditor.Animations;
using UnityEngine;

#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine.UI;
#endif

[CreateAssetMenu(fileName = "HeroConfigData", menuName = "ScriptableObjects/HeroConfigData")]
[InlineEditor]
public class HeroConfigData : ScriptableObject
{
    [field: HorizontalGroup(nameof(HeroConfigData), width: 200), PreviewField(200), HideLabel]
    [field: SerializeField]
    public Sprite HeroSprite { get; set; }

    [field: VerticalGroup(nameof(HeroConfigData) + "/Stats")]
    [field: SerializeField]
    public string Name { get; set; }

    [field: VerticalGroup(nameof(HeroConfigData) + "/Stats")]
    [field: SerializeField]
    public Hero.Rarity HeroRarity { get; set; }

    [field: VerticalGroup(nameof(HeroConfigData) + "/Stats")]
    [field: SerializeField]
    public Hero.Class HeroClass { get; set; }

    [field: VerticalGroup(nameof(HeroConfigData) + "/Stats")]
    [field: SerializeField]
    public Hero.Job[] HeroJob { get; set; }

    [field: VerticalGroup(nameof(HeroConfigData) + "/Stats")]
    [field: SerializeField]
    public int BaseAttackDamage { get; set; }

    [field: VerticalGroup(nameof(HeroConfigData) + "/Stats")]
    [field: SerializeField]
    public float BaseAttackSpeed { get; set; }

    [field: VerticalGroup(nameof(HeroConfigData) + "/Stats")]
    [field: SerializeField]
    public float BaseAttackRange { get; set; }

    [field: VerticalGroup(nameof(HeroConfigData) + "/Stats")]
    [field: SerializeField]
    public float UpgradePercentage { get; set; }

    [field: SerializeField] public AnimatorController AnimatorController { get; set; }
    [field: SerializeField] public AnimatorController ImageAnimatorController { get; set; }
    [field: SerializeField] public Hero HeroPrefab {get; private set;}

    [field: SerializeReference] public List<Ability> HeroAbilities { get; set; }
    
    

#if UNITY_EDITOR
    [Button]
    private async UniTask UpdateData()
    {
        string sheetId = "1-HkinUwSW4A4SkuiLGtl0Tm8771jFPVZB5ZpLs5pxz4";
        EditorUtility.SetDirty(this);
        string result = await ABakingSheet.GetCsv(sheetId, "UnitAbility");
        List<Dictionary<string, string>> csvData = ACsvReader.ReadDataFromString(result)
            .Select(value =>
                value.ToDictionary(
                    d => d.Key,
                    d => d.Value.Replace(" ", "")))
            .ToList();

        Dictionary<string, string> data = csvData.Find(x => x["Name"] == Name);
        GenerateHeroSprite(data);
        GenerateBaseStat(data);
        GenerateHeroJob(data);
        GenerateHeroClass(data);
        
        
        // generate sprite animation
        GenerateIdleAnimData(data);
        GenerateAttackAnimData(data);
        GenerateSkillAnimData(data);
        GenerateAnimatorData(data);
        
        // generate image animation
        GenerateIdleImageAnimData(data);
        GenerateImageAnimatorData(data);
        
        GenerateHeroPrefab(data);

        string newName = $"{Name}.asset";
        string assetPath = AssetDatabase.GetAssetPath(this);
        AssetDatabase.RenameAsset(assetPath, newName);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
    }

    private void GenerateHeroPrefab(Dictionary<string, string> data)
    {
        EditorUtility.SetDirty(this);
        try
        {
            HeroPrefab = AssetDatabase.LoadAssetAtPath<Hero>(
                AssetDatabase.GUIDToAssetPath(
                    AssetDatabase.FindAssets($"t:Prefab {Name}", 
                        new string[] { "Assets/BaseGame/Prefabs/Hero" })[0]));
             
        }
        catch (Exception _)
        {
            Hero baseHero = AssetDatabase.LoadAssetAtPath<Hero>(
                AssetDatabase.GUIDToAssetPath(
                    AssetDatabase.FindAssets("t:Prefab BaseHero")[0]));
            Hero hero = Instantiate(baseHero);
            hero.name = Name;
            PrefabUtility.SaveAsPrefabAsset(hero.gameObject, $"Assets/BaseGame/Prefabs/Hero/{hero.name}.prefab");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            HeroPrefab = AssetDatabase.LoadAssetAtPath<Hero>(
                AssetDatabase.GUIDToAssetPath(
                    AssetDatabase.FindAssets($"t:Prefab {Name}", 
                        new string[] { "Assets/BaseGame/Prefabs/Hero" })[0]));
            DestroyImmediate(hero.gameObject);
        }
        HeroPrefab.InitHero(this);
    }

    private void GenerateHeroSprite(Dictionary<string, string> data)
    {
        HeroSprite = AssetDatabase.LoadAssetAtPath<Sprite>(
            AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets($"t:Sprite {Name}-Idle", new []{$"Assets/BaseGame/Animations/Hero/{data["Name"]}"})[0]));
    }
    private void GenerateBaseStat(Dictionary<string, string> data)
    {
        if (int.TryParse(data["Tier"], out int tier))
        {
            HeroRarity = (Hero.Rarity)(tier - 1);
        }

        if (int.TryParse(data["AttackDamage"], out int attackDamage))
        {
            BaseAttackDamage = attackDamage;
        }

        if (float.TryParse(data["AttackSpeed"], out float attackSpeed))
        {
            BaseAttackSpeed = attackSpeed;
        }

        if (float.TryParse(data["AttackRange"], out float attackRange))
        {
            BaseAttackRange = attackRange;
        }

        if (float.TryParse(data["UpgradePercentage"], out float upgradePercentage))
        {
            UpgradePercentage = upgradePercentage;
        }
    }

    private void GenerateHeroJob(Dictionary<string, string> data)
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

        HeroJob = jobList.ToArray();
    }

    private void GenerateHeroClass(Dictionary<string, string> data)
    {
        if (Enum.TryParse(typeof(Hero.Class), data["Class"], out object class1))
        {
            HeroClass = (Hero.Class)class1;
        }
    }

    private void GenerateIdleAnimData(Dictionary<string, string> data)
    {
        AnimationClip idleAnim;
        try
        {
            string animIdleGuid = AssetDatabase.FindAssets($"t:AnimationClip {data["Name"]}-Idle-Animation", new []{$"Assets/BaseGame/Animations/Hero/{data["Name"]}"})[0];
            idleAnim = AssetDatabase.LoadAssetAtPath<AnimationClip>(AssetDatabase.GUIDToAssetPath(animIdleGuid));
        }
        catch (Exception e)
        {
            idleAnim = null;
        }

        string[] spriteAnim = AssetDatabase.FindAssets($"t:Sprite {data["Name"]}-Idle", new []{$"Assets/BaseGame/Animations/Hero/{data["Name"]}"});
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

    private void GenerateAttackAnimData(Dictionary<string, string> data)
    {
        AnimationClip attackAnim;
        try
        {
            string animAttackGuid = AssetDatabase.FindAssets($"t:AnimationClip {data["Name"]}-Attack-Animation", new []{$"Assets/BaseGame/Animations/Hero/{data["Name"]}"})[0];
            attackAnim = AssetDatabase.LoadAssetAtPath<AnimationClip>(AssetDatabase.GUIDToAssetPath(animAttackGuid));
        }
        catch (Exception e)
        {
            attackAnim = null;
        }

        string[] spriteAnim = AssetDatabase.FindAssets($"t:Sprite {data["Name"]}-Attack", new []{$"Assets/BaseGame/Animations/Hero/{data["Name"]}"});
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

    private void GenerateSkillAnimData(Dictionary<string, string> data)
    {
        AnimationClip skillAnim;
        try
        {
            string animSkillGuid = AssetDatabase.FindAssets($"t:AnimationClip {data["Name"]}-Skill-Animation", new []{$"Assets/BaseGame/Animations/Hero/{data["Name"]}"})[0];
            skillAnim = AssetDatabase.LoadAssetAtPath<AnimationClip>(AssetDatabase.GUIDToAssetPath(animSkillGuid));
        }
        catch (Exception e)
        {
            skillAnim = null;
        }

        string[] spriteAnim = AssetDatabase.FindAssets($"t:Sprite {data["Name"]}-Skill", new []{$"Assets/BaseGame/Animations/Hero/{data["Name"]}"});
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

    private void GenerateAnimatorData(Dictionary<string, string> data)
    {
        AnimatorController animatorController;
        try
        {
            string animControllerGuid = AssetDatabase.FindAssets(
                $"t:AnimatorController {data["Name"]}-AnimatorController", new []{$"Assets/BaseGame/Animations/Hero/{data["Name"]}"})[0];
            animatorController =
                AssetDatabase.LoadAssetAtPath<AnimatorController>(AssetDatabase.GUIDToAssetPath(animControllerGuid));
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

        EditorUtility.SetDirty(this);
        AnimatorController = AssetDatabase.LoadAssetAtPath<AnimatorController>(
            $"Assets/BaseGame/Animations/Hero/{data["Name"]}/{data["Name"]}-AnimatorController.controller");
        animatorController.layers = Array.Empty<AnimatorControllerLayer>();
        animatorController.AddLayer("Base Layer");
        AnimatorStateMachine stateMachine = animatorController.layers[0].stateMachine;
        animatorController.parameters = Array.Empty<AnimatorControllerParameter>();
        animatorController.AddParameter("TickRate", AnimatorControllerParameterType.Float);
        animatorController.parameters[0].defaultFloat = 1;

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

    private void GenerateIdleImageAnimData(Dictionary<string, string> data)
    {
        AnimationClip idleAnim;
        try
        {
            string animIdleGuid = AssetDatabase.FindAssets($"t:AnimationClip {data["Name"]}-Idle-Image-Animation", new []{$"Assets/BaseGame/Animations/Hero/{data["Name"]}"})[0];
            idleAnim = AssetDatabase.LoadAssetAtPath<AnimationClip>(AssetDatabase.GUIDToAssetPath(animIdleGuid));
        }
        catch (Exception e)
        {
            idleAnim = null;
        }

        string[] spriteAnim = AssetDatabase.FindAssets($"t:Sprite {data["Name"]}-Idle", new []{$"Assets/BaseGame/Animations/Hero/{data["Name"]}"});
        List<Sprite> spriteList = spriteAnim.Select(AssetDatabase.GUIDToAssetPath)
            .Select(AssetDatabase.LoadAssetAtPath<Sprite>)
            .ToList();
        if (idleAnim == null)
        {
            if (spriteAnim.Length == 0) return;
            idleAnim = new AnimationClip
            {
                name = $"{data["Name"]}-Idle-Image-Animation",
                frameRate = 30
            };
            if (!AssetDatabase.IsValidFolder($"Assets/BaseGame/Animations/Hero/{data["Name"]}"))
            {
                AssetDatabase.CreateFolder("Assets/BaseGame/Animations/Hero", data["Name"]);
            }

            AssetDatabase.CreateAsset(idleAnim,
                $"Assets/BaseGame/Animations/Hero/{data["Name"]}/{data["Name"]}-Idle-Image-Animation.anim");
            AssetDatabase.SaveAssets();
        }
        else
        {
            idleAnim.ClearCurves();
        }


        EditorCurveBinding curveBinding = new EditorCurveBinding
        {
            type = typeof(Image),
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
            $"Assets/BaseGame/Animations/Hero/{data["Name"]}/{data["Name"]}-Idle-Image-Animation.anim");
        // set loop animation
        AnimationClipSettings clipSettings = AnimationUtility.GetAnimationClipSettings(idleAnim);
        clipSettings.loopTime = true;
        AnimationUtility.SetAnimationClipSettings(idleAnim, clipSettings);
    }

    private void GenerateImageAnimatorData(Dictionary<string, string> data)
    {
        AnimatorController animatorController;
        try
        {
            string animControllerGuid = AssetDatabase.FindAssets(
                $"t:AnimatorController {data["Name"]}-Image-AnimatorController",
                new[] { $"Assets/BaseGame/Animations/Hero/{data["Name"]}" })[0];
            animatorController =
                AssetDatabase.LoadAssetAtPath<AnimatorController>(AssetDatabase.GUIDToAssetPath(animControllerGuid));
        }
        catch (Exception e)
        {
            animatorController = null;
        }

        if (animatorController == null)
        {
            animatorController = new AnimatorController
            {
                name = $"{data["Name"]}-Image-AnimatorController"
            };
            if (!AssetDatabase.IsValidFolder($"Assets/BaseGame/Animations/Hero/{data["Name"]}"))
            {
                AssetDatabase.CreateFolder("Assets/BaseGame/Animations/Hero", data["Name"]);
            }

            AssetDatabase.CreateAsset(animatorController,
                $"Assets/BaseGame/Animations/Hero/{data["Name"]}/{data["Name"]}-Image-AnimatorController.controller");
            AssetDatabase.SaveAssets();
        }

        EditorUtility.SetDirty(this);
        ImageAnimatorController = AssetDatabase.LoadAssetAtPath<AnimatorController>(
            $"Assets/BaseGame/Animations/Hero/{data["Name"]}/{data["Name"]}-Image-AnimatorController.controller");
        animatorController.layers = Array.Empty<AnimatorControllerLayer>();
        animatorController.AddLayer("Base Layer");
        AnimatorStateMachine stateMachine = animatorController.layers[0].stateMachine;

        AnimationClip idleClip = AssetDatabase.LoadAssetAtPath<AnimationClip>(
            $"Assets/BaseGame/Animations/Hero/{data["Name"]}/{data["Name"]}-Idle-Image-Animation.anim");
        AnimatorState idleState = stateMachine.AddState("Idle");
        idleState.motion = idleClip;
    }
#endif
}