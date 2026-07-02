using System;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
public class CameraController : MonoBehaviour , IObserver<PlayerEvent>
{
    [SerializeField]private Transform followTarget;
    [SerializeField]private float _sensitivity;
    [SerializeField]private bool _invertY = false;
    [SerializeField]private CinemachineCamera _mainCamera;
    [SerializeField]private CinemachineCamera _aimCamera;
    [SerializeField] private Vector3 _mainCameraOffset = new Vector3(0.8f, -0.4f, -2f);
    [SerializeField] private Vector3 _aimCameraOffset = new Vector3(0.8f, -0.5f, -1.2f);
    private Dictionary<PlayerEvent, Action> _actions = new Dictionary<PlayerEvent, Action>();
    private Transform _player;
    private float xRotation;
    private float yRotation;
    private void Start()
    {
        _player = GameManager.instance.player.transform;
        GameManager.instance.player.SubscribeObserver(this);
        _aimCamera.Priority = 0;
        _mainCamera.Priority = 1;
        FillDictionary();
    }
    public void Notify(PlayerEvent Actions)
    {
        if (_actions.ContainsKey(Actions))
        {
            _actions[Actions].Invoke();
        }
    }
    private void FillDictionary()
    {
        _actions.Add(PlayerEvent.Aim, ChangeCameraAim);
        _actions.Add(PlayerEvent.StopAim, ChangeCameraNormal);
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
        Vector2 look = PlayerInputsManager.instance.GetCameraLook() * _sensitivity;
        xRotation += (_invertY ? -1 : 1) * look.y;
        xRotation = Mathf.Clamp(xRotation, -30f, 70f);
        yRotation += look.x;
        followTarget.rotation = Quaternion.Euler(xRotation, yRotation, 0);
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
    }
}
