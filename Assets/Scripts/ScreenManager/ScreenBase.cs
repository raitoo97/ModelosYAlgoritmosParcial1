using UnityEngine;
public class ScreenBase : MonoBehaviour , IScreen
{
    public virtual void BTN_Back()
    {
        ScreenManager.Instance.Pop();
    }
    public virtual void Activate()
    {
        gameObject.SetActive(true);
    }
    public virtual void Deactivate()
    {
        gameObject.SetActive(false);
    }
    public virtual void Release()
    {
        Destroy(gameObject);
    }
}
