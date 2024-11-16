using TW.Reactive.CustomComponent;
using UnityEngine;

namespace Core
{
    public class Tiger : Hero
    {
        [field: SerializeField] public ReactiveValue<int> FuryPoint {get; set;} = new(0);
        [field: SerializeField] public int BonusFuryRate {get; private set;}

        [field: SerializeField] private VisualEffect SavageRoarEffect {get; set;}
        protected override void InitAbility()
        {
            FuryPoint.Value = 0;
            ActiveAbilities.Add(new SavageRoarAbility(this, 0, 15f, SavageRoarEffect));
            ActiveAbilities.Add(new TigerNormalAttack(this, 0));
            
            PassiveAbilities.Add(new FuryStrikeAbility(this, 0));
            PassiveAbilities.Add(new FuryPowerAbility(this, 0));
            PassiveAbilities.Add(new FuryEmpowermentAbility(this, 0));
            PassiveAbilities.Add(new FuryMasteryAbility(this, 0));
        }
        public void AddFuryPoint(int point)
        {
            FuryPoint.Value += point;
        }
        public void AddFuryRate(int rate)
        {
            BonusFuryRate += rate;
        }
        
    }
}