using UnityEngine;
public class GunView
{
    private ParticleSystem _muzzleFlash;
    private ParticleSystem _impactEffect;
    private LineRenderer _laser;
    private Transform _laserDot;
    public GunView(ParticleSystem muzzleFlash, ParticleSystem impactEffect, LineRenderer laser, Transform laserDot)
    {
        _muzzleFlash = muzzleFlash;
        _impactEffect = impactEffect;
        _laser = laser;
        _laserDot = laserDot;
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
        if (result.didHit && _impactEffect != null)
        {
            _impactEffect.transform.SetPositionAndRotation(result.hitPoint , Quaternion.LookRotation(result.hitNormal));
            _impactEffect.Play();
        }
    }
}
