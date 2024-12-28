using System.Threading;
using Cysharp.Threading.Tasks;
using Manager;
using TW.Utility.CustomType;
using UnityEngine;

namespace Core
{
    [System.Serializable]
    public class NormalAttackAbility : ActiveAbility, IAbilityTargetAnEnemy
    {
        [field: SerializeField] public DamageType DamageType {get; set;}
        [field: SerializeField] public int DelayFrame {get; set;}
        public Enemy EnemyTarget { get; set; }
        public int EnemyTargetId { get; set; }

        public override bool CanUseAbility()
        {
            return this.IsFindAnyEnemyTarget();
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
    }
}