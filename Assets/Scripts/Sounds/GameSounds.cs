using UnityEngine;
// Sonidos de eventos GLOBALES del juego (EventManager),
// separados del audio del player (que va por PlayerSoundView con los PlayerEvent).
// Para sumar un sonido global nuevo: suscribir el evento aca y nada mas
public class GameSounds : MonoBehaviour
{
    private void Start()
    {
        EventManager.SubscribeToEvent(EventType.EnemyKilled, OnEnemyKilled);
    }
    private void OnDestroy()
    {
        EventManager.UnsubscribeToEvent(EventType.EnemyKilled, OnEnemyKilled);
    }
    private void OnEnemyKilled(params object[] parameters)
    {
        SoundManager.Instance.Play(SoundId.EnemyDeath);
    }
}
