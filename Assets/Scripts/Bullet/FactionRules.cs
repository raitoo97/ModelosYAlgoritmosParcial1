using UnityEngine;
public static class FactionRules
{
    // Regla UNICA de fuego amigo. Cualquier cambio de reglas se hace solo aca.
    // Si el objetivo no declara faccion (sin IFactionMember), se dania igual.
    public static bool ShouldHit(Collider target, Factions attacker)
    {
        if (target.TryGetComponent<IFactionMember>(out var member))
            return member.Faction != attacker;
        return true;
    }
}
