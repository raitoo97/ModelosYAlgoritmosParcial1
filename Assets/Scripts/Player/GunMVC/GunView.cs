using UnityEngine;
public class GunView
{
    private ParticleSystem _muzzleFlash;
    private LineRenderer _laser;
    private Transform _laserDot;
    private ImpactEffectService _impactEffectService;
    public GunView(ParticleSystem muzzleFlash, LineRenderer laser, Transform laserDot, ImpactEffectService impactEffectService)
    {
        _muzzleFlash = muzzleFlash;
        _laser = laser;
        _laserDot = laserDot;
        _impactEffectService = impactEffectService;
        if (_laser != null) _laser.enabled = false;
        if (_laserDot != null) _laserDot.gameObject.SetActive(false);
    }
    public void UpdateLaser(LaserState laser)
    {
        if (_laser == null) return;
        _laser.enabled = laser.isVisible;
        if (_laserDot != null)
            _laserDot.gameObject.SetActive(laser.isVisible && laser.hasHit);
        if (!laser.isVisible) return;
        _laser.SetPosition(0, laser.start);
        _laser.SetPosition(1, laser.end);
        if (laser.hasHit && _laserDot != null)
            //Lo multiplico por 0.02f para que el punto de impacto no se superponga con la superficie del objeto impactado salga un poco en direccion a su normal
            _laserDot.position = laser.end + laser.hitNormal * 0.02f;
    }
    public void PlayShootEffects(ShotResult result)
    {
        if (_muzzleFlash != null)
            _muzzleFlash.Play();
        if (result.didHit)
            PlayImpactEffect(result.hitPoint, result.hitNormal);
    }
    public void PlayImpactEffect(Vector3 point, Vector3 normal)
    {
        if (_impactEffectService == null) return;
        _impactEffectService.PlayAt(point, normal);
    }
}
