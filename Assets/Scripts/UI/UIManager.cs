using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class UIManager : MonoBehaviour
{
    [SerializeField]private Image _lifebar;
    [SerializeField]private TextMeshProUGUI _gameOver;
    [SerializeField]private TextMeshProUGUI _score;
    [SerializeField]private TextMeshProUGUI _saves;
    [SerializeField]private TextMeshProUGUI _loads;
    [SerializeField]private Image _weaponIcon;
    [SerializeField]private Image _powerUpIcon;
    [SerializeField]private TextMeshProUGUI _powerUpCharges;
    [SerializeField]private string _pauseScreen;
    private IController _controller;
    private void Awake()
    {
        _gameOver.gameObject.SetActive(false);
        _saves.color = Color.green;
        _loads.color = Color.red;
        _powerUpIcon.enabled = false;
        _powerUpCharges.text = "";
        _controller = new UIController(_pauseScreen);
    }
    private void OnEnable()
    {
        EventManager.SubscribeToEvent(EventType.PlayerDamage, OnPlayerDamage);
        EventManager.SubscribeToEvent(EventType.PlayerDeath, OnPlayerDeath);
        EventManager.SubscribeToEvent(EventType.UpdateSaves, OnPlayerSave);
        EventManager.SubscribeToEvent(EventType.UpdateLoads, OnPlayerLoad);
        EventManager.SubscribeToEvent(EventType.WeaponChanged, OnWeaponChanged);
        EventManager.SubscribeToEvent(EventType.PowerUpChanged, OnPowerUpChanged);
        EventManager.SubscribeToEvent(EventType.ScoreChanged, OnScoreChanged);
    }
    private void Update()
    {
        _controller.UpdateInputs();
    }
    public void UpdateLifeBar(float amount)
    {
        _lifebar.fillAmount = amount;
    }
    private void OnPlayerDamage(params object[] parameters)
    {
        float life = (float)parameters[0];
        UpdateLifeBar(life);
    }
    private void OnPlayerSave(params object[] parameters)
    {
        int savesUpdate = (int)parameters[0];
        _saves.text = savesUpdate.ToString();
        _saves.color = savesUpdate > 0 ? Color.green : Color.red;
    }
    private void OnPlayerLoad(params object[] parameters)
    {
        int loadsUpdate = (int)parameters[0];
        _loads.text = loadsUpdate.ToString();
        _loads.color = loadsUpdate > 0 ? Color.green : Color.red;
    }
    private void OnWeaponChanged(params object[] parameters)
    {
        Sprite icon = (Sprite)parameters[0];
        if (icon == null) return;
        _weaponIcon.sprite = icon;
    }
    private void OnPowerUpChanged(params object[] parameters)
    {
        Sprite icon = (Sprite)parameters[0];
        int charges = (int)parameters[1];
        bool hasPowerUp = charges > 0 && icon != null;
        _powerUpIcon.enabled = hasPowerUp;
        _powerUpCharges.text = charges > 0 ? charges.ToString() : "";
        if (hasPowerUp)
            _powerUpIcon.sprite = icon;
    }
    private void OnScoreChanged(params object[] parameters)
    {
        int score = (int)parameters[0];
        _score.text = score.ToString();
    }
    private void OnPlayerDeath(params object[] parameters)
    {
        _gameOver.gameObject.SetActive(true);
    }
    private void OnDisable()
    {
        EventManager.UnsubscribeToEvent(EventType.PlayerDamage, OnPlayerDamage);
        EventManager.UnsubscribeToEvent(EventType.PlayerDeath, OnPlayerDeath);
        EventManager.UnsubscribeToEvent(EventType.ScoreChanged, OnScoreChanged);
        EventManager.UnsubscribeToEvent(EventType.UpdateSaves, OnPlayerSave);
        EventManager.UnsubscribeToEvent(EventType.UpdateLoads, OnPlayerLoad);
        EventManager.UnsubscribeToEvent(EventType.WeaponChanged, OnWeaponChanged);
        EventManager.UnsubscribeToEvent(EventType.PowerUpChanged, OnPowerUpChanged);
    }
}
