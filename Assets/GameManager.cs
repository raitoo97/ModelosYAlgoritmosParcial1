using UnityEngine;
public class GameManager : MonoBehaviour
{
    public Player player;
    public static GameManager instance;
    public Transform _projectilesParent;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }
}
