using UnityEngine;

namespace Core
{
    [CreateAssetMenu(fileName = "ArmorShredAbility", menuName = "Ability/Boar/ArmorShredAbility")]
    public class ArmorShredAbility : PassiveAbility
    {
        [field: SerializeField] public int ArmorReduction {get; private set;}

        public override void OnEnterBattleField()
        {
            BattleManager.ChangeGlobalBuff(GlobalBuff.Type.PhysicalArmorReduction, ArmorReduction);
        }

        public override void OnExitBattleField()
        {
            BattleManager.ChangeGlobalBuff(GlobalBuff.Type.PhysicalArmorReduction, -ArmorReduction);
        }
    }
}