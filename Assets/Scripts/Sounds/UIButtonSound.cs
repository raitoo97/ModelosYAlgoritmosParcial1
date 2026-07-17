using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
// Sonido de UI reutilizable: se agrega al GameObject de cualquier boton
// y suena al click y al hover. Funciona en cualquier escena que tenga
// su SoundManager (menu y gameplay usan el mismo componente).
// Usa las interfaces de eventos de UI: el EventSystem de la escena
public class UIButtonSound : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler
{
    [SerializeField] private SoundId _clickSound = SoundId.UIClick;
    [SerializeField] private SoundId _hoverSound = SoundId.UIHover;
    [Range(0f, 1f)][SerializeField] private float _clickVolume = 0.6f;
    [Range(0f, 1f)][SerializeField] private float _hoverVolume = 0.3f;
    //clase madre de todos los controles interactivos de la UI de Unity
    private Button _button;
    private void Awake()
    {
        _button = GetComponent<Button>();
    }
    private bool IsInteractable => _button.interactable;
    public void OnPointerClick(PointerEventData eventData)
    {
        if (!IsInteractable) return;
        SoundManager.Instance.Play(_clickSound, _clickVolume);
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!IsInteractable) return;
        SoundManager.Instance.Play(_hoverSound, _hoverVolume);
    }
}
