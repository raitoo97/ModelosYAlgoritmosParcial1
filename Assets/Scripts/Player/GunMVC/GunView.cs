using UnityEngine;
public class GunView
{
    private ParticleSystem _muzzleFlash;
    private LineRenderer _laser;
    private Transform _laserDot;
    // Bordes de la V de la escopeta.
    private LineRenderer _coneLeftLine;
    private LineRenderer _coneRightLine;
    private ImpactEffectService _impactEffectService;
    public GunView(ParticleSystem muzzleFlash, LineRenderer laser, Transform laserDot, LineRenderer coneLeftLine, LineRenderer coneRightLine, ImpactEffectService impactEffectService)
    {
        _muzzleFlash = muzzleFlash;
        _laser = laser;
        _laserDot = laserDot;
        _coneLeftLine = coneLeftLine;
        _coneRightLine = coneRightLine;
        _impactEffectService = impactEffectService;
        if (_laser != null) _laser.enabled = false;
        if (_coneLeftLine != null) _coneLeftLine.enabled = false;
        if (_coneRightLine != null) _coneRightLine.enabled = false;
        if (_laserDot != null) _laserDot.gameObject.SetActive(false);
    }
    public void UpdateAimIndicator(AimIndicatorState state)
    {
        // El dot solo acompania al laser central; los bordes de la V van sin dot.
        UpdateLine(_laser, state.laser, _laserDot);
        UpdateLine(_coneLeftLine, state.coneLeft, null);
        UpdateLine(_coneRightLine, state.coneRight, null);
    }
    private void UpdateLine(LineRenderer line, LaserState state, Transform dot)
    {
        if (line == null) return;
        line.enabled = state.isVisible;
        if (dot != null)
            dot.gameObject.SetActive(state.isVisible && state.hasHit);
        if (!state.isVisible) return;
        line.SetPosition(0, state.start);
        line.SetPosition(1, state.end);
        if (state.hasHit && dot != null)
            //Lo multiplico por 0.02f para que el punto de impacto no se superponga con la superficie del objeto impactado salga un poco en direccion a su normal
            dot.position = state.end + state.hitNormal * 0.02f;
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
