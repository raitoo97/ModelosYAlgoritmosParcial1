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

    public void UpdateLaser(bool isVisible, Vector3 start, Vector3 end, bool hasHit, Vector3 hitNormal)
    {
        if (_laser == null) return;
        _laser.enabled = isVisible;
        if (_laserDot != null)
            _laserDot.gameObject.SetActive(isVisible && hasHit);
        if (!isVisible) return;
        _laser.SetPosition(0, start);
        _laser.SetPosition(1, end);
        if (hasHit && _laserDot != null)
            _laserDot.position = end + hitNormal * 0.02f;
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
