using System;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
public class CameraController : MonoBehaviour, IObserver<PlayerEvent>, IPauseable
{
    [SerializeField] private Transform followTarget;
    [SerializeField] private float _sensitivity;
    [SerializeField] private bool _invertY = false;
    [SerializeField] private CinemachineCamera _mainCamera;
    [SerializeField] private CinemachineCamera _aimCamera;
    [SerializeField] private Vector3 _mainCameraOffset = new Vector3(0.8f, -0.4f, -2f);
    [SerializeField] private Vector3 _aimCameraOffset = new Vector3(0.8f, -0.5f, -1.2f);
    private CameraModel _model;
    private Dictionary<PlayerEvent, Action> _playerActions = new Dictionary<PlayerEvent, Action>();
    private Transform _player;
    private void Start()
    {
        _player = GameManager.instance.player.transform;
        GameManager.instance.player.SubscribeObserver(this);
        _model = new CameraModel(_sensitivity, _invertY);
        SaveManager.instance.Subscribe(_model);
        _aimCamera.Priority = 0;
        _mainCamera.Priority = 1;
        FillPlayerActionsDictionary();
    }
    public void Notify(PlayerEvent Actions)
    {
        if (_playerActions.ContainsKey(Actions))
            _playerActions[Actions].Invoke();
    }
    private void FillPlayerActionsDictionary()
    {
        _playerActions.Add(PlayerEvent.Aim, ChangeCameraAim);
        _playerActions.Add(PlayerEvent.StopAim, ChangeCameraNormal);
    }
    private void ChangeCameraAim()
    {
        _aimCamera.Priority = 1;
        _mainCamera.Priority = 0;
    }
    private void ChangeCameraNormal()
    {
        _aimCamera.Priority = 0;
        _mainCamera.Priority = 1;
    }
    private void LateUpdate()
    {
        if (_player == null || followTarget == null) return;
        UpdateFollowTarget();
        UpdateCameraPositions();
    }
    private void UpdateFollowTarget()
    {
        followTarget.position = _player.position + Vector3.up * 2;
        Vector2 look = PlayerInputsManager.instance.GetCameraLook();
        followTarget.rotation = _model.ComputeRotation(look);
    }
    private void UpdateCameraPositions()
    {
        Vector3 offset = followTarget.rotation * _mainCameraOffset;
        _mainCamera.transform.position = followTarget.position + offset;
        _mainCamera.transform.rotation = followTarget.rotation;
        Vector3 aimOffset = followTarget.rotation * _aimCameraOffset;
        _aimCamera.transform.position = followTarget.position + aimOffset;
        _aimCamera.transform.rotation = followTarget.rotation;
    }
    private void OnDestroy()
    {
        GameManager.instance.player.UnsubscribeObserver(this);
        SaveManager.instance.Unsubscribe(_model);
    }
    public void Pause()
    {
        enabled = false;
    }
    public void Resume()
    {
        enabled = true;
    }
}
