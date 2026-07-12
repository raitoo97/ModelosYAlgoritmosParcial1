using UnityEngine;
public abstract class ScreenRoot : MonoBehaviour
{
    public abstract IScreen CreateRootScreen();
}
