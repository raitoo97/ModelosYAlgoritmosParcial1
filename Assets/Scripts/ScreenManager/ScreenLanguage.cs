using UnityEngine;
public class ScreenLanguage : ScreenBase
{
    public void BTN_Spanish()
    {
        LocalizationManager.Instance.ChangeLanguage(SystemLanguage.Spanish);
    }
    public void BTN_English()
    {
        LocalizationManager.Instance.ChangeLanguage(SystemLanguage.English);
    }
}
