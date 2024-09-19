using System.Threading;
using Cysharp.Threading.Tasks;
using TW.Utility.DesignPattern;
using TW.Utility.Extension;
using UnityEngine;

namespace Core
{
    public class HeroAttackState : SingletonState<Hero, HeroAttackState>
    {
    }

    public partial class Hero : SingletonState<Hero, HeroAttackState>.IHandler
    {
        [field: SerializeField] private float CurrentAttackTimer { get; set; }
        [field: SerializeField] private float CurrentAttackTimeCounter { get; set; }
        [field: SerializeField] public int CurrentAttackCount {get; private set;}
        [field: SerializeField] public float CurrentAttackProbability {get; private set;}
        public UniTask OnStateEnter(HeroAttackState state, CancellationToken token)
        {
            CurrentAttackTimeCounter = CurrentAttackTimer;
            CurrentAttackCount = 1;
            return UniTask.CompletedTask;
        }   

        public UniTask OnStateExecute(HeroAttackState state, CancellationToken token)
        {
            CurrentAttackTimer = 1f / HeroStatData.BaseAttackSpeed;
            if (CurrentAttackTimeCounter > CurrentAttackTimer)
            {
                CurrentAttackProbability = Random.Range(0f, 100f);
                if (TryTriggerAbility(Ability.Trigger.ProbabilityAttack))
                {
                    CurrentAttackTimeCounter -= CurrentAttackTimer;
                    CurrentAttackCount = AIntExtension.Repeat(CurrentAttackCount + 1, 720720);
                }
                else if (TryTriggerAbility(Ability.Trigger.SpecialAttack))
                {
                    CurrentAttackTimeCounter -= CurrentAttackTimer;
                    CurrentAttackCount = AIntExtension.Repeat(CurrentAttackCount + 1, 720720);
                }
                else if (TryTriggerAbility(Ability.Trigger.NormalAttack))
                {
                    CurrentAttackTimeCounter -= CurrentAttackTimer;
                    CurrentAttackCount = AIntExtension.Repeat(CurrentAttackCount + 1, 720720);
                }
            }
            else
            {
                CurrentAttackTimeCounter += Time.deltaTime;
            }
            
            
            return UniTask.CompletedTask;
        }

        public UniTask OnStateExit(HeroAttackState state, CancellationToken token)
        {
            return UniTask.CompletedTask;
        }
        
        private bool TryTriggerAbility(Ability.Trigger trigger)
        {
            foreach (Ability ability in HeroStatData.HeroAbilities)
            {
                if (ability.AbilityTrigger != trigger) continue;
                if (!ability.CanUseAbility(this)) continue;
                ability.TryUseAbility(this);
                return true;
            }

            return false;
        }
    }
}