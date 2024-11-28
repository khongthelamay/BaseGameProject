using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Core
{
    [System.Serializable]
    public abstract class NormalAttackAbility : ActiveAbility
    {
        [field: SerializeField] public DamageType DamageType {get; set;}
        [field: SerializeField] public int DelayFrame {get; set;}
        protected Enemy EnemyTarget { get; set; }

        public override bool CanUseAbility()
        {
            if (!BattleManager.TryGetEnemyInAttackRange(Owner, out Enemy target)) return false;
            EnemyTarget = target;
            return true;
        }

        public override void OnEnterBattleField()
        {
            
        }

        public override void OnExitBattleField()
        {
            
        }


        public override UniTask UseAbility(TickRate tickRate, CancellationToken ct)
        {
            return UniTask.CompletedTask;
        }

        public virtual void OnAttackComplete()
        {

        }

        // public override void Clone(Ability ability)
        // {
        //     base.Clone(ability);
        //     if (ability is not NormalAttackAbility normalAttackAbility) return;
        //     DamageType = normalAttackAbility.DamageType;
        //     DelayFrame = normalAttackAbility.DelayFrame;
        // }
    }
}