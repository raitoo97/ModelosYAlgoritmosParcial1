using UnityEngine;
public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform followTarget;
    [SerializeField] private float _sensitivity;
    [SerializeField] private bool _invertY = false;
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
        Vector2 look = PlayerInputsManager.instance.GetCameraLook() * _sensitivity;
        xRotation += (_invertY ? -1 : 1) * look.y;
        xRotation = Mathf.Clamp(xRotation, -30f, 70f);
        yRotation += look.x;
        followTarget.rotation = Quaternion.Euler(xRotation, yRotation, 0);
    }
}
