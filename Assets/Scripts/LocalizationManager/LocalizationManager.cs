using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public class LocalizationManager : MonoBehaviour
{
    public static LocalizationManager Instance { get; private set; }
    private string _webURL = "https://docs.google.com/spreadsheets/d/e/2PACX-1vRv1q9nQCMNFiwF8wBqwJdDnGmGzk-mHFcTb1gH3sLblpw1RISs724svd5pqgbC_h0sPeurnhnhq7J4/pub?output=csv";
    public bool IsReady => _lenguageCodex != null;
    [SerializeField] private SystemLanguage _currentLenguage;
    Dictionary<SystemLanguage, Dictionary<string, string>> _lenguageCodex;
    public event Action onUpdate = delegate { };
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            if (PlayerPrefs.HasKey("SelectedLanguage"))
                _currentLenguage = (SystemLanguage)PlayerPrefs.GetInt("SelectedLanguage");
            StartCoroutine(InitializeCodex());
        }
        else
        {
            Destroy(gameObject);
        }
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
        if (_currentLenguage == language)
            return;
        _currentLenguage = language;
        PlayerPrefs.SetInt("SelectedLanguage", (int)language);
        onUpdate?.Invoke();
    }
    public string GetTranslation(string ID)
    {
        if (_lenguageCodex == null)
            return string.Empty;
        if (string.IsNullOrEmpty(ID))
            return string.Empty;
        if (!_lenguageCodex.ContainsKey(_currentLenguage))
            return "No Lenguage";
        if (!_lenguageCodex[_currentLenguage].ContainsKey(ID))
            return "No ID";
        return _lenguageCodex[_currentLenguage][ID];
    }
}
