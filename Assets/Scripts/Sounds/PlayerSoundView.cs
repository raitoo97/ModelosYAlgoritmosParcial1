using System;
using System.Collections.Generic;
// View de audio del player: mismo patron que PlayerView pero dispara sonidos.
// Recibe ISoundPlayer por constructor: depende de la abstraccion, no del singleton
public class PlayerSoundView : IObserver<PlayerEvent>
{
    private Dictionary<PlayerEvent, Action> _actions = new Dictionary<PlayerEvent, Action>();
    private ISoundPlayer _soundPlayer;
    public PlayerSoundView(ISoundPlayer soundPlayer)
    {
        _soundPlayer = soundPlayer;
        FillDictionary();
    }
    private void FillDictionary()
    {
        _actions.Add(PlayerEvent.Shoot, () => _soundPlayer.Play(SoundId.Shoot));
        _actions.Add(PlayerEvent.Death, () => _soundPlayer.Play(SoundId.PlayerDeath));
        _actions.Add(PlayerEvent.UsePowerUp, () => _soundPlayer.Play(SoundId.PowerUp));
        _actions.Add(PlayerEvent.HealOn, () => _soundPlayer.Play(SoundId.Heal));
    }
    public void Notify(PlayerEvent action)
    {
        if (_actions.ContainsKey(action))
        {
            _actions[action].Invoke();
        }
    }
}
