using UnityEngine;
public class SniperView : EnemyView
{
    private LineRenderer _laser;
    private ParticleSystem _impactEffect;
    private const float LaserStartWidth = 0.35f;
    private const float LaserEndWidth = 0.05f;
    private static readonly Color LaserStartColor = new Color(0f, 1f, 0f, 0.35f);
    public SniperView(Enemy user) : base(user)
    {
        _laser = user.GetComponentInChildren<LineRenderer>(true);
        _impactEffect = user.GetComponentInChildren<ParticleSystem>(true); // pongo (true) para que busque en los hijos inactivos tambien, porque el particle system esta desactivado al inicio
    }
    public void ShowLaser(bool visible)
    {
        if (_laser == null) return;
        _laser.enabled = visible;
    }
    public void UpdateLaser(Vector3 origin, Vector3 end, float progress)
    {
        if (_laser == null) return;
        _laser.SetPosition(0, origin);
        _laser.SetPosition(1, end);
        Color color = Color.Lerp(LaserStartColor, Color.red, progress);
        _laser.startColor = color;
        _laser.endColor = color;
        float width = Mathf.Lerp(LaserStartWidth, LaserEndWidth, progress);
        _laser.startWidth = width;
        _laser.endWidth = width;
    }
    public void PlayShootEffects(ShotResult result)
    {
        if (!result.didHit || _impactEffect == null) return;
        _impactEffect.transform.SetPositionAndRotation(result.hitPoint, Quaternion.LookRotation(result.hitNormal));
        _impactEffect.Play();
    }
    protected override void OnEnemyDeath()
    {
        base.OnEnemyDeath();
        ShowLaser(false);   // que no siga el laser prendido mientras muere
    }
    protected override void ResetView()
    {
        base.ResetView();
        ShowLaser(false);
    }
}
