using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Core
{
    public abstract class RandomnessAbility : ActiveAbility
    {
        [field: SerializeField] public float TriggerRate {get; private set;}
        protected override void SetupGroup()
        {
            base.SetupGroup();
            AbilityGroup |= Group.Randomness;
        }

        public override bool CanUseAbility()
        {
            return TriggerRate >= Random.Range(0, 100);
        }
    }
}