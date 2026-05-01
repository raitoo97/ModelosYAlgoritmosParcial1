using UnityEngine;
public class PlayerView
{
    private Animator _animator;
    public PlayerView(Player user)
    {
        _animator = user.GetComponent<Animator>();
    }
    public void MoveAnimation(bool isRunning)
    {
        _animator.SetBool("IsRunning", isRunning);
    }
}
