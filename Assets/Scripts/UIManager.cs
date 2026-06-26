using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class UIManager : MonoBehaviour
{
    [SerializeField]private Image _lifebar;
    [SerializeField]private Text _gameOver;
    [SerializeField]private Text _score;
    [SerializeField]private TextMeshProUGUI _saves;
    [SerializeField]private Transform mainGame;
    [SerializeField]private string _pauseScreen;
    private int _currentScore;
    private void Awake()
    {
        _gameOver.gameObject.SetActive(false);
        _currentScore = 0;
        _saves.color = Color.green;
    }
    private void Start()
    {
        var mainScreen = new ScreenGameplay(mainGame);
        ScreenManager.Instance.Push(mainScreen);
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
