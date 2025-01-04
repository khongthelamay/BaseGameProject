using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Core
{
    [CreateAssetMenu(fileName = "BearNormalAttackAbility", menuName = "Ability/Bear/BearNormalAttackAbility")]
    public class BearNormalAttackAbility : MeleeAttackAbility
    {
        public override void OnAttackComplete()
        {
            base.OnAttackComplete();
            if (Owner.TryGetAbility(out RendingClawsAbility rendingClawsAbility))
            {
                rendingClawsAbility.AddAttackCount();
            }
        }
    }
}