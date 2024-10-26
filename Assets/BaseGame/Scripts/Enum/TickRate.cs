public enum TickRate
{
    Slow = 0, // 0.5x
    Pause = 1, // 0x
    Normal = 2, // 1x
    Hyper = 3, // 2x
    SuperHyper = 4, // 4x

}

public static class TickRateExtension
{
    public static float ToValue(this TickRate tickRate)
    {
        return tickRate switch
        {
            TickRate.Slow => 0.5f,
            TickRate.Pause => 0f,
            TickRate.Normal => 1f,
            TickRate.Hyper => 2f,
            TickRate.SuperHyper => 4f,
            _ => 1f
        };
    }
}