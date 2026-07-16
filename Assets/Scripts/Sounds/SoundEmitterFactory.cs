using UnityEngine;
//Factory de SoundEmmiters 
public class SoundEmitterFactory : Factory<SoundEmitter>
{
    public SoundEmitterFactory(SoundEmitter prefab, Transform parent)
    {
        _prefab = prefab;
        _parent = parent;
    }
}
