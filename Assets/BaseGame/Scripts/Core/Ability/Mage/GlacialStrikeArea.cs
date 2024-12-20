using System.Threading;
using Core.SimplePool;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using UnityEngine;

namespace Core
{
    public class GlacialStrikeArea : DamageOverTimeArea
    {
        private static readonly int Axis = Animator.StringToHash("Axis");
        [field: SerializeField] public Animator Animator {get; private set;}
        [field: SerializeField] public string AnimationName {get; private set;}

        protected override void OnDamageOverTimeTick(Enemy enemy, int enemyId)
        {
            base.OnDamageOverTimeTick(enemy, enemyId);
            enemy.AddStatusEffect(new IceSlowStatusEffect(100, TickRate + 0.1f));
        }

        public override void StopDamageOverTime()
        {
            base.StopDamageOverTime();
        }
        

        public override DamageOverTimeArea WithAxis(int axis)
        {
            Animator.SetFloat(Axis, axis);
            return base.WithAxis(axis);
        }
    }
}