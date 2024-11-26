using TW.Reactive.CustomComponent;
using UnityEngine;

namespace Core
{
    public partial class Tiger : Hero
    {
        [field: SerializeField] public ReactiveValue<int> FuryPoint {get; set;} = new(0);
        [field: SerializeField] public int FuryRate {get; private set;}
        
        [field: SerializeField] public VisualEffect SavageRoarEffect {get; set;}
        protected override void InitAbility()
        {
            base.InitAbility();
            FuryPoint.Value = 0;
        }
        public void ChangeFuryPoint(int point)
        {
            FuryPoint.Value += point;
        }
        public void ChangeFuryRate(int rate)
        {
            FuryRate += rate;
        }
        
    }
}