using BaseGame.Scripts.Enum;
using TW.Reactive.CustomComponent;
using UnityEngine;

namespace Core
{
    public class Tiger : Hero
    {
        [field: SerializeField] public ReactiveValue<int> FuryPoint {get; set;} = new(0);
        [field: SerializeField] public int BonusFuryStrikeRate {get; private set;}

        [field: SerializeField] private VisualEffect SavageRoarEffect {get; set;}
        protected override void InitAbility()
        {
            FuryPoint.Value = 0;
            ActiveAbilities.Add(new SavageRoarAbility(this, 0, 15f, 100, DamageType.Physical, SavageRoarEffect));
            ActiveAbilities.Add(new FuryStrikeAbility(this, 0, 8, DamageType.Physical, 5));
            
            PassiveAbilities.Add(new FuryPowerAbility(this, 0, 0.25f, 0.25f));
            PassiveAbilities.Add(new FuryEmpowermentAbility(this, 0, 0.25f, 0.25f));
            PassiveAbilities.Add(new FuryMasteryAbility(this, 0, 10));
        }
        public void AddFuryPoint(int point)
        {
            FuryPoint.Value += point;
        }
        public void ChangeBonusFuryStrikeRate(int rate)
        {
            BonusFuryStrikeRate = rate;
        }
        
    }
}