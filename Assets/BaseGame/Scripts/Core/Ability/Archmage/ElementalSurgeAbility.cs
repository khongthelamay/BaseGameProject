using System.Threading;
using BaseGame.Scripts.Enum;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Core
{
    public class ElementalSurgeAbility : ActiveAbility
    {
        [field: SerializeField] public DamageType DamageType { get; private set; } = DamageType.Magical;
        [field: SerializeField] public Projectile Projectile {get; private set;}
        [field: SerializeField] public Transform SpawnPosition {get; private set;}

        
        public ElementalSurgeAbility(Hero owner, int levelUnlock) : base(owner, levelUnlock)
        {
        }

        public override void OnEnterBattleField()
        {
            
        }

        public override void OnExitBattleField()
        {
            
        }

        public override bool CanUseAbility()
        {
            return true;
        }

        public override UniTask UseAbility(TickRate tickRate, CancellationToken ct)
        {
            return UniTask.CompletedTask;
        }
    }
}