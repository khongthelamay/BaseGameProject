using System.Collections.Generic;
using Core;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using System.Linq;
using TW.Utility.Extension;
using UnityEngine;
using System;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor.Animations;
using UnityEditor;
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

    [field: SerializeField] public RuntimeAnimatorController AnimatorController { get; set; }
    [field: SerializeField] public RuntimeAnimatorController ImageAnimatorController { get; set; }
    [field: SerializeField] public GameObject HeroGameObjectPrefab { get; private set; }
    [field: SerializeField] public Hero HeroPrefab { get; private set; }
    [field: SerializeField, InlineEditor] public List<Ability> HeroAbilities { get; private set; }


#if UNITY_EDITOR
    [Button]
    private async UniTask UpdateData()
    {
        string sheetId = "1-HkinUwSW4A4SkuiLGtl0Tm8771jFPVZB5ZpLs5pxz4";
        EditorUtility.SetDirty(this);
        string result = await ABakingSheet.GetCsv(sheetId, "UnitAbility");
        List<Dictionary<string, string>> csvData = ACsvReader.ReadDataFromString(result)
            // .Select(value =>
            //     value.ToDictionary(
            //         d => d.Key,
            //         d => d.Value.Replace(" ", "")))
            .ToList();
        try
        {
            Dictionary<string, string> data = csvData.Find(x => x["Name"] == Name);
            GenerateHeroSprite(data);
            GenerateBaseStat(data);
            GenerateHeroJob(data);
            GenerateHeroClass(data);


            // generate sprite animation
            GenerateIdleAnimData(data);
            GenerateAttackAnimData(data);
            GenerateSkill1AnimData(data);
            GenerateSkill2AnimData(data);
            GenerateSkill3AnimData(data);
            GenerateAnimatorData(data);

            // generate image animation
            GenerateIdleImageAnimData(data);
            GenerateImageAnimatorData(data);
            GenerateRunImageAnimData(data);

            GenerateHeroGameObjectPrefab(data);
            AddUniqueHeroScript();

            GenerateHeroPrefab(data);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }


        string newName = $"{Name}.asset";
        string assetPath = AssetDatabase.GetAssetPath(this);
        AssetDatabase.RenameAsset(assetPath, newName);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private void GenerateHeroGameObjectPrefab(Dictionary<string, string> data)
    {
        try
        {
            EditorUtility.SetDirty(this);
            HeroGameObjectPrefab = AssetDatabase.FindAssets($"t:Prefab {Name}",
                    new string[] { "Assets/BaseGame/Prefabs/Hero" })
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<GameObject>)
                .FirstOrDefault(t => t.name == Name);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            if (HeroGameObjectPrefab == null)
            {
                GenerateNewGameObjectPrefab(data);
            }
        }
        catch (Exception _)
        {
            GenerateNewGameObjectPrefab(data);
        }
    }

    private void GenerateNewGameObjectPrefab(Dictionary<string, string> data)
    {
        try
        {
            EditorUtility.SetDirty(this);
            Hero heroPrefab = AssetDatabase.LoadAssetAtPath<Hero>(
                AssetDatabase.GUIDToAssetPath(
                    AssetDatabase.FindAssets("t:Prefab BaseHero")[0]));

            Hero hero = Instantiate(heroPrefab);
            hero.name = $"{Name}";

            PrefabUtility.SaveAsPrefabAsset(hero.gameObject, $"Assets/BaseGame/Prefabs/Hero/{Name}.prefab");


            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            HeroGameObjectPrefab = AssetDatabase.FindAssets($"t:Prefab {Name}",
                    new string[] { "Assets/BaseGame/Prefabs/Hero" })
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<GameObject>)
                .FirstOrDefault(t => t.name == Name);
            DestroyImmediate(hero.gameObject);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }

    private void GenerateHeroPrefab(Dictionary<string, string> data)
    {
        try
        {
            EditorUtility.SetDirty(this);
            HeroPrefab = HeroGameObjectPrefab.GetComponent<Hero>();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }

    private void AddUniqueHeroScript()
    {
        try
        {
            EditorUtility.SetDirty(this);
            string assetPath = AssetDatabase.GetAssetPath(HeroGameObjectPrefab);
            using PrefabUtility.EditPrefabContentsScope editorScope =
                new PrefabUtility.EditPrefabContentsScope(assetPath);

            Hero[] heroes = editorScope.prefabContentsRoot.GetComponents<Hero>();
            string typeName = $"Core.{Name.Replace(" ", "")}";
            Type type = Type.GetType(typeName);

            foreach (Hero hero in heroes)
            {
                if (hero.GetType() == type) continue;
                DestroyImmediate(hero);
            }


            if (type == null)
            {
                // create new script
                string scriptPath = $"Assets/BaseGame/Scripts/Core/Hero/UniqueHero/{Name.Replace(" ", "")}.cs";
                string code = @$"namespace Core
{{
    public class {Name.Replace(" ", "")} : Hero
    {{

    }}
}}";

                System.IO.File.WriteAllText(scriptPath, code);
                AssetDatabase.ImportAsset(scriptPath);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                Debug.Log("Add script failed. Create new script instead.");
            }
            else if (heroes.Any(hero => hero.GetType() == type))
            {
                Hero hero = editorScope.prefabContentsRoot.GetComponent<Hero>();
                hero.EditorInit(this);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
            else
            {
                Component component = editorScope.prefabContentsRoot.AddComponent(Type.GetType(typeName));
                Hero hero = editorScope.prefabContentsRoot.GetComponent<Hero>();
                hero.EditorInit(this);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                Debug.Log("Add script success.");
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }

    private void GenerateHeroSprite(Dictionary<string, string> data)
    {
        try
        {
            EditorUtility.SetDirty(this);
            HeroSprite = AssetDatabase.LoadAssetAtPath<Sprite>(
                AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets($"t:Sprite {Name}-Idle",
                    new[] { $"Assets/BaseGame/Animations/Hero/{data["Name"]}" })[0]));
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }

    private void GenerateBaseStat(Dictionary<string, string> data)
    {
        try
        {
            EditorUtility.SetDirty(this);
            HeroRarity = (Hero.Rarity)(int.Parse(data["Tier"]) - 1);
            BaseAttackDamage = int.Parse(data["AttackDamage"]);
            BaseAttackSpeed = float.Parse(data["AttackSpeed"]);
            BaseAttackRange = float.Parse(data["AttackRange"]);
            UpgradePercentage = float.Parse(data["UpgradePercentage"]);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private void GenerateHeroJob(Dictionary<string, string> data)
    {
        try
        {
            EditorUtility.SetDirty(this);
            List<Hero.Job> jobList = new List<Hero.Job>();
            if (Enum.TryParse<Hero.Job>(data["MainRace"], out var mainRace))
            {
                jobList.Add(mainRace);
            }

            if (Enum.TryParse<Hero.Job>(data["SubRace1"], out var subRace1))
            {
                jobList.Add(subRace1);
            }

            if (Enum.TryParse<Hero.Job>(data["SubRace2"], out var subRace2))
            {
                jobList.Add(subRace2);
            }

            HeroJob = jobList.ToArray();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private void GenerateHeroClass(Dictionary<string, string> data)
    {
        try
        {
            EditorUtility.SetDirty(this);
            HeroClass = Enum.Parse<Hero.Class>(data["Class"]);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private void GenerateIdleAnimData(Dictionary<string, string> data)
    {
        AnimationClip idleAnim;
        try
        {
            string animIdleGuid = AssetDatabase.FindAssets($"t:AnimationClip {data["Name"]}-Idle-Animation",
                new[] { $"Assets/BaseGame/Animations/Hero/{data["Name"]}" })[0];
            idleAnim = AssetDatabase.LoadAssetAtPath<AnimationClip>(AssetDatabase.GUIDToAssetPath(animIdleGuid));
        }
        catch (Exception e)
        {
            idleAnim = null;
        }

        string[] spriteAnim = AssetDatabase.FindAssets($"t:Sprite {data["Name"]}-Idle",
            new[] { $"Assets/BaseGame/Animations/Hero/{data["Name"]}" });
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
            string animAttackGuid = AssetDatabase.FindAssets($"t:AnimationClip {data["Name"]}-Attack-Animation",
                new[] { $"Assets/BaseGame/Animations/Hero/{data["Name"]}" })[0];
            attackAnim = AssetDatabase.LoadAssetAtPath<AnimationClip>(AssetDatabase.GUIDToAssetPath(animAttackGuid));
        }
        catch (Exception e)
        {
            attackAnim = null;
        }

        string[] spriteAnim = AssetDatabase.FindAssets($"t:Sprite {data["Name"]}-Attack",
            new[] { $"Assets/BaseGame/Animations/Hero/{data["Name"]}" });
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

    private void GenerateSkill1AnimData(Dictionary<string, string> data)
    {
        AnimationClip skillAnim;
        try
        {
            string animSkillGuid = AssetDatabase.FindAssets($"t:AnimationClip {data["Name"]}-Skill1-Animation",
                new[] { $"Assets/BaseGame/Animations/Hero/{data["Name"]}" })[0];
            skillAnim = AssetDatabase.LoadAssetAtPath<AnimationClip>(AssetDatabase.GUIDToAssetPath(animSkillGuid));
        }
        catch (Exception e)
        {
            skillAnim = null;
        }

        string[] spriteAnim = AssetDatabase.FindAssets($"t:Sprite {data["Name"]}-Skill1",
            new[] { $"Assets/BaseGame/Animations/Hero/{data["Name"]}" });
        List<Sprite> spriteList = spriteAnim.Select(AssetDatabase.GUIDToAssetPath)
            .Select(AssetDatabase.LoadAssetAtPath<Sprite>)
            .ToList();
        if (skillAnim == null)
        {
            if (spriteAnim.Length == 0) return;
            skillAnim = new AnimationClip
            {
                name = $"{data["Name"]}-Skill1-Animation",
                frameRate = 30
            };
            if (!AssetDatabase.IsValidFolder($"Assets/BaseGame/Animations/Hero/{data["Name"]}"))
            {
                AssetDatabase.CreateFolder("Assets/BaseGame/Animations/Hero", data["Name"]);
            }

            AssetDatabase.CreateAsset(skillAnim,
                $"Assets/BaseGame/Animations/Hero/{data["Name"]}/{data["Name"]}-Skill1-Animation.anim");
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
            $"Assets/BaseGame/Animations/Hero/{data["Name"]}/{data["Name"]}-Skill1-Animation.anim");
    }

    private void GenerateSkill2AnimData(Dictionary<string, string> data)
    {
        AnimationClip skillAnim;
        try
        {
            string animSkillGuid = AssetDatabase.FindAssets($"t:AnimationClip {data["Name"]}-Skill2-Animation",
                new[] { $"Assets/BaseGame/Animations/Hero/{data["Name"]}" })[0];
            skillAnim = AssetDatabase.LoadAssetAtPath<AnimationClip>(AssetDatabase.GUIDToAssetPath(animSkillGuid));
        }
        catch (Exception e)
        {
            skillAnim = null;
        }

        string[] spriteAnim = AssetDatabase.FindAssets($"t:Sprite {data["Name"]}-Skill2",
            new[] { $"Assets/BaseGame/Animations/Hero/{data["Name"]}" });
        List<Sprite> spriteList = spriteAnim.Select(AssetDatabase.GUIDToAssetPath)
            .Select(AssetDatabase.LoadAssetAtPath<Sprite>)
            .ToList();
        if (skillAnim == null)
        {
            if (spriteAnim.Length == 0) return;
            skillAnim = new AnimationClip
            {
                name = $"{data["Name"]}-Skill2-Animation",
                frameRate = 30
            };
            if (!AssetDatabase.IsValidFolder($"Assets/BaseGame/Animations/Hero/{data["Name"]}"))
            {
                AssetDatabase.CreateFolder("Assets/BaseGame/Animations/Hero", data["Name"]);
            }

            AssetDatabase.CreateAsset(skillAnim,
                $"Assets/BaseGame/Animations/Hero/{data["Name"]}/{data["Name"]}-Skill2-Animation.anim");
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
            $"Assets/BaseGame/Animations/Hero/{data["Name"]}/{data["Name"]}-Skill2-Animation.anim");
    }

    private void GenerateSkill3AnimData(Dictionary<string, string> data)
    {
        AnimationClip skillAnim;
        try
        {
            string animSkillGuid = AssetDatabase.FindAssets($"t:AnimationClip {data["Name"]}-Skill3-Animation",
                new[] { $"Assets/BaseGame/Animations/Hero/{data["Name"]}" })[0];
            skillAnim = AssetDatabase.LoadAssetAtPath<AnimationClip>(AssetDatabase.GUIDToAssetPath(animSkillGuid));
        }
        catch (Exception e)
        {
            skillAnim = null;
        }

        string[] spriteAnim = AssetDatabase.FindAssets($"t:Sprite {data["Name"]}-Skill3",
            new[] { $"Assets/BaseGame/Animations/Hero/{data["Name"]}" });
        List<Sprite> spriteList = spriteAnim.Select(AssetDatabase.GUIDToAssetPath)
            .Select(AssetDatabase.LoadAssetAtPath<Sprite>)
            .ToList();
        if (skillAnim == null)
        {
            if (spriteAnim.Length == 0) return;
            skillAnim = new AnimationClip
            {
                name = $"{data["Name"]}-Skill3-Animation",
                frameRate = 30
            };
            if (!AssetDatabase.IsValidFolder($"Assets/BaseGame/Animations/Hero/{data["Name"]}"))
            {
                AssetDatabase.CreateFolder("Assets/BaseGame/Animations/Hero", data["Name"]);
            }

            AssetDatabase.CreateAsset(skillAnim,
                $"Assets/BaseGame/Animations/Hero/{data["Name"]}/{data["Name"]}-Skill3-Animation.anim");
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
            $"Assets/BaseGame/Animations/Hero/{data["Name"]}/{data["Name"]}-Skill3-Animation.anim");
    }

    private void GenerateAnimatorData(Dictionary<string, string> data)
    {
        AnimatorController animatorController;
        try
        {
            string animControllerGuid = AssetDatabase.FindAssets(
                $"t:AnimatorController {data["Name"]}-AnimatorController",
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


        AnimationClip skill1Clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(
            $"Assets/BaseGame/Animations/Hero/{data["Name"]}/{data["Name"]}-Skill1-Animation.anim");
        if (skill1Clip != null)
        {
            AnimatorState skillState = stateMachine.AddState("Skill1");
            skillState.motion = skill1Clip;
            skillState.speedParameterActive = true;
            skillState.speedParameter = "TickRate";
            AnimatorStateTransition skillToIdle = skillState.AddTransition(idleState);
            skillToIdle.hasExitTime = true;
            skillToIdle.exitTime = 1;
            skillToIdle.hasFixedDuration = false;
            skillToIdle.duration = 0;
            skillToIdle.offset = 0;
        }

        AnimationClip skill2Clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(
            $"Assets/BaseGame/Animations/Hero/{data["Name"]}/{data["Name"]}-Skill2-Animation.anim");
        if (skill2Clip != null)
        {
            AnimatorState skillState = stateMachine.AddState("Skill2");
            skillState.motion = skill2Clip;
            skillState.speedParameterActive = true;
            skillState.speedParameter = "TickRate";
            AnimatorStateTransition skillToIdle = skillState.AddTransition(idleState);
            skillToIdle.hasExitTime = true;
            skillToIdle.exitTime = 1;
            skillToIdle.hasFixedDuration = false;
            skillToIdle.duration = 0;
            skillToIdle.offset = 0;
        }
        
        AnimationClip skill3Clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(
            $"Assets/BaseGame/Animations/Hero/{data["Name"]}/{data["Name"]}-Skill3-Animation.anim");
        if (skill3Clip != null)
        {
            AnimatorState skillState = stateMachine.AddState("Skill3");
            skillState.motion = skill3Clip;
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
            string animIdleGuid = AssetDatabase.FindAssets($"t:AnimationClip {data["Name"]}-Idle-Image-Animation",
                new[] { $"Assets/BaseGame/Animations/Hero/{data["Name"]}" })[0];
            idleAnim = AssetDatabase.LoadAssetAtPath<AnimationClip>(AssetDatabase.GUIDToAssetPath(animIdleGuid));
        }
        catch (Exception e)
        {
            idleAnim = null;
        }

        string[] spriteAnim = AssetDatabase.FindAssets($"t:Sprite {data["Name"]}-Idle",
            new[] { $"Assets/BaseGame/Animations/Hero/{data["Name"]}" });
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

    private void GenerateRunImageAnimData(Dictionary<string, string> data)
    {
        AnimationClip runAnim;
        try
        {
            string animIdleGuid = AssetDatabase.FindAssets($"t:AnimationClip {data["Name"]}-Run-Image-Animation",
                new[] { $"Assets/BaseGame/Animations/Hero/{data["Name"]}" })[0];
            runAnim = AssetDatabase.LoadAssetAtPath<AnimationClip>(AssetDatabase.GUIDToAssetPath(animIdleGuid));
        }
        catch (Exception e)
        {
            runAnim = null;
        }

        string[] spriteAnim = AssetDatabase.FindAssets($"t:Sprite {data["Name"]}-Run",
            new[] { $"Assets/BaseGame/Animations/Hero/{data["Name"]}" });
        List<Sprite> spriteList = spriteAnim.Select(AssetDatabase.GUIDToAssetPath)
            .Select(AssetDatabase.LoadAssetAtPath<Sprite>)
            .ToList();
        if (runAnim == null)
        {
            if (spriteAnim.Length == 0) return;
            runAnim = new AnimationClip
            {
                name = $"{data["Name"]}-Run-Image-Animation",
                frameRate = 30
            };
            if (!AssetDatabase.IsValidFolder($"Assets/BaseGame/Animations/Hero/{data["Name"]}"))
            {
                AssetDatabase.CreateFolder("Assets/BaseGame/Animations/Hero", data["Name"]);
            }

            AssetDatabase.CreateAsset(runAnim,
                $"Assets/BaseGame/Animations/Hero/{data["Name"]}/{data["Name"]}-Run-Image-Animation.anim");
            AssetDatabase.SaveAssets();
        }
        else
        {
            runAnim.ClearCurves();
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
                time = i / runAnim.frameRate,
                value = spriteList[i]
            };
        }

        AnimationUtility.SetObjectReferenceCurve(runAnim, curveBinding, keyFrames);

        runAnim = AssetDatabase.LoadAssetAtPath<AnimationClip>(
            $"Assets/BaseGame/Animations/Hero/{data["Name"]}/{data["Name"]}-Run-Image-Animation.anim");
        // set loop animation
        AnimationClipSettings clipSettings = AnimationUtility.GetAnimationClipSettings(runAnim);
        clipSettings.loopTime = true;
        AnimationUtility.SetAnimationClipSettings(runAnim, clipSettings);
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

        AnimationClip runClip = AssetDatabase.LoadAssetAtPath<AnimationClip>(
            $"Assets/BaseGame/Animations/Hero/{data["Name"]}/{data["Name"]}-Run-Image-Animation.anim");
        AnimatorState runState = stateMachine.AddState("Run");
        runState.motion = runClip;
    }
#endif
}