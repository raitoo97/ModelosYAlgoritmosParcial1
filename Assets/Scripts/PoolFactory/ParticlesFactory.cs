using UnityEngine;
public class ParticlesFactory : Factory<ImpactEffect>
{
    public ParticlesFactory(ImpactEffect prefab, Transform parent)
    {
        _prefab = prefab;
        _parent = parent;
    }
}
