using UnityEngine;

public class PlayerInputs : MonoBehaviour
{
    private PlayerInputsMap _playerMap;
    public static PlayerInputs instance;
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
    public bool ShootAction()
    {
        return _playerMap.PlayerInputs.Shoot.IsPressed();
    }
    private void OnDisable()
    {
        _playerMap.Disable();
        _playerMap = null;
    }
}
