using System;
using System.Collections.Generic;
using UnityEngine;
public class CameraModel : IObserver<SaveEvent>, IMementoEntity<CameraMemento>
{
    private MementoState<CameraMemento> _cameraMemento;
    private Dictionary<SaveEvent, Action> _saveActions;
    private float _sensitivity;
    private bool _invertY;
    private float _xRotation;
    private float _yRotation;
    public CameraModel(float sensitivity, bool invertY)
    {
        _sensitivity = sensitivity;
        _invertY = invertY;
        _cameraMemento = new MementoState<CameraMemento>();
        FillDictionary();
    }
    private void FillDictionary()
    {
        _saveActions = new Dictionary<SaveEvent, Action>
        {
            { SaveEvent.Save, SaveState },
            { SaveEvent.Load, TryLoadStates }
        };
    }
    public Quaternion ComputeRotation(Vector2 lookInput)
    {
        Vector2 look = lookInput * _sensitivity;
        _xRotation += (_invertY ? -1 : 1) * look.y;
        _xRotation = Mathf.Clamp(_xRotation, -30f, 70f);
        _yRotation += look.x;
        return Quaternion.Euler(_xRotation, _yRotation, 0);
    }
    public void Notify(SaveEvent Actions)
    {
        if (_saveActions.ContainsKey(Actions))
            _saveActions[Actions].Invoke();
    }
    public void SaveState()
    {
        _cameraMemento.SaveMemory(new CameraMemento { xRotation = _xRotation, yRotation = _yRotation });
    }
    public void LoadState(CameraMemento memory)
    {
        _xRotation = memory.xRotation;
        _yRotation = memory.yRotation;
    }
    public void TryLoadStates()
    {
        if (_cameraMemento.memoriesAmount == 0) return;
        var lastMemory = _cameraMemento.LoadMemory();
        LoadState(lastMemory);
    }
}
