using Manager;
using UnityEngine;

namespace Core
{
    public class HeroAnim : MonoBehaviour
    {
        public enum State
        {
            None = 0,
            Idle = 1,
            Attack = 2,
        }

        private static readonly string Idle = "Idle";
        private static readonly string Attack = "Attack";
        private static readonly string Skill = "Skill";
        private static readonly int TickRate = Animator.StringToHash("TickRate");

        [field: SerializeField] public Animator Animator { get; private set; }
        private BattleManager BattleManagerCache { get; set; }
        public BattleManager BattleManager => BattleManagerCache ??= BattleManager.Instance;
        public HeroAnim PlayIdleAnimation(float speed)
        {
            PlayAnimation(Idle, speed);
            return this;
        }

        public HeroAnim PlayAttackAnimation(float speed)
        {
            PlayAnimation(Attack, speed);
            return this;
        }
        
        public HeroAnim PlaySkillAnimation(float speed)
        {
            PlayAnimation(Skill, speed);
            return this;
        }

        private void PlayAnimation(string animationName, float speed)
        {
            if (Animator.runtimeAnimatorController == null) return;
            Animator.SetFloat(TickRate, speed * BattleManager.TickRate.ToValue());
            Animator.Play(animationName);
        }
    }
}