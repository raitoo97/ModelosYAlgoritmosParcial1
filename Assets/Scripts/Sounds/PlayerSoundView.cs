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
        _actions.Add(PlayerEvent.Move, () => _soundPlayer.Play(SoundId.Footsteps, 0.5f, loop: true));
        _actions.Add(PlayerEvent.Idle, () => _soundPlayer.Stop(SoundId.Footsteps));
        _actions.Add(PlayerEvent.UsePowerUp, () => _soundPlayer.Play(SoundId.PowerUp));
        _actions.Add(PlayerEvent.HealOn, () => _soundPlayer.Play(SoundId.Heal));
        _actions.Add(PlayerEvent.Death, () =>
        {
            _soundPlayer.Stop(SoundId.Footsteps);
            _soundPlayer.Play(SoundId.PlayerDeath);
        });
    }
    public void Notify(PlayerEvent action)
    {
        if (_actions.ContainsKey(action))
        {
            _actions[action].Invoke();
        }
    }
}
