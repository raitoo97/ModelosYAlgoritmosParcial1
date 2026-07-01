public class SaveController : IController
{
    public void UpdateInputs()
    {
        if (PlayerInputsManager.instance.SaveAction() && SaveManager.instance.CanSave)
        {
            SaveManager.instance.Save();
        }

        if (PlayerInputsManager.instance.LoadAction() && SaveManager.instance.CanLoad)
        {
            SaveManager.instance.Load();
        }
    }
}
