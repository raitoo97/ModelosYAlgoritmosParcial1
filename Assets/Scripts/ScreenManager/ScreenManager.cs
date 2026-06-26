using System.Collections.Generic;
using UnityEngine;
public class ScreenManager : MonoBehaviour
{
    public static ScreenManager Instance { get; private set; }
    private Stack<IScreen> screenStack = new Stack<IScreen>();
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    public void Pop()
    {
        if (screenStack.Count == 1) return;
        var oldScreen = screenStack.Pop();
        oldScreen.Release();
        screenStack.Peek().Activate();
    }
    public void Push(string resource)
    {
        var Screen = Instantiate(Resources.Load<ScreenBase>(resource));
        Push(Screen);
    }
    private void Update()
    {
        Debug.Log(screenStack.Count);
    }
    public void Push(IScreen screen)
    {
        if (screenStack.Count > 0)
        {
            screenStack.Peek().Deactivate();
        }
        screenStack.Push(screen);
        screen.Activate();
    }
    public bool StackContainsType<T>()
    {
        foreach (var screen in screenStack)
            if (screen is T) return true;
        return false;
    }
}
