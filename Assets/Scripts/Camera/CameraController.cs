using UnityEngine;
public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform followTarget;
    private Transform _player;
    float xRotation;
    float yRotation;
    private void Start()
    {
        _player = GameManager.instance.player.transform;
    }
    private void Update()
    {
        if (_player == null || followTarget == null) return;
        followTarget.position = _player.position + Vector3.up * 2;
        xRotation += PlayerInputsManager.instance.GetCameraLook().y;
        xRotation = Mathf.Clamp(xRotation, -30f, 70f);
        yRotation += PlayerInputsManager.instance.GetCameraLook().x;
        followTarget.rotation = Quaternion.Euler(xRotation, yRotation, 0);
    }
}
