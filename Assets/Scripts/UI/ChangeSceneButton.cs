using UnityEngine;
using UnityEngine.SceneManagement;
public class ChangeSceneButton : MonoBehaviour
{
    public void SelectScene(int index)
    {
        SceneManager.LoadScene(index);
    }
}
