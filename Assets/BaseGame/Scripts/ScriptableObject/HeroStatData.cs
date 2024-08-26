using Sirenix.OdinInspector;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif
[CreateAssetMenu(fileName = "HeroStatData", menuName = "ScriptableObjects/HeroStatData")]
[InlineEditor]
public class HeroStatData : ScriptableObject
{
    [field: HorizontalGroup(nameof(HeroStatData), width: 150), PreviewField(150), HideLabel]
    [field: SerializeField]
    public Sprite HeroSprite {get; set;}
    [field: VerticalGroup(nameof(HeroStatData)+"/Stats")]
    [field: SerializeField] public string Name {get; set;}
    [field: VerticalGroup(nameof(HeroStatData)+"/Stats")]
    [field: SerializeField] public Hero.Rarity HeroRarity {get; set;}
    [field: VerticalGroup(nameof(HeroStatData)+"/Stats")]
    [field: SerializeField] public Hero.Job[] HeroJob {get; set;}
    [field: VerticalGroup(nameof(HeroStatData)+"/Stats")]
    [field: SerializeField] public int BaseAttackDamage {get; set;}
    [field: VerticalGroup(nameof(HeroStatData)+"/Stats")]
    [field: SerializeField] public float BaseAttackSpeed {get; set;}
    [field: VerticalGroup(nameof(HeroStatData)+"/Stats")]
    [field: SerializeField] public float BaseAttackRange {get; set;}
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