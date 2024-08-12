using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "HeroStatData", menuName = "ScriptableObjects/HeroStatData")]
[InlineEditor]
public class HeroStatData : ScriptableObject
{
    [field: SerializeField] private Hero.Rarity HeroRarity {get; set;}
    [field: SerializeField] public Hero.Trait HeroTrait {get; private set;}
    [field: SerializeField] public Hero.Race HeroRace {get; private set;}
    [field: SerializeField] public int HeroWeight {get; private set;}
    [field: SerializeField] public int AttackDamage {get; private set;}
    [field: SerializeField] public float AttackSpeed {get; private set;}

}