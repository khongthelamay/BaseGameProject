using UnityEngine;

namespace Core
{
    public class Queen : Hero
    {
        [field: SerializeField] public Transform ProjectileSpawnPosition { get; private set; }
        [field: SerializeField] public Projectile NormalAttackProjectile { get; private set; }

        [field: SerializeField] public VisualEffect QueenNormalAttackEffect {get; private set;}

        protected override void InitAbility()
        {
            base.InitAbility();
            // ActiveAbilities.Add(new StarfallAbility(this, 0, 5f));
            ActiveAbilities.Add(new QueenNormalAttack(this, 0, NormalAttackProjectile, ProjectileSpawnPosition));
            
            PassiveAbilities.Add(new AuraOfCelerityAbility(this, 0));
            PassiveAbilities.Add(new CooldownSurgeAbility(this, 0));
        }
    }
}