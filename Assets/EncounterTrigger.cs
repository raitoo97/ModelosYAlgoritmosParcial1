using System.Collections.Generic;
using UnityEngine;
public class EncounterTrigger : MonoBehaviour, IPauseable
{
    [SerializeField] private List<EnemySpawnerManager> _spawners = new List<EnemySpawnerManager>();
    [SerializeField] private float _disableDelay = 3f;
    private bool _triggered;
    private float _timer;
    private Renderer _renderer;
    private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");
    private void Start()
    {
        _triggered = false;
        _renderer = GetComponent<Renderer>();
        _renderer.material.SetColor(BaseColorId, Color.blue);
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
        if (!other.TryGetComponent<Player>(out _)) return;
        _triggered = true;
        _timer = 0f;
        foreach (EnemySpawnerManager spawner in _spawners)
            if (spawner != null) spawner.Activate();
        _renderer.material.SetColor(BaseColorId, Color.red);
    }
    private void Update()
    {
        if (!_triggered) return;
        _timer += Time.deltaTime;
        if (_timer >= _disableDelay)
            gameObject.SetActive(false);
    }
    public void Pause() { enabled = false; }
    public void Resume() { enabled = true; }
}
