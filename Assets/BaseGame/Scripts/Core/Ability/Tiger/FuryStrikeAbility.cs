using System;
using System.Numerics;
using System.Threading;
using BaseGame.Scripts.Enum;
using Cysharp.Threading.Tasks;
using Manager;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Core
{
    public class FuryStrikeAbility : ActiveAbility
    {
        [field: SerializeField] private int SampleDelay {get; set;}
        [field: SerializeField] private DamageType DamageType {get; set;}
        [field: SerializeField] private int FuryGainRate {get; set;}

        private Enemy EnemyTarget { get; set; }
        private Tiger OwnerTiger { get; set; }
        public FuryStrikeAbility(Hero owner, int levelUnlock, int sampleDelay,DamageType damageType, int furyGainRate) : base(owner, levelUnlock)
        {
            SampleDelay = sampleDelay;
            DamageType = damageType;
            FuryGainRate = furyGainRate;
            OwnerTiger = (Tiger) owner;
        }
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


        public override async UniTask UseAbility(TickRate tickRate, CancellationToken ct)
        {
            Owner.HeroAnim.PlayAttackAnimation(Owner.AttackSpeed);
            await DelaySample(SampleDelay, tickRate, ct);
            BigInteger attackDamage = (BigInteger)(Owner.AttackDamage * (1 + BattleManager.GetGlobalBuff(GlobalBuff.Type.AttackDamage).Value/100));
            EnemyTarget.TakeDamage(attackDamage, DamageType);
            int random = Random.Range(0, 100);
            if (random < FuryGainRate + OwnerTiger.BonusFuryStrikeRate)
            {
                OwnerTiger.AddFuryPoint(1);
            }
            await DelaySample(30 - SampleDelay, tickRate, ct);

        }
    }
}