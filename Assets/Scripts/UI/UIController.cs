public class UIController : IController
{
    private string _pauseScreen;
    public UIController(string pauseScreen)
    {
        _pauseScreen = pauseScreen;
    }
    public void UpdateInputs()
    {
        //primera vez que apreto PauseAction me instancia screenPause , luego va para atreas con el mismo boton 
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
