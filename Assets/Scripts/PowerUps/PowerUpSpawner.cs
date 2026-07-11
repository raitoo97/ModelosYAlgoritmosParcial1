using System.Collections.Generic;
using UnityEngine;
// Spawner simple de pickups. Anti-bugs por disenio:
// - Los puntos de spawn son los HIJOS de este GameObject, puestos a mano:
//   sin posiciones aleatorias no hay items adentro de estructuras.
// - Cada punto se "apoya" en el piso con un raycast hacia abajo: aunque el
//   punto quede flotando o medio hundido, el item aparece sobre el suelo.
// - El pickup ya se desactiva solo al consumirse (OnTriggerEnter): aca solo
//   se lo reactiva pasado el cooldown. Mini-pool de 1 instancia por punto.
public class PowerUpSpawner : MonoBehaviour, IPauseable
{
    // Un slot por punto de spawn: el pickup instanciado y SU timer de
    // respawn, juntos (mismo esquema que PowerUpSlot en el controller).
    // Es clase y no struct a proposito: la lista guarda la referencia y
    // el timer se puede modificar en el lugar.
    private class SpawnSlot
    {
        public PowerUpPickup pickup;
        public float timer;
    }
    // Prefabs posibles; en cada punto aparece uno al azar (sorteo unico en Start).
    [SerializeField] private List<PowerUpPickup> _pickupPrefabs;
    [SerializeField] private float _respawnSeconds = 30f;
    // Layers que cuentan como piso para apoyar el item.
    [SerializeField] private LayerMask _groundMask;
    // Altura del item sobre el piso.
    [SerializeField] private float _itemHeight = 0.5f;
    private List<SpawnSlot> _slots = new List<SpawnSlot>();
    private void Start()
    {
        if (_pickupPrefabs == null || _pickupPrefabs.Count == 0)
        {
            Debug.LogWarning("PowerUpSpawner: no hay prefabs de pickup asignados.");
            return;
        }
        // un pickup por cada hijo: cada hijo vacio ES un punto de spawn
        foreach (Transform point in transform)
        {
            PowerUpPickup prefab = _pickupPrefabs[Random.Range(0, _pickupPrefabs.Count)];
            Vector3 position = SnapToGround(point.position, point.name);
            PowerUpPickup pickup = Instantiate(prefab, position, prefab.transform.rotation, point);
            _slots.Add(new SpawnSlot { pickup = pickup, timer = 0f });
        }
    }
    // Rayo desde arriba del punto hacia abajo: donde pega en el piso, ahi va
    // el item. Ignora triggers (otros pickups, la burbuja) para no apoyarse
    // sobre ellos. Si no encuentra piso, avisa y usa el punto tal cual.
    private Vector3 SnapToGround(Vector3 position, string pointName)
    {
        Ray ray = new Ray(position + Vector3.up * 5f, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, 50f, _groundMask, QueryTriggerInteraction.Ignore))
            return hit.point + Vector3.up * _itemHeight;
        Debug.LogWarning("PowerUpSpawner: el punto '" + pointName + "' no encontro piso debajo. Revisar su posicion o el groundMask.");
        return position;
    }
    private void Update()
    {
        foreach (SpawnSlot slot in _slots)
        {
            // Activo = esta en el mapa, nada que hacer.
            if (slot.pickup.gameObject.activeSelf) continue;
            // Inactivo = alguien lo agarro: cuento el respawn y lo reactivo
            // en el mismo lugar (la instancia nunca se movio, estaba apagada).
            slot.timer += Time.deltaTime;
            if (slot.timer >= _respawnSeconds)
            {
                slot.timer = 0f;
                slot.pickup.gameObject.SetActive(true);
            }
        }
    }
    // Congela los timers de respawn en pausa (mismo esquema que el
    // PowerUpController: la pausa apaga Updates, no usa timeScale).
    public void Pause()
    {
        enabled = false;
    }
    public void Resume()
    {
        enabled = true;
    }
    // En la Scene view muestra DONDE va a caer cada item de verdad:
    // esfera cyan = posicion final apoyada en el piso, linea = la caida
    // desde el punto. Asi se detecta a ojo un punto mal ubicado antes de Play.
    private void OnDrawGizmos()
    {
        foreach (Transform point in transform)
        {
            Vector3 snapped = SnapToGround(point.position, point.name);
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(snapped, 0.5f);
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(point.position, snapped);
        }
    }
}