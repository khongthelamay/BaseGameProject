using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
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
        private Dictionary<ActiveAbility, float> Cooldown {get; set;} = new Dictionary<ActiveAbility, float>();
#if UNITY_EDITOR
        [ShowInInspector]
        private List<float> CooldownList => Cooldown.Values.ToList();
#endif
        public UniTask OnStateEnter(HeroAttackState state, CancellationToken token)
        {
            CurrentAttackTimeCounter = CurrentAttackTimer;
            CurrentAttackCount = 1;
            return UniTask.CompletedTask;
        }   

        public UniTask OnStateExecute(HeroAttackState state, CancellationToken token)
        {
            CurrentAttackTimer = 1f / HeroStatData.BaseAttackSpeed;
            // Cooldown.TryAdd(HeroStatData.HeroAbilities[0], 0);
            TryUpdateCooldownAbility();
            if (CurrentAttackTimeCounter > CurrentAttackTimer)
            {
                CurrentAttackProbability = Random.Range(0f, 100f);
                if (TryTriggerAbility(Ability.Trigger.Passive))
                {
                    HeroAnim.PlayAttackAnimation(HeroStatData.BaseAttackSpeed);
                    CurrentAttackTimeCounter -= CurrentAttackTimer;
                    CurrentAttackCount = AIntExtension.Repeat(CurrentAttackCount + 1, 720720);
                }
                else if (TryTriggerAbility(Ability.Trigger.Active))
                {
                    Debug.Log(1);
                    HeroAnim.PlayAttackAnimation(HeroStatData.BaseAttackSpeed);
                    CurrentAttackTimeCounter -= CurrentAttackTimer;
                    CurrentAttackCount = AIntExtension.Repeat(CurrentAttackCount + 1, 720720);
                }
                else if (TryTriggerAbility(Ability.Trigger.NormalAttack))
                {
                    HeroAnim.PlayAttackAnimation(HeroStatData.BaseAttackSpeed);
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
        public void SetCooldown(ActiveAbility ability, float cooldown)
        {
            if (!Cooldown.ContainsKey(ability))
            {
                Cooldown.TryAdd(ability, 0);
            }
            Cooldown[ability] = cooldown;
        }
        public float GetCooldown(ActiveAbility ability)
        {
            if (!Cooldown.ContainsKey(ability))
            {
                Cooldown.TryAdd(ability, 0);
            }
            return Cooldown[ability];
        }
        
        private bool TryTriggerAbility(Ability.Trigger trigger)
        {
            foreach (Ability ability in HeroStatData.HeroAbilities)
            {
                if (ability.AbilityTrigger != trigger) continue;
                if (!ability.CanUseAbility(this)) continue;
                if (!ability.TryUseAbility(this)) continue;
                return true;
            }

            return false;
        }
        private bool TryUpdateCooldownAbility()
        {
            foreach (Ability ability in HeroStatData.HeroAbilities)
            {
                if (ability.AbilityTrigger != Ability.Trigger.Active) continue;
                ActiveAbility activeAbility = (ActiveAbility)ability;
                SetCooldown(activeAbility, GetCooldown(activeAbility) + Time.deltaTime);
                Debug.Log(GetCooldown(activeAbility));
                return true;
            }

            return false;
        }
    }
}