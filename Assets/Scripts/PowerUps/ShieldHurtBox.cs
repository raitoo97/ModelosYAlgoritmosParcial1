using UnityEngine;
// Hurtbox de la burbuja: absorbe las balas enemigas para que exploten
// en la superficie del escudo y no adentro.
// - IDamageable: Bullet.OnTriggerEnter entra por la rama de danio y llama
//   Impact() (VFX + vuelta al pool). El danio se descarta aca: la
//   invulnerabilidad real se resuelve en PlayerModel.TakeDamage.
// - IFactionMember: FactionRules hace que las balas PROPIAS atraviesen la
//   burbuja (misma faccion = no hit) y las enemigas impacten.
public class ShieldHurtBox : MonoBehaviour, IDamageable, IFactionMember
{
    [SerializeField] private Factions _faction = Factions.Player;
    public Factions Faction => _faction;
    public void TakeDamage(float dmg)
    {
        // Vacio a proposito: absorber. La bala ya hace su Impact() al pegarnos.
    }
}
