using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class UIManager : MonoBehaviour
{
    [SerializeField]private Image _lifebar;
    [SerializeField]private TextMeshProUGUI _gameOver;
    [SerializeField]private TextMeshProUGUI _score;
    [SerializeField]private TextMeshProUGUI _saves;
    [SerializeField]private string _pauseScreen;
    private IController _controller;
    private int _currentScore;
    private void Awake()
    {
        _gameOver.gameObject.SetActive(false);
        _currentScore = 0;
        _saves.color = Color.green;
        _controller = new UIController(_pauseScreen);
    }
    private void OnEnable()
    {
        EventManager.SubscribeToEvent(EventType.PlayerDamage, OnPlayerDamage);
        EventManager.SubscribeToEvent(EventType.PlayerDeath, OnPlayerDeath);
        EventManager.SubscribeToEvent(EventType.EnemyKilled, OnEnemyKilled);
        EventManager.SubscribeToEvent(EventType.UpdateSaves, OnPlayerSave);
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
        if ((int)parameters[0] > 0) return;
        _saves.color = Color.red;
    }
    private void OnEnemyKilled(params object[] parameters)
    {
        int scoreToAdd = (int)parameters[0];
        _currentScore += scoreToAdd;
        _score.text = _currentScore.ToString();
    }
    private void OnPlayerDeath(params object[] parameters)
    {
        _gameOver.gameObject.SetActive(true);
    }
    private void OnDisable()
    {
        EventManager.UnsubscribeToEvent(EventType.PlayerDamage, OnPlayerDamage);
        EventManager.UnsubscribeToEvent(EventType.PlayerDeath, OnPlayerDeath);
        EventManager.UnsubscribeToEvent(EventType.EnemyKilled, OnEnemyKilled);
        EventManager.UnsubscribeToEvent(EventType.UpdateSaves, OnPlayerSave);
    }
}
