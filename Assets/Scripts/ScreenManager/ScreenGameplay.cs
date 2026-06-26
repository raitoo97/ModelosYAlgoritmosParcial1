using System.Collections.Generic;
using UnityEngine;
public class ScreenGameplay : IScreen
{
    [SerializeField]private Transform _root;
    private List<IPauseable> _pauseables;
    public ScreenGameplay(Transform root)
    {
        _root = root;
        _pauseables = new List<IPauseable>();
    }
    public void Deactivate()
    {
        _pauseables.Clear();
        foreach (MonoBehaviour behaviour in _root.GetComponentsInChildren<MonoBehaviour>())
        {
            if(behaviour.TryGetComponent<IPauseable>(out var pauseable))
            {
                if (!_pauseables.Contains(pauseable))
                {
                    _pauseables.Add(pauseable);
                    pauseable.Pause();
                }
            }
        }
    }
    public void Activate()
    {
        foreach (var pauseable in _pauseables)
        {
            pauseable?.Resume();
        }
        _pauseables.Clear();
    }
    public void Release()
    {
        _pauseables.Clear();
        GameObject.Destroy(_root.gameObject);
    }
}
