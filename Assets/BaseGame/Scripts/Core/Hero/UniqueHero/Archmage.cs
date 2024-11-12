using Core.SimplePool;
using UnityEngine;

namespace Core
{
    public class Archmage : Hero
    {
        [field: SerializeField] public Transform ProjectileSpawnPosition {get; private set;}
        [field: SerializeField] public Projectile AttackProjectile {get; private set;}

        [field: SerializeField] public ArchmageOrb FireOrb {get; private set;}
        [field: SerializeField] public ArchmageOrb IceOrb {get; private set;}
        [field: SerializeField] public ArchmageOrb DarkOrb {get; private set;}

        protected override void InitAbility()
        {
            base.InitAbility();
            ActiveAbilities.Add(new OrbCatalystAbility(this, 0, AttackProjectile, ProjectileSpawnPosition));
        }
        public void SpawnRandomOrb()
        {
            var random = Random.Range(0, 3);
            switch (random)
            {
                case 0:
                    FireOrb.Spawn(Transform.position, Quaternion.identity, Transform).Setup(this);
                    break;
                case 1:
                    IceOrb.Spawn(Transform.position, Quaternion.identity, Transform).Setup(this);
                    break;
                case 2:
                    DarkOrb.Spawn(Transform.position, Quaternion.identity, Transform).Setup(this);
                    break;
            }
        }
    }
}