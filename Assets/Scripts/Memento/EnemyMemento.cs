using UnityEngine;
public struct EnemyMemento
{
    public float life;
    public bool isDead;
    public bool isActive;
    public Vector3 position;
    public Quaternion rotation;
    public FSM.StateID currentState;
}
