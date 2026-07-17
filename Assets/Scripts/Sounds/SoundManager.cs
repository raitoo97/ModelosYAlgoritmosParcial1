using System.Collections.Generic;
using UnityEngine;
public enum SoundId
{
    Shoot, PlayerDeath, EnemyDeath, Speed, Heal,
    Music, Impact, Footsteps, EnemyShoot, Shield,
    UIClick,
    UIHover
}
//Encargado de Gestionar todo los SoundEmitter de la escena a traves de SoundService
public class SoundManager : MonoBehaviour, ISoundPlayer
{
    public static SoundManager Instance;
    [System.Serializable]
    private struct SoundEntry
    {
        public SoundId id;
        public AudioClip clip;
    }
    //lista de SoundEntry
    [SerializeField] private List<SoundEntry> _sounds;
    //prefab del sound Emmiter
    [SerializeField] private SoundEmitter _emitterPrefab;
    [SerializeField] private int _poolSize = 8;
    private SoundService _service;
    //cada soundId esta relacionado con un audioClip
    private Dictionary<SoundId, AudioClip> _clips = new Dictionary<SoundId, AudioClip>();
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        FillClipsDictionary();
        //creo el sound service
        _service = new SoundService(_emitterPrefab, transform, _poolSize);
    }
    private void FillClipsDictionary()
    {
        //completo el diccionario a cada soundID le corresponde un Audio Clip
        //EJ ID = Shoot
        //clip = disparoFuerte.wav
        //_clips[Shoot] -> devuelve disparoFuerte.wav
        foreach (var sound in _sounds)
        {
            _clips[sound.id] = sound.clip;
        }
    }
    //Metodo que reproducir el clip asociado al ID
    public void Play(SoundId id, float volume = 1f, bool loop = false)
    {
        if (!_clips.TryGetValue(id, out AudioClip clip) || clip == null) return;
        _service.Play(clip, volume, loop);
    }
    //Metodo que corta y resetea el clip asociado al ID
    public void Stop(SoundId id)
    {
        if (!_clips.TryGetValue(id, out AudioClip clip)) return;
        _service.Stop(clip);
    }
    //Metodo que cambia el volumen del clip asociado al ID
    public void SetVolume(SoundId id, float volume)
    {
        if (!_clips.TryGetValue(id, out AudioClip clip)) return;
        _service.SetVolume(clip, volume);
    }
}
