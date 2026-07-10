using UnityEngine;
public static class FactionRules
{
    public static bool ShouldHit(Collider target, Factions attacker)
    {
        if (target.TryGetComponent<IFactionMember>(out var member))
            return member.Faction != attacker;
        return true;
    }
}
