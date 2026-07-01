using UnityEngine;
public class PlayerController : ICharacterController
{
    private PlayerModel _model;
    private Vector3 _direction;
    public PlayerController(PlayerModel model)
    {
        _model = model;
        _direction = Vector3.zero;
    }
    public void UpdateInputs()
    {
        if(PlayerInputsManager.instance.ShootAction())
        {
            _model.Shoot();
        }
        _model.SetAiming(PlayerInputsManager.instance.AimAction());
    }
    public void FixedUpdateInputs()
    {
        Vector2 movement = PlayerInputsManager.instance.GetMovement();
        _direction = new Vector3(movement.x, 0, movement.y);
        _model.Move(_direction);
        _model.Rotate(_direction);
    }
}
