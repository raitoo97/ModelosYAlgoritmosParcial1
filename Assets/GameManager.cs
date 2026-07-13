using UnityEngine;
//Game Manager
public class GameManager : MonoBehaviour
{
    public Player player;
    public static GameManager instance;
    public Transform projectilesParent;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);
    }
}
