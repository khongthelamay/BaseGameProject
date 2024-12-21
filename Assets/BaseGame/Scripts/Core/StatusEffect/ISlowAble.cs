using R3;

namespace Core.GameStatusEffect
{
    public interface ISlowAble
    {
        public SerializableReactiveProperty<float> SlowAmount { get; set; }
    }
}