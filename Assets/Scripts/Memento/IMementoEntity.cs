public interface IMementoEntity<T>
{
    void SaveState();
    void LoadState(T memory);
    void TryLoadStates();
}
