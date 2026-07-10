using UnityEngine;
public class ImpactEffectService
{
    private Pool<ImpactEffect> _pool;
    private ParticlesFactory _factory;
    public ImpactEffectService(ImpactEffect prefab, Transform parent, int size)
    {
        _factory = new ParticlesFactory(prefab, parent);
        _pool = new Pool<ImpactEffect>(CreateEffect, TurnOn, TurnOff, size);
    }
    private ImpactEffect CreateEffect()
    {
        ImpactEffect effect = _factory.CreateObject();
        effect.SetReturnToPoolCallBack(ReturnToPool);
        return effect;
    }
    // Saca un efecto del pool y lo reproduce en el punto de impacto.
    // El efecto vuelve solo al pool cuando termina (OnParticleSystemStopped).
    public void PlayAt(Vector3 point, Vector3 normal)
    {
        ImpactEffect effect = _pool.GetObject();
        effect.PlayAt(point, normal);
    }
    private void TurnOn(ImpactEffect effect)
    {
        effect.ResetEffect();
        effect.gameObject.SetActive(true);
    }
    private void TurnOff(ImpactEffect effect)
    {
        effect.gameObject.SetActive(false);
    }
    private void ReturnToPool(ImpactEffect effect)
    {
        _pool.ReturnObject(effect);
    }

}
