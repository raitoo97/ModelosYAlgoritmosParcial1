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
        // GetComponentInParent en vez de TryGetComponent: el collider que
        // entra puede vivir en un GameObject distinto al del collector
        // (colliders compuestos, hijos, etc). Busca en el propio y sube
        // por la jerarquia.
        IPowerUpCollector collector = other.GetComponentInChildren<IPowerUpCollector>();
        if (collector == null) return;
        // Si el collector no la acepta (tiene otro power up corriendo),
        // el item queda en el mapa para despues.
        if (!collector.AddCharge(_config)) return;
        gameObject.SetActive(false);
    }
}
