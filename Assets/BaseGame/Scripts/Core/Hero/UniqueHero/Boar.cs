using UnityEngine;

namespace Core
{
    public class Boar : Hero
    {
        [field: SerializeField] public Transform ProjectileSpawnPosition {get; private set;}

    }
}