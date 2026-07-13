using UnityEngine;
public static class LineOfSight
{
    public static bool IsOnSight(Vector3 start, Vector3 end)
    {
        var direction = end - start;
        return !Physics.Raycast(start + Vector3.up, direction.normalized, direction.magnitude, LayerMask.GetMask("Wall", "Ground"));
    }
}
