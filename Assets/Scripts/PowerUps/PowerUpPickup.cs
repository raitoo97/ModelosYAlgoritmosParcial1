using UnityEngine;
// Item de power up en el mapa. Solo sabe QUE config regala y se la ofrece
// a cualquier IPowerUpCollector que lo pise.
public class PowerUpPickup : MonoBehaviour
{
    [SerializeField] private PowerUpConfig _config;
    [SerializeField] private float _rotationSpeed = 90f;
    private void Update()
    {
        if (_rotationSpeed != 0)
            transform.Rotate(0, _rotationSpeed * Time.deltaTime, 0);
    }
    private void OnTriggerEnter(Collider other)
    {
        //busca el componeente IPowerUpCollector si lo tiene llama a addCharge
        //si Add Charge es true osea -> recogo otro PowerUpIdentico al que tengo equipado
        //o es la primera vez que agarro el Power Up de este tipo se me suma a las cargas
        if (!other.TryGetComponent<IPowerUpCollector>(out var collector)) return;
        if (!collector.AddCharge(_config)) return;
        SoundManager.Instance.Play(SoundId.PickUpItem);
        gameObject.SetActive(false);
    }
}
