using System.Collections.Generic;
using Core.SimplePool;
using UnityEngine;

namespace Core
{
    public class Archmage : Hero
    {
        [field: SerializeField] public Transform ProjectileSpawnPosition { get; private set; }
        [field: SerializeField] public Projectile NormalAttackProjectile { get; private set; }
        [field: SerializeField] public Transform OrbContainer { get; private set; }
        [field: SerializeField] public VisualEffect ElementalSurgeEffect { get; private set; }
        [field: SerializeField] public VisualEffect ChargeEffect { get; private set; }
        [field: SerializeField] public ArchmageOrb FireOrb { get; private set; }
        [field: SerializeField] public ArchmageOrb IceOrb { get; private set; }
        [field: SerializeField] public ArchmageOrb DarkOrb { get; private set; }
        public int OrbSpawnRate { get; private set; } 
        private List<ArchmageOrb> OrbList { get; set; } = new();
        public int FireOrbCount { get; private set; }
        public int IceOrbCount { get; private set; }
        public int DarkOrbCount { get; private set; }

        protected override void InitAbility()
        {
            base.InitAbility();
            // ActiveAbilities.Add(new ElementalSurgeAbility(this, 5, ElementalSurgeEffect));
            // ActiveAbilities.Add(new ArchmageNormalAttack(this, NormalAttackProjectile, ProjectileSpawnPosition));
            //
            // PassiveAbilities.Add(new OrbCatalystAbility(this));
        }
        public void AddOrbSpawnRate(int value)
        {
            OrbSpawnRate += value;
        }
        public void SpawnRandomOrb()
        {
            ArchmageOrb orbPrefab = null;
            switch (Random.Range(0, 3))
            {
                case 0:
                    orbPrefab = FireOrb;
                    FireOrbCount++;
                    break;
                case 1:
                    orbPrefab = IceOrb;
                    IceOrbCount++;
                    break;
                case 2:
                    orbPrefab = DarkOrb;
                    DarkOrbCount++;
                    break;
            }

            OrbList.Add(orbPrefab.Spawn(Transform.position, Quaternion.identity, OrbContainer)
                .Setup(this)
                .MovingAround());
        }

        public void ConsumeAllOrb()
        {
            ChargeEffect.Spawn(OrbContainer.position, Quaternion.identity);
            foreach (var archmageOrb in OrbList)
            {
                archmageOrb.Consumed();
            }
            OrbList.Clear();
            FireOrbCount = 0;
            IceOrbCount = 0;
            DarkOrbCount = 0;
        }
    }
}