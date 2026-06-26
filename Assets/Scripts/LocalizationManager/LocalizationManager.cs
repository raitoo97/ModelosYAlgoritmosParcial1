using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public class LocalizationManager : MonoBehaviour
{
    public static LocalizationManager Instance { get; private set; }
    private string _webURL = "https://docs.google.com/spreadsheets/d/e/2PACX-1vRv1q9nQCMNFiwF8wBqwJdDnGmGzk-mHFcTb1gH3sLblpw1RISs724svd5pqgbC_h0sPeurnhnhq7J4/pub?output=csv";
    [SerializeField] private SystemLanguage _currentLengugage;
    Dictionary<SystemLanguage, Dictionary<string, string>> _lenguageCodex;
    public event Action onUpdate = delegate { };
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        StartCoroutine(InitializeCodex());
    }
    private IEnumerator InitializeCodex()
    {
        var www = new UnityWebRequest(_webURL);
        www.downloadHandler = new DownloadHandlerBuffer();
        yield return www.SendWebRequest();
        if (www.result == UnityWebRequest.Result.Success)
        {
            var result = www.downloadHandler.text;
            _lenguageCodex = LenguageSplit.LoadCSV(result);
            onUpdate.Invoke();
        }
    }
    public void ChangeLanguage(SystemLanguage language)
    {
        if (_currentLengugage == language)
            return;
        _currentLengugage = language;
        onUpdate?.Invoke();
    }
    public string GetTranslation(string ID)
    {
        if (!_lenguageCodex.ContainsKey(_currentLengugage))
            return "No Lenguage";
        if (!_lenguageCodex[_currentLengugage].ContainsKey(ID))
            return "No ID";
        return _lenguageCodex[_currentLengugage][ID];
    }
}
