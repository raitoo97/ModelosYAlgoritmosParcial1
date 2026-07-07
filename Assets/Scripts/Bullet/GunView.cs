using UnityEngine;
public class GunView
{
    private ParticleSystem _muzzleFlash;
    private ParticleSystem _impactEffect;
    public GunView(ParticleSystem muzzleFlash, ParticleSystem impactEffect)
    {
        _muzzleFlash = muzzleFlash;
        _impactEffect = impactEffect;
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
