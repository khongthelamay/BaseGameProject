using TW.Reactive.CustomComponent;

public interface ISlowAble
{
    public ReactiveValue<float> SlowAmount { get; set; }
}