using System.Threading;
using Core.SimplePool;
using Cysharp.Threading.Tasks;
using LitMotion;
using TW.ACacheEverything;
using TW.Utility.CustomType;
using UnityEngine;

namespace Core
{
    [CreateAssetMenu(fileName = "ArrowStormAbility", menuName = "Ability/Archer/ArrowStormAbility")]
    public class ArrowStormAbility : ActiveCooldownAbility, IAbilityTargetAnEnemy
    {
        [field: SerializeField] public int DevastatingStrikeRate {get; private set;}
        [field: SerializeField] public DamageType DamageType {get; private set;}
        [field: SerializeField] public int DamageDelayFrame {get; private set;}
        private Transform SpawnPosition {get; set;}
        public Enemy EnemyTarget { get; set; }
        public int EnemyTargetId { get; set; }
        private Archer OwnerArcher { get; set; }

        public override Ability WithOwnerHero(Hero owner)
        {
            OwnerArcher = owner as Archer;
            SpawnPosition = OwnerArcher?.ProjectileSpawnPosition;

            return base.WithOwnerHero(owner);
        }
        public override void OnEnterBattleField()
        {
            
        }

        public override void OnExitBattleField()
        {
            
        }

        public override bool CanUseAbility()
        {
            // if (Random.Range(0, 100) >= DevastatingStrikeRate) return false;
            // return this.IsFindAnyEnemyTarget();
            return false;
        }

        public override UniTask UseAbility(TickRate tickRate, CancellationToken ct)
        {
            return UniTask.CompletedTask;
        }


    }
}