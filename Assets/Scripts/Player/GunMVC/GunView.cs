using UnityEngine;
public class GunView
{
    private ParticleSystem _muzzleFlash;
    private ParticleSystem _impactEffect;
    private LineRenderer _laser;
    private Transform _laserDot;
    private ParticleSystem[] _impactEffects;
    private int _nextImpactEffect;
    public GunView(ParticleSystem muzzleFlash, ParticleSystem impactEffectTemplate, LineRenderer laser, Transform laserDot, Transform effectsParent, int impactEffectCount)
    {
        _muzzleFlash = muzzleFlash;
        _laser = laser;
        _laserDot = laserDot;
        if (_laser != null) _laser.enabled = false;
        if (_laserDot != null) _laserDot.gameObject.SetActive(false);
        if (impactEffectTemplate != null && impactEffectCount > 0)
        {
            _impactEffects = new ParticleSystem[impactEffectCount];
            for (int i = 0; i < impactEffectCount; i++)
            {
                _impactEffects[i] = Object.Instantiate(impactEffectTemplate, effectsParent);
                _impactEffects[i].Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            }
        }
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
    public void PlayImpactEffect(Vector3 point, Vector3 normal)
    {
        if (_impactEffects == null) return;
        ParticleSystem effect = _impactEffects[_nextImpactEffect];
        // Avanzo el indice circular para que el proximo impacto use otro clon.
        _nextImpactEffect = (_nextImpactEffect + 1) % _impactEffects.Length;
        effect.transform.SetPositionAndRotation(point, Quaternion.LookRotation(normal));
        effect.Play();
    }
}
