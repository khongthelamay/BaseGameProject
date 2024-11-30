using System.Threading;
using Core.SimplePool;
using Cysharp.Threading.Tasks;
using Manager;
using TW.Utility.CustomType;
using UnityEngine;

namespace Core
{
    // public class DevastatingStrikeAbility : ActiveAbility
    // {
    //     [field: SerializeField] private DamageType DamageType {get; set;} = DamageType.Physical;
    //     [field: SerializeField] private int Rate {get; set;} = 10;
    //     [field: SerializeField] private int DamageScale { get; set; } = 500;
    //     [field: SerializeField] private VisualEffect VisualEffect {get; set;}
    //     private Enemy EnemyTarget { get; set; }
    //     private Footman OwnerFootman { get; set; }
    //     public DevastatingStrikeAbility(Hero owner, VisualEffect visualEffect) : base(owner)
    //     {
    //         VisualEffect = visualEffect;
    //         OwnerFootman = (Footman) owner;
    //     }
    //     public override bool CanUseAbility()
    //     {
    //         if (Random.Range(0, 100) >= Rate) return false;
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
    //         BigNumber damageDeal = Owner.AttackDamage * DamageScale;
    //         float attackSpeed = Owner.AttackSpeed;
    //         
    //         EnemyTarget.WillTakeDamage(damageDeal);
    //         Owner.SetFacingPosition(EnemyTarget.Transform.position);
    //         Owner.HeroAnim.PlayAttackAnimation(attackSpeed);
    //         await DelaySample(6, tickRate, ct);
    //         
    //         VisualEffect.Spawn(EnemyTarget.Transform.position, Quaternion.identity);
    //         EnemyTarget.TakeDamage(damageDeal, DamageType);
    //         await DelaySample(24, tickRate, ct);
    //
    //     }
    //
    //     public override Ability Clone()
    //     {
    //         return new DevastatingStrikeAbility(Owner, VisualEffect);
    //     }
    // }
}