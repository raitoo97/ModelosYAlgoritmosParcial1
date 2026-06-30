using UnityEngine;
public class PlayerInputsManager : MonoBehaviour
{
    private PlayerInputsMap _playerMap;
    public static PlayerInputsManager instance;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }
    private void OnEnable()
    {
        _playerMap = new PlayerInputsMap();
        _playerMap.Enable();
    }
    public Vector2 GetMovement()
    {
        return _playerMap.PlayerInputs.Move.ReadValue<Vector2>();
    }
    public Vector2 GetCameraLook()
    {
        return _playerMap.Camera.Look.ReadValue<Vector2>();
    }
    public bool ShootAction()
    {
        return _playerMap.PlayerInputs.Shoot.triggered;
    }
    public bool SaveAction()
    {
        return _playerMap.LoadAndSaveInputs.Save.triggered;
    }
    public bool LoadAction()
    {
        return _playerMap.LoadAndSaveInputs.Load.triggered;
    }
    public bool PauseAction()
    {
        return _playerMap.UIInputs.Pause.triggered;
    }
    private void OnDisable()
    {
        _playerMap.Disable();
        _playerMap = null;
    }
}