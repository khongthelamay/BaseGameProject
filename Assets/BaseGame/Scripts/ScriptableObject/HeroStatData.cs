using System.Collections.Generic;
using Core;
using Sirenix.OdinInspector;
using Spine.Unity;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif
[CreateAssetMenu(fileName = "HeroStatData", menuName = "ScriptableObjects/HeroStatData")]
[InlineEditor]
public class HeroStatData : ScriptableObject
{
    [field: HorizontalGroup(nameof(HeroStatData), width: 200), PreviewField(200), HideLabel]
    [field: SerializeField]
    [field: ShowIf("@HeroSprite != null")]
    public Sprite HeroSprite {get; set;}
    [field: HorizontalGroup(nameof(HeroStatData), width: 200), PreviewField(200), HideLabel]
    [field: SerializeField]
    [field: ShowIf("@HeroSkeletonDataAsset != null")]
    public SkeletonDataAsset HeroSkeletonDataAsset {get; set;}
    [field: VerticalGroup(nameof(HeroStatData)+"/Stats")]
    [field: SerializeField] public string Name {get; set;}
    [field: VerticalGroup(nameof(HeroStatData)+"/Stats")]
    [field: SerializeField] public Hero.Rarity HeroRarity {get; set;}
    [field: VerticalGroup(nameof(HeroStatData)+"/Stats")]
    [field: SerializeField] public Hero.Class HeroClass {get; set;}
    [field: VerticalGroup(nameof(HeroStatData)+"/Stats")]
    [field: SerializeField] public Hero.Job[] HeroJob {get; set;}
    [field: VerticalGroup(nameof(HeroStatData)+"/Stats")]
    [field: SerializeField] public int BaseAttackDamage {get; set;}
    [field: VerticalGroup(nameof(HeroStatData)+"/Stats")]
    [field: SerializeField] public float BaseAttackSpeed {get; set;}
    [field: VerticalGroup(nameof(HeroStatData)+"/Stats")]
    [field: SerializeField] public float BaseAttackRange {get; set;}
    [field: VerticalGroup(nameof(HeroStatData)+"/Stats")]
    [field: SerializeField] public float UpgradePercentage {get; set;}
    
    [field: SerializeField] public List<Ability> HeroAbilities {get; set;}

    [Button]
    private void UpdateName()
    {
#if UNITY_EDITOR
        string newName = $"{HeroRarity}_{Name}.asset";
        string assetPath = AssetDatabase.GetAssetPath(this);
        AssetDatabase.RenameAsset(assetPath, newName);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
#endif
    }
}