using System;
using UnityEngine;
//Prefab de objeto encargado de reproducir Sonido
[RequireComponent(typeof(AudioSource))]
public class SoundEmitter : MonoBehaviour
{
    //Tiene su audioSource y su evento para volver al pool
    private AudioSource _source;
    private Action<SoundEmitter> _returnToPool;
    private void Awake()
    {
        _source = GetComponent<AudioSource>();
        _source.playOnAwake = false;
    }
    //le seteo el evento
    public void SetReturnToPoolCallBack(Action<SoundEmitter> returnToPool)
    {
        _returnToPool = returnToPool;
    }
    //metodo para reproducir audio recibo por parametro el clip con su volumen y si es loopeable
    //dentro del metodo configuro el source
    public void Play(AudioClip clip, float volume, bool loop)
    {
        _source.clip = clip;
        _source.volume = volume;
        _source.loop = loop;
        _source.Play();
    }
    //metodo para detener el clip (lo corta y resetea al principio)
    public void Stop()
    {
        _source.Stop();
    }
    //metodo para cambiar el volumen al clip
    public void SetVolume(float volume)
    {
        _source.volume = volume;
    }
    //metodo que devuelve un bool
    // true -> si el objeto esta activado && si el clip que le paso por parametro es el mismo que tenia antes este SoundEmitter  && si el SoundEmitter esta reproduciendo
    //false -> si alguna de las condiciones anteriores no se cumple
    public bool IsPlayingClip(AudioClip clip)
    {
        return gameObject.activeSelf && _source.clip == clip && _source.isPlaying;
    }
    private void Update()
    {
        // cuando el clip termina, isPlaying pasa a false y me devuelvo al pool.
        if (!_source.isPlaying)
        {
            _returnToPool?.Invoke(this);
        }
    }
}
