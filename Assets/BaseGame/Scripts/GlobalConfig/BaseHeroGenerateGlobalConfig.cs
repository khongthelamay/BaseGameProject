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
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor.Animations;
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "BaseHeroGenerateGlobalConfig", menuName = "GlobalConfigs/BaseHeroGenerateGlobalConfig")]
[GlobalConfig("Assets/Resources/GlobalConfig/")]
public class BaseHeroGenerateGlobalConfig : GlobalConfig<BaseHeroGenerateGlobalConfig>
{
    [field: SerializeField] public List<HeroConfigData> SheetHeroStatDataList { get; private set; }
    [field: SerializeField] public List<HeroConfigData> HeroStatDataList { get; private set; }
    [field: SerializeField] public List<Hero> HeroPrefabList { get; private set; }
    [field: SerializeField] public Hero BaseHero { get; private set; }
    [field: SerializeField] public Projectile BaseProjectile { get; private set; }

    [field: SerializeField] public Sprite[] GraphicArray { get; private set; }
    [field: SerializeField] public Sprite[] RarityArray { get; private set; }
    [field: SerializeField] public Color[] RarityColorArray { get; private set; }

    // [Button]
    // public async UniTask GenerateHeroStatData()
    // {
    //     string sheetId = "1-HkinUwSW4A4SkuiLGtl0Tm8771jFPVZB5ZpLs5pxz4";
    //
    //     List<HeroConfigData> heroStatDataList = AssetDatabase.FindAssets("t:HeroConfigData")
    //         .Select(AssetDatabase.GUIDToAssetPath)
    //         .Select(AssetDatabase.LoadAssetAtPath<HeroConfigData>)
    //         .ToList();
    //
    //     SheetHeroStatDataList = new List<HeroConfigData>();
    //     string result = await ABakingSheet.GetCsv(sheetId, "UnitAbility");
    //     List<Dictionary<string, string>> csvData = ACsvReader.ReadDataFromString(result);
    //     HeroConfigData heroConfigData = null;
    //     for (var i = 0; i < csvData.Count; i++)
    //     {
    //         Dictionary<string, string> data = csvData[i];
    //         data = data.ToDictionary(d => d.Key, d => d.Value.Replace(" ", ""));
    //         if (data["ID"].IsNullOrWhitespace()) continue;
    //         heroConfigData = heroStatDataList.FirstOrDefault(d => d.Name == data["Name"]);
    //         if (heroConfigData == null)
    //         {
    //             heroConfigData = CreateInstance<HeroConfigData>();
    //             heroConfigData.Name = data["Name"];
    //             heroConfigData.name = data["Name"];
    //             AssetDatabase.CreateAsset(heroConfigData,
    //                 $"Assets/BaseGame/ScriptableObjects/HeroConfigData/{heroConfigData.Name}.asset");
    //             AssetDatabase.SaveAssets();
    //             heroConfigData =
    //                 AssetDatabase.LoadAssetAtPath<HeroConfigData>(
    //                     $"Assets/BaseGame/ScriptableObjects/HeroConfigData/{heroConfigData.Name}.asset");
    //         }
    //
    //         EditorUtility.SetDirty(heroConfigData);
    //         GenerateBaseData(data, heroConfigData);
    //         
    //         EditorUtility.SetDirty(heroConfigData);
    //         heroConfigData.HeroAbilities = new List<Ability1>();
    //
    //         NormalAttackAbility1 normalAttackAbility1 = GenerateNormalAttackAbility(data, heroConfigData);
    //         heroConfigData.HeroAbilities.Add(normalAttackAbility1);
    //
    //         for (int j = i; j < csvData.Count; j++)
    //         {
    //             if (csvData[j]["AbilityName"].IsNullOrWhitespace()) break;
    //             Ability1 ability1 = GenerateAbility(csvData[j], heroConfigData);
    //             heroConfigData.HeroAbilities.Add(ability1);
    //         }
    //
    //         heroConfigData.HeroAbilities = heroConfigData.HeroAbilities.Where(a => a != null).ToList();
    //     }
    //
    //     SheetHeroStatDataList = AssetDatabase.FindAssets("t:HeroConfigData")
    //         .Select(AssetDatabase.GUIDToAssetPath)
    //         .Select(AssetDatabase.LoadAssetAtPath<HeroConfigData>)
    //         .Where(hsd => hsd.HeroSprite != null)
    //         .ToList();
    //
    //     HeroConfigDataList = AssetDatabase.FindAssets("t:Prefab", new string[] { "Assets/BaseGame/Prefabs/Hero" })
    //         .Select(AssetDatabase.GUIDToAssetPath)
    //         .Select(AssetDatabase.LoadAssetAtPath<Hero>)
    //         .ToList();
    //
    //
    //     SheetHeroStatDataList.ForEach(hsd =>
    //     {
    //         if (HeroConfigDataList.Any(p => p.HeroConfigData == hsd)) return;
    //         Hero hero = Instantiate(BaseHero);
    //         hero.HeroConfigData = hsd;
    //         hero.name = hero.HeroConfigData.Name;
    //         hero.InitHero();
    //         PrefabUtility.SaveAsPrefabAsset(hero.gameObject, $"Assets/BaseGame/Prefabs/Hero/{hero.name}.prefab");
    //         AssetDatabase.SaveAssets();
    //
    //         DestroyImmediate(hero.gameObject);
    //     });
    //
    //     HeroConfigDataList = AssetDatabase.FindAssets("t:Prefab", new string[] { "Assets/BaseGame/Prefabs/Hero" })
    //         .Select(AssetDatabase.GUIDToAssetPath)
    //         .Select(AssetDatabase.LoadAssetAtPath<Hero>)
    //         .Where(h => h.HeroConfigData.HeroSprite != null)
    //         .ToList();
    //     HeroConfigDataList.ForEach(h =>
    //     {
    //         using PrefabUtility.EditPrefabContentsScope editorScope = new PrefabUtility.EditPrefabContentsScope(AssetDatabase.GetAssetPath(h));
    //         editorScope.prefabContentsRoot.GetComponent<Hero>().InitHero();
    //     });
    //
    //     AssetDatabase.SaveAssets();
    //     AssetDatabase.Refresh();
    //
    //     HeroPoolGlobalConfig.Instance.GetAllHeroPrefab();
    // }
    //
    // private static void GenerateBaseData(Dictionary<string, string> data, HeroConfigData heroConfigData)
    // {
    //     GenerateHeroJob(data, heroConfigData);
    //     GenerateHeroClass(data, heroConfigData);
    //     GenerateBaseStat(data, heroConfigData);
    //     GenerateIdleAnimData(data, heroConfigData);
    //     GenerateAttackAnimData(data, heroConfigData);
    //     GenerateSkillAnimData(data, heroConfigData);
    //     GenerateAnimatorData(data, heroConfigData);
    //     
    //     GenerateIdleImageAnimData(data, heroConfigData);
    //     GenerateImageAnimatorData(data, heroConfigData);
    // }
    //
    // private static void GenerateIdleAnimData(Dictionary<string, string> data, HeroConfigData heroConfigData)
    // {
    //     try
    //     {
    //         EditorUtility.SetDirty(heroConfigData);
    //         heroConfigData.HeroSprite = AssetDatabase.LoadAssetAtPath<Sprite>(
    //             AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets($"t:Sprite {data["Name"]}-Idle")[0]));
    //     }
    //     catch (Exception e)
    //     {
    //         UnityEngine.Debug.Log(e);
    //     }
    //
    //     AnimationClip idleAnim;
    //     try
    //     {
    //         string animIdleGuid = AssetDatabase.FindAssets($"t:AnimationClip {data["Name"]}-Idle-Animation")[0];
    //         idleAnim = AssetDatabase.LoadAssetAtPath<AnimationClip>(AssetDatabase.GUIDToAssetPath(animIdleGuid));
    //     }
    //     catch (Exception e)
    //     {
    //         idleAnim = null;
    //     }
    //
    //     string[] spriteAnim = AssetDatabase.FindAssets($"t:Sprite {data["Name"]}-Idle");
    //     List<Sprite> spriteList = spriteAnim.Select(AssetDatabase.GUIDToAssetPath)
    //         .Select(AssetDatabase.LoadAssetAtPath<Sprite>)
    //         .ToList();
    //     if (idleAnim == null)
    //     {
    //         if (spriteAnim.Length == 0) return;
    //         idleAnim = new AnimationClip
    //         {
    //             name = $"{data["Name"]}-Idle-Animation",
    //             frameRate = 30
    //         };
    //         if (!AssetDatabase.IsValidFolder($"Assets/BaseGame/Animations/Hero/{data["Name"]}"))
    //         {
    //             AssetDatabase.CreateFolder("Assets/BaseGame/Animations/Hero", data["Name"]);
    //         }
    //
    //         AssetDatabase.CreateAsset(idleAnim,
    //             $"Assets/BaseGame/Animations/Hero/{data["Name"]}/{data["Name"]}-Idle-Animation.anim");
    //         AssetDatabase.SaveAssets();
    //     }
    //     else
    //     {
    //         idleAnim.ClearCurves();
    //     }
    //
    //
    //     EditorCurveBinding curveBinding = new EditorCurveBinding
    //     {
    //         type = typeof(SpriteRenderer),
    //         path = "",
    //         propertyName = "m_Sprite"
    //     };
    //     ObjectReferenceKeyframe[] keyFrames = new ObjectReferenceKeyframe[spriteList.Count];
    //     for (int i = 0; i < spriteList.Count; i++)
    //     {
    //         keyFrames[i] = new ObjectReferenceKeyframe
    //         {
    //             time = i / idleAnim.frameRate,
    //             value = spriteList[i]
    //         };
    //     }
    //
    //     AnimationUtility.SetObjectReferenceCurve(idleAnim, curveBinding, keyFrames);
    //
    //     idleAnim = AssetDatabase.LoadAssetAtPath<AnimationClip>(
    //         $"Assets/BaseGame/Animations/Hero/{data["Name"]}/{data["Name"]}-Idle-Animation.anim");
    //     // set loop animation
    //     AnimationClipSettings clipSettings = AnimationUtility.GetAnimationClipSettings(idleAnim);
    //     clipSettings.loopTime = true;
    //     AnimationUtility.SetAnimationClipSettings(idleAnim, clipSettings);
    //     
    // }
    // private static void GenerateAttackAnimData(Dictionary<string, string> data, HeroConfigData heroConfigData)
    // {
    //     AnimationClip attackAnim;
    //     try
    //     {
    //         string animAttackGuid = AssetDatabase.FindAssets($"t:AnimationClip {data["Name"]}-Attack-Animation")[0];
    //         attackAnim = AssetDatabase.LoadAssetAtPath<AnimationClip>(AssetDatabase.GUIDToAssetPath(animAttackGuid));
    //     }
    //     catch (Exception e)
    //     {
    //         attackAnim = null;
    //     }
    //
    //     string[] spriteAnim = AssetDatabase.FindAssets($"t:Sprite {data["Name"]}-Attack");
    //     List<Sprite> spriteList = spriteAnim.Select(AssetDatabase.GUIDToAssetPath)
    //         .Select(AssetDatabase.LoadAssetAtPath<Sprite>)
    //         .ToList();
    //     if (attackAnim == null)
    //     {
    //         if (spriteAnim.Length == 0) return;
    //         attackAnim = new AnimationClip
    //         {
    //             name = $"{data["Name"]}-Attack-Animation",
    //             frameRate = 30
    //         };
    //         if (!AssetDatabase.IsValidFolder($"Assets/BaseGame/Animations/Hero/{data["Name"]}"))
    //         {
    //             AssetDatabase.CreateFolder("Assets/BaseGame/Animations/Hero", data["Name"]);
    //         }
    //
    //         AssetDatabase.CreateAsset(attackAnim,
    //             $"Assets/BaseGame/Animations/Hero/{data["Name"]}/{data["Name"]}-Attack-Animation.anim");
    //         AssetDatabase.SaveAssets();
    //     }
    //     else
    //     {
    //         attackAnim.ClearCurves();
    //     }
    //
    //     EditorCurveBinding curveBinding = new EditorCurveBinding
    //     {
    //         type = typeof(SpriteRenderer),
    //         path = "",
    //         propertyName = "m_Sprite"
    //     };
    //     ObjectReferenceKeyframe[] keyFrames = new ObjectReferenceKeyframe[spriteList.Count];
    //     for (int i = 0; i < spriteList.Count; i++)
    //     {
    //         keyFrames[i] = new ObjectReferenceKeyframe
    //         {
    //             time = i / attackAnim.frameRate,
    //             value = spriteList[i]
    //         };
    //     }
    //
    //     AnimationUtility.SetObjectReferenceCurve(attackAnim, curveBinding, keyFrames);
    //
    //     attackAnim = AssetDatabase.LoadAssetAtPath<AnimationClip>(
    //         $"Assets/BaseGame/Animations/Hero/{data["Name"]}/{data["Name"]}-Attack-Animation.anim");
    // }
    // private static void GenerateSkillAnimData(Dictionary<string, string> data, HeroConfigData heroConfigData)
    // {
    //     AnimationClip skillAnim;
    //     try
    //     {
    //         string animSkillGuid = AssetDatabase.FindAssets($"t:AnimationClip {data["Name"]}-Skill-Animation")[0];
    //         skillAnim = AssetDatabase.LoadAssetAtPath<AnimationClip>(AssetDatabase.GUIDToAssetPath(animSkillGuid));
    //     }
    //     catch (Exception e)
    //     {
    //         skillAnim = null;
    //     }
    //
    //     string[] spriteAnim = AssetDatabase.FindAssets($"t:Sprite {data["Name"]}-Skill");
    //     List<Sprite> spriteList = spriteAnim.Select(AssetDatabase.GUIDToAssetPath)
    //         .Select(AssetDatabase.LoadAssetAtPath<Sprite>)
    //         .ToList();
    //     if (skillAnim == null)
    //     {
    //         if (spriteAnim.Length == 0) return;
    //         skillAnim = new AnimationClip
    //         {
    //             name = $"{data["Name"]}-Skill-Animation",
    //             frameRate = 30
    //         };
    //         if (!AssetDatabase.IsValidFolder($"Assets/BaseGame/Animations/Hero/{data["Name"]}"))
    //         {
    //             AssetDatabase.CreateFolder("Assets/BaseGame/Animations/Hero", data["Name"]);
    //         }
    //
    //         AssetDatabase.CreateAsset(skillAnim,
    //             $"Assets/BaseGame/Animations/Hero/{data["Name"]}/{data["Name"]}-Skill-Animation.anim");
    //         AssetDatabase.SaveAssets();
    //     }
    //     else
    //     {
    //         skillAnim.ClearCurves();
    //     }
    //
    //     EditorCurveBinding curveBinding = new EditorCurveBinding
    //     {
    //         type = typeof(SpriteRenderer),
    //         path = "",
    //         propertyName = "m_Sprite"
    //     };
    //     ObjectReferenceKeyframe[] keyFrames = new ObjectReferenceKeyframe[spriteList.Count];
    //     for (int i = 0; i < spriteList.Count; i++)
    //     {
    //         keyFrames[i] = new ObjectReferenceKeyframe
    //         {
    //             time = i / skillAnim.frameRate,
    //             value = spriteList[i]
    //         };
    //     }
    //
    //     AnimationUtility.SetObjectReferenceCurve(skillAnim, curveBinding, keyFrames);
    //
    //     skillAnim = AssetDatabase.LoadAssetAtPath<AnimationClip>(
    //         $"Assets/BaseGame/Animations/Hero/{data["Name"]}/{data["Name"]}-Skill-Animation.anim");
    // }
    // private static void GenerateAnimatorData(Dictionary<string, string> data, HeroConfigData heroConfigData)
    // {
    //     AnimatorController animatorController;
    //     try
    //     {
    //         string animControllerGuid = AssetDatabase.FindAssets($"t:AnimatorController {data["Name"]}-AnimatorController", 
    //             new []{$"Assets/BaseGame/Animations/Hero/{data["Name"]}"})[0];
    //         animatorController = AssetDatabase.LoadAssetAtPath<AnimatorController>(AssetDatabase.GUIDToAssetPath(animControllerGuid));
    //     }
    //     catch (Exception e)
    //     {
    //         animatorController = null;
    //     }
    //
    //     if (animatorController == null)
    //     {
    //         animatorController = new AnimatorController
    //         {
    //             name = $"{data["Name"]}-AnimatorController"
    //         };
    //         if (!AssetDatabase.IsValidFolder($"Assets/BaseGame/Animations/Hero/{data["Name"]}"))
    //         {
    //             AssetDatabase.CreateFolder("Assets/BaseGame/Animations/Hero", data["Name"]);
    //         }
    //
    //         AssetDatabase.CreateAsset(animatorController,
    //             $"Assets/BaseGame/Animations/Hero/{data["Name"]}/{data["Name"]}-AnimatorController.controller");
    //         AssetDatabase.SaveAssets();
    //     }
    //     EditorUtility.SetDirty(heroConfigData);
    //     heroConfigData.AnimatorController = AssetDatabase.LoadAssetAtPath<AnimatorController>(
    //         $"Assets/BaseGame/Animations/Hero/{data["Name"]}/{data["Name"]}-AnimatorController.controller");
    //     animatorController.layers = Array.Empty<AnimatorControllerLayer>();
    //     animatorController.AddLayer("Base Layer");
    //     AnimatorStateMachine stateMachine = animatorController.layers[0].stateMachine;
    //     animatorController.parameters = Array.Empty<AnimatorControllerParameter>();
    //     animatorController.AddParameter("TickRate", AnimatorControllerParameterType.Float);
    //     animatorController.parameters[0].defaultFloat = 1;
    //
    //     AnimationClip idleClip = AssetDatabase.LoadAssetAtPath<AnimationClip>(
    //         $"Assets/BaseGame/Animations/Hero/{data["Name"]}/{data["Name"]}-Idle-Animation.anim");
    //     AnimatorState idleState = stateMachine.AddState("Idle");
    //     idleState.motion = idleClip;
    //     
    //     AnimationClip attackClip = AssetDatabase.LoadAssetAtPath<AnimationClip>(
    //         $"Assets/BaseGame/Animations/Hero/{data["Name"]}/{data["Name"]}-Attack-Animation.anim");
    //     AnimatorState attackState = stateMachine.AddState("Attack");
    //     attackState.motion = attackClip;
    //     attackState.speedParameterActive = true;
    //     attackState.speedParameter = "TickRate";
    //     AnimatorStateTransition attackToIdle = attackState.AddTransition(idleState);
    //     attackToIdle.hasExitTime = true;
    //     attackToIdle.exitTime = 1;
    //     attackToIdle.hasFixedDuration = false;
    //     attackToIdle.duration = 0;
    //     attackToIdle.offset = 0;
    //     
    //     // attackToIdle;
    //     
    //     
    //     AnimationClip skillClip = AssetDatabase.LoadAssetAtPath<AnimationClip>(
    //         $"Assets/BaseGame/Animations/Hero/{data["Name"]}/{data["Name"]}-Skill-Animation.anim");
    //     if (skillClip != null)
    //     {
    //         AnimatorState skillState = stateMachine.AddState("Skill");
    //         skillState.motion = skillClip;
    //         skillState.speedParameterActive = true;
    //         skillState.speedParameter = "TickRate";
    //         AnimatorStateTransition skillToIdle = skillState.AddTransition(idleState);
    //         skillToIdle.hasExitTime = true;
    //         skillToIdle.exitTime = 1;
    //         skillToIdle.hasFixedDuration = false;
    //         skillToIdle.duration = 0;
    //         skillToIdle.offset = 0;
    //     }
    // }
    //
    //
    // private static void GenerateIdleImageAnimData(Dictionary<string, string> data, HeroConfigData heroConfigData)
    // {
    //     try
    //     {
    //         EditorUtility.SetDirty(heroConfigData);
    //         heroConfigData.HeroSprite = AssetDatabase.LoadAssetAtPath<Sprite>(
    //             AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets($"t:Sprite {data["Name"]}-Idle")[0]));
    //     }
    //     catch (Exception e)
    //     {
    //         UnityEngine.Debug.Log(e);
    //     }
    //
    //     AnimationClip idleAnim;
    //     try
    //     {
    //         string animIdleGuid = AssetDatabase.FindAssets($"t:AnimationClip {data["Name"]}-Idle-Image-Animation")[0];
    //         idleAnim = AssetDatabase.LoadAssetAtPath<AnimationClip>(AssetDatabase.GUIDToAssetPath(animIdleGuid));
    //     }
    //     catch (Exception e)
    //     {
    //         idleAnim = null;
    //     }
    //
    //     string[] spriteAnim = AssetDatabase.FindAssets($"t:Sprite {data["Name"]}-Idle");
    //     List<Sprite> spriteList = spriteAnim.Select(AssetDatabase.GUIDToAssetPath)
    //         .Select(AssetDatabase.LoadAssetAtPath<Sprite>)
    //         .ToList();
    //     if (idleAnim == null)
    //     {
    //         if (spriteAnim.Length == 0) return;
    //         idleAnim = new AnimationClip
    //         {
    //             name = $"{data["Name"]}-Idle-Image-Animation",
    //             frameRate = 30
    //         };
    //         if (!AssetDatabase.IsValidFolder($"Assets/BaseGame/Animations/Hero/{data["Name"]}"))
    //         {
    //             AssetDatabase.CreateFolder("Assets/BaseGame/Animations/Hero", data["Name"]);
    //         }
    //
    //         AssetDatabase.CreateAsset(idleAnim,
    //             $"Assets/BaseGame/Animations/Hero/{data["Name"]}/{data["Name"]}-Idle-Image-Animation.anim");
    //         AssetDatabase.SaveAssets();
    //     }
    //     else
    //     {
    //         idleAnim.ClearCurves();
    //     }
    //
    //
    //     EditorCurveBinding curveBinding = new EditorCurveBinding
    //     {
    //         type = typeof(Image),
    //         path = "",
    //         propertyName = "m_Sprite"
    //     };
    //     ObjectReferenceKeyframe[] keyFrames = new ObjectReferenceKeyframe[spriteList.Count];
    //     for (int i = 0; i < spriteList.Count; i++)
    //     {
    //         keyFrames[i] = new ObjectReferenceKeyframe
    //         {
    //             time = i / idleAnim.frameRate,
    //             value = spriteList[i]
    //         };
    //     }
    //
    //     AnimationUtility.SetObjectReferenceCurve(idleAnim, curveBinding, keyFrames);
    //
    //     idleAnim = AssetDatabase.LoadAssetAtPath<AnimationClip>(
    //         $"Assets/BaseGame/Animations/Hero/{data["Name"]}/{data["Name"]}-Idle-Image-Animation.anim");
    //     // set loop animation
    //     AnimationClipSettings clipSettings = AnimationUtility.GetAnimationClipSettings(idleAnim);
    //     clipSettings.loopTime = true;
    //     AnimationUtility.SetAnimationClipSettings(idleAnim, clipSettings);
    // }
    //
    // private static void GenerateImageAnimatorData(Dictionary<string, string> data, HeroConfigData heroConfigData)
    // {
    //     
    //     AnimatorController animatorController;
    //     try
    //     {
    //         string animControllerGuid = AssetDatabase.FindAssets($"t:AnimatorController {data["Name"]}-Image-AnimatorController", 
    //             new []{$"Assets/BaseGame/Animations/Hero/{data["Name"]}"})[0];
    //         animatorController = AssetDatabase.LoadAssetAtPath<AnimatorController>(AssetDatabase.GUIDToAssetPath(animControllerGuid));
    //     }
    //     catch (Exception e)
    //     {
    //         animatorController = null;
    //     }
    //
    //     if (animatorController == null)
    //     {
    //         animatorController = new AnimatorController
    //         {
    //             name = $"{data["Name"]}-Image-AnimatorController"
    //         };
    //         if (!AssetDatabase.IsValidFolder($"Assets/BaseGame/Animations/Hero/{data["Name"]}"))
    //         {
    //             AssetDatabase.CreateFolder("Assets/BaseGame/Animations/Hero", data["Name"]);
    //         }
    //
    //         AssetDatabase.CreateAsset(animatorController,
    //             $"Assets/BaseGame/Animations/Hero/{data["Name"]}/{data["Name"]}-Image-AnimatorController.controller");
    //         AssetDatabase.SaveAssets();
    //     }
    //     EditorUtility.SetDirty(heroConfigData);
    //     heroConfigData.ImageAnimatorController = AssetDatabase.LoadAssetAtPath<AnimatorController>(
    //         $"Assets/BaseGame/Animations/Hero/{data["Name"]}/{data["Name"]}-Image-AnimatorController.controller");
    //     animatorController.layers = Array.Empty<AnimatorControllerLayer>();
    //     animatorController.AddLayer("Base Layer");
    //     AnimatorStateMachine stateMachine = animatorController.layers[0].stateMachine;
    //
    //     AnimationClip idleClip = AssetDatabase.LoadAssetAtPath<AnimationClip>(
    //         $"Assets/BaseGame/Animations/Hero/{data["Name"]}/{data["Name"]}-Idle-Image-Animation.anim");
    //     AnimatorState idleState = stateMachine.AddState("Idle");
    //     idleState.motion = idleClip;
    // }
    // private static void GenerateBaseStat(Dictionary<string, string> data, HeroConfigData heroConfigData)
    // {
    //     if (int.TryParse(data["Tier"], out int tier))
    //     {
    //         heroConfigData.HeroRarity = (Hero.Rarity)(tier - 1);
    //     }
    //
    //     if (int.TryParse(data["AttackDamage"], out int attackDamage))
    //     {
    //         heroConfigData.BaseAttackDamage = attackDamage;
    //     }
    //
    //     if (float.TryParse(data["AttackSpeed"], out float attackSpeed))
    //     {
    //         heroConfigData.BaseAttackSpeed = attackSpeed;
    //     }
    //
    //     if (float.TryParse(data["AttackRange"], out float attackRange))
    //     {
    //         heroConfigData.BaseAttackRange = attackRange;
    //     }
    //
    //     if (float.TryParse(data["UpgradePercentage"], out float upgradePercentage))
    //     {
    //         heroConfigData.UpgradePercentage = upgradePercentage;
    //     }
    // }
    //
    // private static void GenerateHeroClass(Dictionary<string, string> data, HeroConfigData heroConfigData)
    // {
    //     if (Enum.TryParse(typeof(Hero.Class), data["Class"], out object class1))
    //     {
    //         heroConfigData.HeroClass = (Hero.Class)class1;
    //     }
    // }
    //
    // private static void GenerateHeroJob(Dictionary<string, string> data, HeroConfigData heroConfigData)
    // {
    //     List<Hero.Job> jobList = new List<Hero.Job>();
    //     if (Enum.TryParse(typeof(Hero.Job), data["MainRace"], out object job1))
    //     {
    //         jobList.Add((Hero.Job)job1);
    //     }
    //
    //     if (Enum.TryParse(typeof(Hero.Job), data["SubRace1"], out object job2))
    //     {
    //         jobList.Add((Hero.Job)job2);
    //     }
    //
    //     if (Enum.TryParse(typeof(Hero.Job), data["SubRace2"], out object job3))
    //     {
    //         jobList.Add((Hero.Job)job3);
    //     }
    //
    //     heroConfigData.HeroJob = jobList.ToArray();
    // }
    //
    // private NormalAttackAbility1 GenerateNormalAttackAbility(Dictionary<string, string> data, HeroConfigData heroConfigData)
    // {
    //     NormalAttackAbility1 normalAttackAbility1 = null;
    //     try
    //     {
    //         normalAttackAbility1 = AssetDatabase.LoadAssetAtPath<NormalAttackAbility1>(
    //             AssetDatabase.GUIDToAssetPath(
    //                 AssetDatabase.FindAssets($"t:NormalAttackAbility1 {data["Name"]}_NormalAttackAbility")[0]));
    //     }
    //     catch (Exception e)
    //     {
    //         normalAttackAbility1 = CreateInstance<NormalAttackAbility1>();
    //         normalAttackAbility1.name = $"{data["Name"]}_NormalAttackAbility";
    //         AssetDatabase.CreateAsset(normalAttackAbility1,
    //             $"Assets/BaseGame/ScriptableObjects/Ability1/{data["Name"]}_NormalAttackAbility.asset");
    //         AssetDatabase.SaveAssets();
    //
    //         normalAttackAbility1 = AssetDatabase.LoadAssetAtPath<NormalAttackAbility1>(
    //             $"Assets/BaseGame/ScriptableObjects/Ability1/{data["Name"]}_NormalAttackAbility.asset");
    //     }
    //
    //     EditorUtility.SetDirty(normalAttackAbility1);
    //     Projectile projectile = null;
    //     try
    //     {
    //         projectile = AssetDatabase.LoadAssetAtPath<Projectile>(
    //             AssetDatabase.GUIDToAssetPath(
    //                 AssetDatabase.FindAssets($"t:Prefab {data["Name"]}_NormalAttackProjectile")[0]));
    //     }
    //     catch (Exception e)
    //     {
    //         projectile = Instantiate(BaseProjectile);
    //         projectile.name = $"{data["Name"]}_NormalAttackProjectile";
    //         PrefabUtility.SaveAsPrefabAsset(projectile.gameObject,
    //             $"Assets/BaseGame/Prefabs/Projectile/{data["Name"]}_NormalAttackProjectile.prefab");
    //         AssetDatabase.SaveAssets();
    //         DestroyImmediate(projectile.gameObject);
    //
    //         projectile =
    //             AssetDatabase.LoadAssetAtPath<Projectile>(
    //                 $"Assets/BaseGame/Prefabs/Projectile/{data["Name"]}_NormalAttackProjectile.prefab");
    //     }
    //
    //     normalAttackAbility1.Projectile = projectile;
    //     normalAttackAbility1.AbilityTarget = Ability1.Target.EnemyInRange;
    //     normalAttackAbility1.AbilityTrigger = Ability1.Trigger.NormalAttack;
    //     return normalAttackAbility1;
    // }
    //
    // private Ability1 GenerateAbility(Dictionary<string, string> data, HeroConfigData heroConfigData)
    // {
    //     if (data["ActiveType"] == "Passive")
    //     {
    //         return GeneratePassiveAbility(data, heroConfigData);
    //     }
    //
    //     if (data["ActiveType"] == "Active")
    //     {
    //         return GenerateActiveAbility(data, heroConfigData);
    //     }
    //
    //     Debug.Log($"Not found ability1 type {data["ActiveType"]}");
    //     return null;
    // }
    //
    // private PassiveAbility1 GeneratePassiveAbility(Dictionary<string, string> data, HeroConfigData heroConfigData)
    // {
    //     PassiveAbility1 passiveAbility1 = null;
    //     try
    //     {
    //         passiveAbility1 = AssetDatabase.LoadAssetAtPath<PassiveAbility1>(
    //             AssetDatabase.GUIDToAssetPath(
    //                 AssetDatabase.FindAssets($"t:PassiveAbility1 {data["Name"]}_{data["AbilityName"]}")[0]));
    //     }
    //     catch (Exception e)
    //     {
    //         passiveAbility1 = CreateInstance<PassiveAbility1>();
    //         passiveAbility1.name = $"{data["Name"]}_{data["AbilityName"]}";
    //         AssetDatabase.CreateAsset(passiveAbility1,
    //             $"Assets/BaseGame/ScriptableObjects/Ability1/{data["Name"]}_{data["AbilityName"]}.asset");
    //         AssetDatabase.SaveAssets();
    //
    //         passiveAbility1 = AssetDatabase.LoadAssetAtPath<PassiveAbility1>(
    //             $"Assets/BaseGame/ScriptableObjects/Ability1/{data["Name"]}_{data["AbilityName"]}.asset");
    //     }
    //
    //     EditorUtility.SetDirty(passiveAbility1);
    //     ProbabilityAttackAbilityBehavior probabilityAttackAbilityBehavior = new ProbabilityAttackAbilityBehavior();
    //     Projectile projectile = null;
    //     try
    //     {
    //         projectile = AssetDatabase.LoadAssetAtPath<Projectile>(
    //             AssetDatabase.GUIDToAssetPath(
    //                 AssetDatabase.FindAssets($"t:Prefab {heroConfigData.Name}_{data["AbilityName"]}_Projectile")[0]));
    //     }
    //     catch (Exception e)
    //     {
    //         projectile = Instantiate(BaseProjectile);
    //         projectile.name = $"{data["Name"]}_ProbabilityAttackProjectile";
    //         PrefabUtility.SaveAsPrefabAsset(projectile.gameObject,
    //             $"Assets/BaseGame/Prefabs/Projectile/{heroConfigData.Name}_{data["AbilityName"]}_Projectile.prefab");
    //         AssetDatabase.SaveAssets();
    //         DestroyImmediate(projectile.gameObject);
    //
    //         projectile = AssetDatabase.LoadAssetAtPath<Projectile>(
    //             $"Assets/BaseGame/Prefabs/Projectile/{heroConfigData.Name}_{data["AbilityName"]}_Projectile.prefab");
    //     }
    //
    //     probabilityAttackAbilityBehavior.Projectile = projectile;
    //     passiveAbility1.PassiveAbilityBehavior = probabilityAttackAbilityBehavior;
    //     passiveAbility1.AbilityTarget = Ability1.Target.EnemyInRange;
    //     passiveAbility1.AbilityTrigger = Ability1.Trigger.Passive;
    //     return passiveAbility1;
    // }
    //
    // private ActiveAbility1 GenerateActiveAbility(Dictionary<string, string> data, HeroConfigData heroConfigData)
    // {
    //     ActiveAbility1 activeAbility1 = null;
    //     try
    //     {
    //         activeAbility1 = AssetDatabase.LoadAssetAtPath<ActiveAbility1>(
    //             AssetDatabase.GUIDToAssetPath(
    //                 AssetDatabase.FindAssets($"t:ActiveAbility1 {heroConfigData.Name}_{data["AbilityName"]}")[0]));
    //     }
    //     catch (Exception e)
    //     {
    //         activeAbility1 = CreateInstance<ActiveAbility1>();
    //         activeAbility1.name = $"{data["Name"]}_{data["AbilityName"]}";
    //         AssetDatabase.CreateAsset(activeAbility1,
    //             $"Assets/BaseGame/ScriptableObjects/Ability1/{heroConfigData.Name}_{data["AbilityName"]}.asset");
    //         AssetDatabase.SaveAssets();
    //
    //         activeAbility1 = AssetDatabase.LoadAssetAtPath<ActiveAbility1>(
    //             $"Assets/BaseGame/ScriptableObjects/Ability1/{heroConfigData.Name}_{data["AbilityName"]}.asset");
    //     }
    //
    //     return activeAbility1;
    // }
}