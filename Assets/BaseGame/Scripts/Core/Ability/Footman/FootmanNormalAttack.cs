using System.Threading;
using Cysharp.Threading.Tasks;
using TW.Utility.CustomType;
using UnityEngine;

namespace Core
{
    // public class FootmanNormalAttack : ActiveAbility
    // {
    //     [field: SerializeField] private DamageType DamageType {get; set;} = DamageType.Physical;
    //
    //     private Enemy EnemyTarget { get; set; }
    //     private Footman OwnerFootman { get; set; }
    //     public FootmanNormalAttack(Hero owner) : base(owner)
    //     {
    //         OwnerFootman = (Footman) owner;
    //     }
    //     public override bool CanUseAbility()
    //     {
    //         if (!BattleManager.TryGetEnemyInAttackRange(Owner, out Enemy target)) return false;
    //         EnemyTarget = target;
    //         return true;
    //     }
    //
    //     public override void OnEnterBattleField()
    //     {
    //         
    //     }
    //
    //     public override void OnExitBattleField()
    //     {
    //         
    //     }
    //
    //
    //     public override async UniTask UseAbility(TickRate tickRate, CancellationToken ct)
    //     {
    //         BigNumber damageDeal = Owner.AttackDamage;
    //         float attackSpeed = Owner.AttackSpeed;
    //         
    //         EnemyTarget.WillTakeDamage(damageDeal);
    //         Owner.SetFacingPosition(EnemyTarget.Transform.position);
    //         Owner.HeroAnim.PlayAttackAnimation(attackSpeed);
    //         await DelaySample(6, tickRate, ct);
    //
    //         EnemyTarget.TakeDamage(damageDeal, DamageType);
    //         await DelaySample(24, tickRate, ct);
    //
    //     }
    //
    //     public override Ability Clone()
    //     {
    //         return new FootmanNormalAttack(Owner);
    //     }
    // }
}