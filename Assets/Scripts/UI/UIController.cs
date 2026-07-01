public class UIController : IController
{
    private string _pauseScreen;
    public UIController(string pauseScreen)
    {
        _pauseScreen = pauseScreen;
    }
    public void UpdateInputs()
    {
        if (PlayerInputsManager.instance.PauseAction())
        {
            if (ScreenManager.Instance.StackContainsType<ScreenPause>())
            {
                ScreenManager.Instance.Pop();
            }
            else
            {
                ScreenManager.Instance.Push(_pauseScreen);
            }
        }
    }
}
