using TW.Reactive.CustomComponent;
using UnityEngine;

namespace Core
{
    public class Tiger : Hero
    {
        [field: SerializeField] public ReactiveValue<int> FuryPoint {get; set;} = new(0);
        protected override void InitAbility()
        {
            base.InitAbility();
            FuryPoint.Value = 0;
        }
        public void ChangeFuryPoint(int point)
        {
            FuryPoint.Value += point;
        }
        
    }
}