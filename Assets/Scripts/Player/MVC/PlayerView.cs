using System;
using System.Collections.Generic;
using UnityEngine;
public class PlayerView : IObserver<PlayerEvent>
{
    private Animator _animator;
    private Dictionary<PlayerEvent, Action> _actions;
    private float _currentAimWeight;
    private int _aimLayerIndex;
    public PlayerView(Player user)
    {
        _animator = user.GetComponent<Animator>();
        _actions = new Dictionary<PlayerEvent, Action>();
        _aimLayerIndex = _animator.GetLayerIndex("Aim");
        FillDictionary();
    }
    public void MoveAnimation(bool isRunning)
    {
        _animator.SetBool("IsRunning", isRunning);
    }
    public void OnPlayerDeath()
    {
        _animator.SetTrigger("OnDeath");
    }
    public void UpdateAimIK(bool isAiming, Vector3 aimPoint)
    {
        // Determina el peso objetivo del IK:
        // 1 = apuntando, 0 = sin apuntar.
        float targetWeight = isAiming ? 1f : 0f;
        // Interpola suavemente el peso para evitar cambios bruscos
        // al entrar o salir del modo apuntado.
        _currentAimWeight = Mathf.Lerp(_currentAimWeight, targetWeight, Time.deltaTime * 8f);
        // Si existe una capa de animacion para apuntar, ajusta su peso
        // para mezclar progresivamente la pose de apuntado.
        if (_aimLayerIndex >= 0)
            _animator.SetLayerWeight(_aimLayerIndex, _currentAimWeight);
        // Define el punto del mundo hacia el que debe mirar el personaje.
        _animator.SetLookAtPosition(aimPoint);
        // Configura el LookAt IK indicando el peso total y cuanto participan
        // el torso, la cabeza, los ojos y la restriccion del giro.
        _animator.SetLookAtWeight(
            weight: _currentAimWeight,
            bodyWeight: 0.6f,
            headWeight: 0.4f,
            eyesWeight: 0f,
            clampWeight: 0.5f
        );
    }
    private void FillDictionary()
    {
        _actions.Add(PlayerEvent.Move, () => MoveAnimation(true));
        _actions.Add(PlayerEvent.Idle, () => MoveAnimation(false));
        _actions.Add(PlayerEvent.Death, OnPlayerDeath);
    }
    public void Notify(PlayerEvent Actions)
    {
        if (_actions.ContainsKey(Actions))
        {
            _actions[Actions].Invoke();
        }
    }
    public Animator GetAnimator => _animator;
}