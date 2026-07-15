using TMPro;
using UnityEngine;
public class HighScoreDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _highScoreText;
    private void Start()
    {
        int highScore = PlayerPrefs.GetInt(GameManager.HighScoreKey, 0);
        _highScoreText.text = highScore.ToString();
    }
}
