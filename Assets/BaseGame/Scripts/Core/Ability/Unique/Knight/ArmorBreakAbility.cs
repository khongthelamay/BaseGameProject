using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Core
{
    [CreateAssetMenu(fileName = "ArmorBreakAbility", menuName = "Ability/Knight/ArmorBreakAbility")]
    public class ArmorBreakAbility : PassiveAbility
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