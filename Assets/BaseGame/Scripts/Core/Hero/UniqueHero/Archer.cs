using BaseGame.Scripts.Enum;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Core
{
    public class Archer : Hero
    {
        [field: Title(nameof(Archer))]
        [field: SerializeField] public Transform ProjectileSpawnPosition {get; private set;}
        [field: SerializeField] public Projectile ArrowStormProjectile {get; private set;}
        [field: SerializeField] public Projectile PiercingShotProjectile {get; private set;}
        [field: SerializeField] public Projectile RangeAttackProjectile {get; private set;}
        protected override void InitAbility()
        {
            ActiveAbilities.Add(new ArrowStormAbility(this, 0, ArrowStormProjectile, ProjectileSpawnPosition ));
            ActiveAbilities.Add(new PiercingShotAbility(this, 0, PiercingShotProjectile, ProjectileSpawnPosition));
            ActiveAbilities.Add(new RangeAttackAbility(this, 0, RangeAttackProjectile, ProjectileSpawnPosition));
        }
        
    }
}