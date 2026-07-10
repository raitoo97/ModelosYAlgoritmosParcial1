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
    //Creo un objeto donde voy a guardar los valores que quiero sobrescribir del material
    private MaterialPropertyBlock _propertyBlock = new MaterialPropertyBlock();
    // IDs de las propiedades del shader del LaserMat.
    private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");
    private static readonly int EmissionColorId = Shader.PropertyToID("_EmissionColor");
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
    // Pinta las lineas del indicador con el color del arma equipada.
    // Lo llama Gun.ApplyShootType: UNA vez por cambio de arma, no por frame.
    // La vista no sabe de que arma viene el color, solo lo aplica.
    public void SetIndicatorColor(Color color)
    {
        TintLine(_laser, color);
        TintLine(_coneLeftLine, color);
        TintLine(_coneRightLine, color);
    }
    private void TintLine(LineRenderer line, Color color)
    {
        if (line == null) return;
        // El look del laser NO sale del color de vertice del LineRenderer
        // (startColor/endColor): sale del BaseColor + Emission del material.
        // Por eso se pinta con MaterialPropertyBlock, que overridea esas
        // propiedades por renderer sin instanciar el material compartido.
        line.GetPropertyBlock(_propertyBlock);
        _propertyBlock.SetColor(BaseColorId, color);
        _propertyBlock.SetColor(EmissionColorId, color);
        line.SetPropertyBlock(_propertyBlock);
    }
    // La vista NO conoce el enum ni pregunta que arma hay: recibe las tres lineas
    // y dibuja cada una segun su isVisible. Si el modelo calculo solo el laser,
    // los bordes de la V vienen invisibles y UpdateLine los apaga solo.
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
