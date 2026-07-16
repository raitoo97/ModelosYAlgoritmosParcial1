//Interfaz que implementa SoundManager
//porque la interfaz? para desacoplar PlayerSoundView: por si mas adelante
//otro que no sea SoundManager implementa ISoundPlayer y se encarga del sonido
public interface ISoundPlayer
{
    void Play(SoundId id, float volume = 1f, bool loop = false);
    void Stop(SoundId id);
    void SetVolume(SoundId id, float volume);
}
