using UnityEngine;
//PlayerPref
//PlayerPrefs.GetInt(key, 0) -> tomo el valor de la key si a la key nunca le asinge valor por defecto devuelve 0
//PlayerPrefs.SetInt(key, 1)-> le asigno un valor a la key
public class GameManager : MonoBehaviour
{
    public Player player;
    public static GameManager instance;
    public Transform projectilesParent;
    //Una const pertence a la clase no a la instancia y yo desde el Menu quiero acceder a esta variable
    //por eso la hago Const
    //como el Menu no tiene game manager por eso necesito que sea constante esta varibale
    //ademas no quiero q cambie nunca la key.
    public const string HighScoreKey = "HighScore";
    private int _score;
    public int Score => _score;
    private void Awake()
    {
        if(instance == null)
            instance = this;
        else
            Destroy(gameObject);
        //Se active cuando muere un enemigo en EnemySpawnerManager
        EventManager.SubscribeToEvent(EventType.EnemyKilled, OnEnemyKilled);
        Cursor.visible = false;
    }
    private void Start()
    {
        _score = 0;
        //El unico que se suscbir al evento ScoreChanged es la UI
        EventManager.TriggerEvent(EventType.ScoreChanged, _score);
    }
    private void OnEnemyKilled(params object[] parameters)
    {
        AddScore((int)parameters[0]);
    }
    public void AddScore(int amount)
    {
        _score += amount;
        // Actualizo el record historico si supere el maximo.
        if (_score > PlayerPrefs.GetInt(HighScoreKey, 0))
            PlayerPrefs.SetInt(HighScoreKey, _score);
        EventManager.TriggerEvent(EventType.ScoreChanged, _score);
    }
    private void OnDestroy()
    {
        EventManager.UnsubscribeToEvent(EventType.EnemyKilled, OnEnemyKilled);
    }
}
