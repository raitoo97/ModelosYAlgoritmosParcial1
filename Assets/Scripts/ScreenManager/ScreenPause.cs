public class ScreenPause : ScreenBase
{
    public void BTN_Screen(string screenToPush)
    {
        ScreenManager.Instance.Push(screenToPush);
    }
}
