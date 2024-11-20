using Core.SimplePool;
using UnityEngine;

namespace Core
{
    public class Queen : Hero
    {
        [field: SerializeField] public Transform ProjectileSpawnPosition { get; private set; }
        [field: SerializeField] public Projectile NormalAttackProjectile { get; private set; }

        [field: SerializeField] public VisualEffect CooldownSurgeEffect {get; private set;}
        private Hero[] AlliesAround { get; set; } = new Hero[30];
        public float CooldownSurgeRate { get; private set; }
        public float CooldownSurgeDecrease { get; private set; }
        protected override void InitAbility()
        {
            base.InitAbility();
            ActiveAbilities.Add(new StarfallAbility(this, 0, 5f));
            ActiveAbilities.Add(new QueenNormalAttack(this, 0, NormalAttackProjectile, ProjectileSpawnPosition));
            
            PassiveAbilities.Add(new AuraOfCelerityAbility(this, 0));
            PassiveAbilities.Add(new CooldownSurgeAbility(this, 0));
        }
        public void ChangeCooldownSurgeRate(float rate)
        {
            CooldownSurgeRate += rate;
        }
        public void ChangeCooldownSurgeDecrease(float rate)
        {
            CooldownSurgeDecrease += rate;
        }
        public void ReduceCooldownAllHeroAround(float rate)
        {
            int count = BattleManager.GetAlliesAroundNonAlloc(this,  AlliesAround);
            for (int i = 0; i < count; i++)
            {
                if (AlliesAround[i].TryReduceCooldown(rate))
                {
                    CooldownSurgeEffect.Spawn(AlliesAround[i].Transform.position, Quaternion.identity);
                }
            }
        }
    }
}