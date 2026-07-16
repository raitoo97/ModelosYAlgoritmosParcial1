using UnityEngine;
//Arranca la musica de fondo del nivel (ID Music del SoundManager)
//e implementa IPauseable para bajarle el volumen durante la pausa y restaurarlo al reanudar
public class LevelMusic : MonoBehaviour, IPauseable
{
    [SerializeField] private SoundId _music = SoundId.Music;
    [Range(0f, 1f)]
    [SerializeField] private float _volume = 0.4f;
    [Range(0f, 1f)]
    [SerializeField] private float _pausedVolume = 0.1f;
    private void Start()
    {
        SoundManager.Instance.Play(_music, _volume, loop: true);
    }
    public void Pause()
    {
        SoundManager.Instance.SetVolume(_music, _pausedVolume);
    }
    public void Resume()
    {
        SoundManager.Instance.SetVolume(_music, _volume);
    }
}
