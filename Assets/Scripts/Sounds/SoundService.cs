using System.Collections.Generic;
using UnityEngine;
//Script que me genera los SoundEmitter
//para eso creo su pool de SoundEmitter y su Factoy de SoundEmitterFactory
public class SoundService
{
    private Pool<SoundEmitter> _pool;
    private SoundEmitterFactory _factory;
    private List<SoundEmitter> _emitters = new List<SoundEmitter>();
    public SoundService(SoundEmitter prefab, Transform parent, int size)
    {
        //creo el factoy y el pool en el constructor
        _factory = new SoundEmitterFactory(prefab, parent);
        _pool = new Pool<SoundEmitter>(CreateEmitter, TurnOn, TurnOff, size);
    }
    // cuando se crea un emiter creo un objeto , le seteo la accion de return to pool y lo adjunto a la lista de emmiters
    private SoundEmitter CreateEmitter()
    {
        SoundEmitter emitter = _factory.CreateObject();
        emitter.SetReturnToPoolCallBack(ReturnToPool);
        _emitters.Add(emitter);
        return emitter;
    }
    //agarro un emmiter del pool y reproduzco su audio
    public void Play(AudioClip clip, float volume, bool loop)
    {
        SoundEmitter emitter = _pool.GetObject();
        emitter.Play(clip, volume, loop);
    }
    //para detener un emiter (corta y resetea) primero paso un clip
    // y los emmiters internamente se fijan
    // estoy activado, estoy reproduciendo el clip que me pasaste por parametro y ademas estoy reproduciendo ese audio
    //entonces me devuelvo al pool
    public void Stop(AudioClip clip)
    {
        foreach (var emitter in _emitters)
        {
            if (emitter.IsPlayingClip(clip)) ReturnToPool(emitter);
        }
    }
    //para setear el volumen de un emiter primero paso un clip
    // y los emmiters internamente se fijan
    // estoy activado, estoy reproduciendo el clip que me pasaste por parametro y ademas estoy reproduciendo ese audio
    // entonces cambio mi volumen al que me pasas por parametro
    public void SetVolume(AudioClip clip, float volume)
    {
        foreach (var emitter in _emitters)
        {
            if (emitter.IsPlayingClip(clip)) emitter.SetVolume(volume);
        }
    }
    private void TurnOn(SoundEmitter emitter)
    {
        emitter.gameObject.SetActive(true);
    }
    //Se ejecuta cuando el emitter vuelve al Pool:
    //detiene el clip (corta y resetea) y desactiva el objeto
    private void TurnOff(SoundEmitter emitter)
    {
        emitter.Stop();
        emitter.gameObject.SetActive(false);
    }
    private void ReturnToPool(SoundEmitter emitter)
    {
        _pool.ReturnObject(emitter);
    }
}