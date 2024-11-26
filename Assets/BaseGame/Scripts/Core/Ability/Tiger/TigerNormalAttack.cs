using System.Threading;
using Cysharp.Threading.Tasks;
using TW.Utility.CustomType;
using UnityEngine;

namespace Core.TigerAbility
{
    [System.Serializable]
    public class TigerNormalAttack : MeleeAttackAbility
    {
        private Tiger OwnerTiger { get; set; }
        public TigerNormalAttack()
        {
            
        }
        
        public TigerNormalAttack(Hero owner) : base(owner)
        {
            OwnerTiger = owner as Tiger;
        }
        
        public override Ability Clone(Hero owner)
        {
            return new TigerNormalAttack(owner)
            {
                DamageType = DamageType,
                DelayFrame = DelayFrame,
                Description = Description,
                LevelUnlock = LevelUnlock
            };
        }
    }
}