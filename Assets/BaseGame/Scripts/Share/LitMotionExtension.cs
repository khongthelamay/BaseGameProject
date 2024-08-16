using LitMotion;

public static class MyLitMotionExtension 
{
    public static void TryCancel(this MotionHandle handle)
    {
        if (handle.IsActive())
        {
            handle.Cancel();
        }
    }
}