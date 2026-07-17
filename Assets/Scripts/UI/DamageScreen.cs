using System.Collections;
using UnityEngine;
public class DamageScreen : MonoBehaviour
{
    [SerializeField] private Material _screenDamageMat;
    private Coroutine _damageCoroutine;
    private void Start()
    {
        HideDamage();
    }
    private void OnEnable()
    {
        EventManager.SubscribeToEvent(EventType.PlayerDamage, OnScreenDamage);
    }
    public void HideDamage()
    {
        _screenDamageMat.SetFloat("_VignetteRadius", 0);
    }
    public void ShowDamage()
    {
        if (_damageCoroutine != null)
            StopCoroutine(_damageCoroutine);
        _damageCoroutine = StartCoroutine(DamageCorutine());
    }
    private void OnScreenDamage(params object[] parameters)
    {
        ShowDamage();
    }
    private IEnumerator DamageCorutine()
    {
        float _currentTime = 0;
        float _duration = 0.3f;
        float _minValue = 0f;
        float _maxValue = 1f;
        while (_currentTime < _duration)
        {
            _currentTime += Time.deltaTime;
            float alpha = Mathf.Lerp(_minValue, _maxValue, _currentTime / _duration);
            _screenDamageMat.SetFloat("_VignetteRadius", alpha);
            yield return null;
        }
        _screenDamageMat.SetFloat("_VignetteRadius", _maxValue);
        _currentTime = 0;
        while (_currentTime < _duration)
        {
            _currentTime += Time.deltaTime;
            float alpha = Mathf.Lerp(_maxValue, _minValue, _currentTime / _duration);
            _screenDamageMat.SetFloat("_VignetteRadius", alpha);
            yield return null;
        }
        _screenDamageMat.SetFloat("_VignetteRadius", _minValue);
        _currentTime = 0;
        _damageCoroutine = null;
    }
    private void OnDestroy()
    {
        EventManager.UnsubscribeToEvent(EventType.PlayerDamage, OnScreenDamage);
    }
}
