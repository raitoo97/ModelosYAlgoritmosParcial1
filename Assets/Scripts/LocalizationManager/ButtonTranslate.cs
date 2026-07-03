using TMPro;
using UnityEngine;
public class ButtonTranslate : MonoBehaviour
{
    [SerializeField] private string id;
    [SerializeField] private TextMeshProUGUI _myText;
    private void Start()
    {
        if (LocalizationManager.Instance == null) return;
        if (id != gameObject.name) id = gameObject.name;
        LocalizationManager.Instance.onUpdate += UpdateText;
        if (LocalizationManager.Instance.IsReady)
            UpdateText();
    }
    private void OnEnable()
    {
        if (LocalizationManager.Instance == null) return;
        if (LocalizationManager.Instance.IsReady)
            UpdateText();
    }
    private void OnDisable()
    {
        if (LocalizationManager.Instance != null)
            LocalizationManager.Instance.onUpdate -= UpdateText;
    }
    private void OnDestroy()
    {
        if (LocalizationManager.Instance != null)
            LocalizationManager.Instance.onUpdate -= UpdateText;
    }
    private void UpdateText()
    {
        _myText.text = LocalizationManager.Instance.GetTranslation(id);
    }
}
