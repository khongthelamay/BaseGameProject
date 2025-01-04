using System.Threading;
using Core.GameStatusEffect;
using Core.SimplePool;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using TW.Utility.CustomType;
using UnityEngine;

namespace Core
{
    public class GlacialStrikeArea : DamageOverTimeArea
    {
        [field: SerializeField] public float StunDuration {get; private set;}

        [field: SerializeField] public SpriteForceDurationVisualEffect GroundEffect {get; private set;}
        [field: SerializeField] public SpriteForceDurationVisualEffect IceEffect {get; private set;}

        private bool IsFirstTickDamage { get; set; }

        
        public GlacialStrikeArea WithStunDuration(float stunDuration)
        {
            StunDuration = stunDuration;
            return this;
        }

        public override void StartDamageOverTimeHandle()
        {
            GroundEffect.Spawn(Transform.position, Quaternion.identity, Transform)
                .WithAxis(Range.Axis)
                .WithDuration(Duration)
                .Play();
            base.StartDamageOverTimeHandle();
        }

        protected override UniTask StartDamageOverTime()
        {
            IsFirstTickDamage = true;
            return base.StartDamageOverTime();
        }

        protected override void OnDamageOverTimeInterval(Enemy[] enemies, int[] enemiesId)
        {
            base.OnDamageOverTimeInterval(enemies, enemiesId);
            for (int i = 0; i < EnemiesCount; i++)
            {
                if (enemies[i].Id != enemiesId[i]) continue;
                enemies[i].AddStatusEffect(new IceSlowStatusEffect(50, Interval + 0.1f));
            }
            if (!IsFirstTickDamage) return;
            for (int i = 0; i < EnemiesCount; i++)
            {
                if (enemies[i].Id != enemiesId[i]) continue;
                enemies[i].AddStatusEffect(new IceStunStatusEffect(StunDuration, IceEffect));
            }
            IsFirstTickDamage = false;
        }
    }
}