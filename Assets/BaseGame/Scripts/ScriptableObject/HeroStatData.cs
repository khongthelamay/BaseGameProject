using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "HeroStatData", menuName = "ScriptableObjects/HeroStatData")]
[InlineEditor]
public class HeroStatData : ScriptableObject
{
    [field: SerializeField] public string Name {get; set;}
    [field: SerializeField] public Hero.Rarity HeroRarity {get; set;}
    [field: SerializeField] public Hero.Trait HeroTrait {get; set;}
    [field: SerializeField] public Hero.Race HeroRace {get; set;}
    
    [field: SerializeField] public int HeroWeight {get; set;}
    [field: SerializeField] public int AttackDamage {get; set;}
    [field: SerializeField] public float AttackSpeed {get; set;}

}