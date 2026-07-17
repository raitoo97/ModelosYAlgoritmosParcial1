using UnityEngine;
//yo no quiero ni que player conozca a PlayerSoundView ni que PlayerSoundView conozca a player
//pero necesito que PlayerSoundView se suscriba al model del player para reaccionar a sus eventos
//este script los relaciona sin que se conozcan, hace de intermediario.
[RequireComponent(typeof(Player))]
public class PlayerSoundBinder : MonoBehaviour
{
    private Player _player;
    private PlayerSoundView _soundView;
    private void Start()
    {
        _player = GetComponent<Player>();
        _soundView = new PlayerSoundView(SoundManager.Instance);
        _player.SubscribeObserver(_soundView);
    }
    private void OnDestroy()
    {
        if (_soundView != null) _player.UnsubscribeObserver(_soundView);
    }
}
