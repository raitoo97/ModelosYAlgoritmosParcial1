using UnityEngine;
public class SniperView : EnemyView
{
    private LineRenderer _laser;
    private ParticleSystem _impactEffect;
    private const float LaserStartWidth = 0.35f;
    private const float LaserEndWidth = 0.05f;
    private static readonly Color LaserStartColor = new Color(0f, 1f, 0f, 0.35f);
    // nuevo: pesos del torso segun desnivel con el player
    private const float SameHeightThreshold = 2f;   // hasta esta diferencia en Y lo considero misma altura
    private const float FlatBodyWeight = 0.05f;     // misma altura: torso clavado en la animacion de aim
    private const float ElevatedBodyWeight = 0.3f;// hay desnivel: el torso sigue 
    private Transform _transform;
    private const float PullInDistance = 3f;
    private const float ChestHeight = 1.5f;
    public SniperView(Enemy user) : base(user)
    {
        _transform = user.transform;
        _laser = user.GetComponentInChildren<LineRenderer>(true);
        _impactEffect = user.GetComponentInChildren<ParticleSystem>(true); // pongo (true) para que busque en los hijos inactivos tambien, porque el particle system esta desactivado al inicio
    }
    protected override float GetBodyWeight(Vector3 aimPoint)
    {
        float heightDiff = Mathf.Abs(aimPoint.y - (_transform.position.y + ChestHeight)); // mido pecho contra pecho, simetrico arriba/abajo
        return heightDiff <= SameHeightThreshold ? FlatBodyWeight : ElevatedBodyWeight;
    }
    protected override Vector3 GetLookAtPoint(Vector3 aimPoint)
    {
        Vector3 chestPos = _transform.position + Vector3.up * ChestHeight;//vector que es la posicion actual del sniper + vector.up * 1.5
        Vector3 toTarget = aimPoint - chestPos; //saco un vector que apunta hacia el aimPoint que es el player
        Vector3 flat = new Vector3(toTarget.x, 0f, toTarget.z); // lo aplano a nivel del suelo, para calcular la distancia horizontal
        float dist = flat.magnitude; // distancia horizontal al player
        if (dist <= PullInDistance) return aimPoint; // ya esta cerca: dejo el punto real osea si la dsitancia del player < PullInDistance, devuelvo el aimPoint real
        // armo el punto falso en 3 pasos:
        // 1) parto del pecho del sniper (chestPos)
        // 2) avanzo solo PullInDistance metros en la direccion horizontal al player (flat/dist normaliza y * PullInDistance estira a 3) -> para normalizar un vector divido sus componente por su magnitud (dist)
        //en conclusion, tengo un vector de largo 1 y lo multiplico * 3 para estirarlo
        // 3)  y a ese punto le sumo la diferencia de altura real con el player (si está 4 abajo, toTarget.y es -4, así que el punto baja 4)
        return chestPos + flat / dist * PullInDistance + Vector3.up * toTarget.y;
    }
    public void ShowLaser(bool visible)
    {
        if (_laser == null) return;
        _laser.enabled = visible;
    }
    public void UpdateLaser(Vector3 origin, Vector3 end, float progress)
    {
        if (_laser == null) return;
        _laser.SetPosition(0, origin);
        _laser.SetPosition(1, end);
        Color color = Color.Lerp(LaserStartColor, Color.red, progress);
        _laser.startColor = color;
        _laser.endColor = color;
        float width = Mathf.Lerp(LaserStartWidth, LaserEndWidth, progress);
        _laser.startWidth = width;
        _laser.endWidth = width;
    }
    public void PlayShootEffects(ShotResult result)
    {
        if (!result.didHit || _impactEffect == null) return;
        _impactEffect.transform.SetPositionAndRotation(result.hitPoint, Quaternion.LookRotation(result.hitNormal));
        _impactEffect.Play();
    }
    protected override void OnEnemyDeath()
    {
        base.OnEnemyDeath();
        ShowLaser(false);   // que no siga el laser prendido mientras muere
    }
    protected override void ResetView()
    {
        base.ResetView();
        ShowLaser(false);
    }
}
