using UnityEngine;
//PlayerPref
//PlayerPrefs.GetInt(key, 0) -> tomo el valor de la key si a la key nunca le asinge valor por defecto devuelve 0
//PlayerPrefs.SetInt(key, 1)-> le asigno un valor a la key
public class GameManager : MonoBehaviour
{
    public Player player;
    public static GameManager instance;
    public Transform projectilesParent;
    //score la hago constante simplente porque la Key no va a cambiar pero la realidad es que nos se llama fuera de gameManager
    private const string ScoreKey = "Score";
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
        _score = PlayerPrefs.GetInt(ScoreKey, 0);
        //Se active cuando muere un enemigo en EnemySpawnerManager
        EventManager.SubscribeToEvent(EventType.EnemyKilled, OnEnemyKilled);
    }
    private void Start()
    {
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
        PlayerPrefs.SetInt(ScoreKey, _score);
        // Actualizo el record historico si supere el maximo.
        if (_score > PlayerPrefs.GetInt(HighScoreKey, 0))
            PlayerPrefs.SetInt(HighScoreKey, _score);
        EventManager.TriggerEvent(EventType.ScoreChanged, _score);
    }
    //Metodo por si algun dia necesito reiniciar el score.
    public void ResetScore()
    {
        _score = 0;
        PlayerPrefs.SetInt(ScoreKey, _score);
        EventManager.TriggerEvent(EventType.ScoreChanged, _score);
    }
    private void OnDestroy()
    {
        EventManager.UnsubscribeToEvent(EventType.EnemyKilled, OnEnemyKilled);
    }
}
