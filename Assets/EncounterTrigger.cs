using System.Collections.Generic;
using UnityEngine;
public class EncounterTrigger : MonoBehaviour
{
    [SerializeField] private List<EnemySpawnerManager> _spawners = new List<EnemySpawnerManager>();
    private bool _triggered;
    private void Start()
    {
        _triggered = false;
    }
    //se activa cuando agregas el componente por primera vez a un GameObject.
    //O sea: apenas pones el EncounterTrigger en un objeto, te tilda solo el Is Trigger del collider.
    private void Reset() 
    {
        GetComponent<Collider>().isTrigger = true;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (_triggered) return;
        if (!other.TryGetComponent<Player>(out _)) return; // solo el player
        _triggered = true;
        foreach (EnemySpawnerManager spawner in _spawners)
            if (spawner != null) spawner.Activate();
        gameObject.SetActive(false);
    }
}
