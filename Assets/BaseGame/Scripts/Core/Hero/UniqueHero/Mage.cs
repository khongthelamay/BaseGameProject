using UnityEngine;

namespace Core
{
    public class Mage : Hero
    {
        [field: SerializeField] public Transform ProjectileSpawnPosition {get; private set;}
        
    }
}