using UnityEngine;
public class SniperSpawnerManager : EnemySpawnerManager
{
    [SerializeField] private Transform[] _spawnPoints;
    [SerializeField] private LayerMask _hitMask;
    [SerializeField] private float _respawnTime;
    [SerializeField] private float _respawnTimeDecrease = 1f;
    [SerializeField] private float _minRespawnTime = 1f;
    private SniperSlot[] _slots;
    private class SniperSlot
    {
        public Transform point;
        public Enemy sniper;
        public float timer;
    }
    protected override void Start()
    {
        base.Start(); // la base crea el EnemyService y se suscribe al SaveManager
        //voy a tener misma cantidad de slots que de _spawnPoints
        //ademas a cada slot de sniper le asigno un _spawnPoints
        _slots = new SniperSlot[_spawnPoints.Length];
        for (int i = 0; i < _spawnPoints.Length; i++)
            _slots[i] = new SniperSlot { point = _spawnPoints[i] };
    }
    protected override IShootStrategy CreateShootStrategy()
    {
        return new HitscanShootStrategy(FlyWeightPointer.Sniper.damage, Factions.Enemy, _hitMask, FlyWeightPointer.Sniper.maxDistance);
    }
    protected override int GetPoolSize()
    {
        return _spawnPoints.Length;
    }
    protected override void IncreaseDifficulty()
    {
        _respawnTime = Mathf.Max(_minRespawnTime, _respawnTime - _respawnTimeDecrease);
    }
    protected override void OnActivated()
    {
        // cuando se activa le digo que spwneen el enemy sniper en la posicion que le setee en el start
        //_enemyService.Spawn ->
        //Enemy enemy = _pool.GetObject();
        //enemy.WarpToPosition(position);
        //enemy.ResetEnemy();
        //return enemy;
        foreach (SniperSlot slot in _slots)
            slot.sniper = _enemyService.Spawn(slot.point.position);
    }
    private void Update()
    {
        if (!IsActive) return;
        SpawnSnipers();
    }
    private void SpawnSnipers()
    {
        foreach (SniperSlot slot in _slots)
        {
            // si el sniper existe y esta activo, esta vivo entonces lo salteo, no hay nada que respawnear porque esta fuera del pool
            if (slot.sniper != null && slot.sniper.gameObject.activeSelf) continue;
            //seteo el sniper en nulo
            slot.sniper = null;
            //le sumo el timer a ese slot
            slot.timer += Time.deltaTime;
            if (slot.timer >= _respawnTime)
            {
                //cuando el timer llegue al _respawnTime lo seteo a 0 y llamo a enemy service del padre y spwneo un sniper en su correspondiente posicion
                slot.timer = 0;
                Enemy spawned = _enemyService.Spawn(slot.point.position);
                //este for each limpia referencias viejas
                //other != slot -> si soy yo mismo me salteo porque quiero quedarme con la referencia del nuevo spawned
                // other.sniper == spawned -> ese sniper es lo mismo que saque del pool porque si es el mismo hay dos apuntando al mismo sniper, si tiene a otro o a nadie , no pasa nada :D
                // bueno si se cumple osea basicamente limpio al otro para que ese otro slot quede vacio.
                //y ahora si a mi slot nuevo le asigno el sniper que saque del pool
                foreach (SniperSlot other in _slots)
                    if (other != slot && other.sniper == spawned) other.sniper = null;
                slot.sniper = spawned;
            }
        }
    }
}
