using UnityEngine;
public class SniperEnemy : Enemy
{
    [SerializeField] private LayerMask _losMask;
    private SniperView _sniperView;
    protected override FlyWeight Stats => FlyWeightPointer.Sniper;
    protected override EnemyView CreateView()
    {
        _sniperView = new SniperView(this);
        return _sniperView;
    }
    protected override IState CreateAttackState(Transform playerTransform)
    {
        return new SniperAttackState(transform, _agent, this, _fsm, playerTransform, Stats, _gunSight, _losMask);
    }
    protected override IState CreateChaseState(Transform playerTransform)
    {
        return new SniperChaseState(transform, _agent, this, _fsm, playerTransform, Stats);
    }
    public override void Shoot()
    {
        ShotResult result = Model.Shoot(_gunSight.position);
        _sniperView.PlayShootEffects(result);
    }
    public void ShowLaser(bool visible) 
    {
        _sniperView.ShowLaser(visible);
    } 
    public void UpdateLaser(Vector3 origin, Vector3 end, float progress) 
    {
        _sniperView.UpdateLaser(origin, end, progress);
    } 
}
