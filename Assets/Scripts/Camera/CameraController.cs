using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private Transform followTarget;
    float xRotation;
    float yRotation;
    private void FixedUpdate()
    {
        followTarget.position = player.position + Vector3.up * 2;
        xRotation += PlayerInputsManager.instance.GetCameraLook().y;
        xRotation = Mathf.Clamp(xRotation, -30f,70f);
        yRotation += PlayerInputsManager.instance.GetCameraLook().x;
        Quaternion rot = Quaternion.Euler(xRotation, yRotation, 0);
        followTarget.rotation = rot;
    }

}
