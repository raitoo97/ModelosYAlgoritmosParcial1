using UnityEngine;
public enum PlayerEvent
{
    Move,
    Death
}
[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour
{
    private PlayerModel _model;
    private IController _controler;
    private PlayerView _view;
    private void Awake()
    {
        _model = new PlayerModel(this);
        _controler = new PlayerController(_model);
        _view = new PlayerView(this);
        _model.onMovement += _view.MoveAnimation;
    }
    private void Update()
    {
        _controler.UpdateInputs();
    }

    private void FixedUpdate()
    {
        _controler.FixedUpdateInputs();
    }
}
