using TMPro;
using UnityEngine;
public class ButtonTranslate : MonoBehaviour
{
    [SerializeField] private string id;
    [SerializeField] private TextMeshProUGUI _myText;
    private void OnEnable()
    {
        if (id != gameObject.name) id = gameObject.name;
        LocalizationManager.Instance.onUpdate += UpdateText;
        UpdateText();
    }
    private void OnDisable()
    {
        LocalizationManager.Instance.onUpdate -= UpdateText;
    }
    private void OnDestroy()
    {
        LocalizationManager.Instance.onUpdate -= UpdateText;
    }
    private void UpdateText()
    {
        _myText.text = LocalizationManager.Instance.GetTranslation(id);
    }
}
