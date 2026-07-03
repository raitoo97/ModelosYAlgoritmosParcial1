using UnityEngine;
public class LanguageButton : MonoBehaviour
{
    [SerializeField] private SystemLanguage _language;
    public void SelectLanguage()
    {
        LocalizationManager.Instance.ChangeLanguage(_language);
    }
}
