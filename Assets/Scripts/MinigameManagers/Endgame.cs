
using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Endgame : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _roundText;
    [SerializeField] private GameObject _newBest;
    [SerializeField] private TextMeshProUGUI _topRoundText;

    [SerializeField] private CustomButton _retryBtn;
    [SerializeField] private CustomButton _quitBtn;

    private void Start()
    {
        LoadText();

        _retryBtn.onClick.AddListener(ClickRetryBtn);
        _quitBtn.onClick.AddListener(QuitBtn);
    }

    private void LoadText()
    {
        int rounds = MinigameManager.Instance.MinigameRound;
        _roundText.text = rounds.ToString();

        int bestRound = PlayerPrefs.GetInt("BEST_ROUND", 0);
        
        if (rounds > bestRound)
        {
            PlayerPrefs.SetInt("BEST_ROUND", rounds);

            _topRoundText.text = "Top Round: " + rounds;
            _newBest.gameObject.SetActive(true);
        }
        else
        {
            _topRoundText.text = "Top Round: " + bestRound;
            _newBest.gameObject.SetActive(false);

        }
    }

    private void ClickRetryBtn()
    {
        MinigameManager.Instance.ResetGameSession();
        MinigameManager.Instance.ForceGameGameState(GameState.TRANSITION_SCENE);
        SceneManager.LoadScene("TransitionScene");

    }

    private void QuitBtn()
    {
        MinigameManager.Instance.ResetGameSession();
        MinigameManager.Instance.ForceGameGameState(GameState.MAIN_MENU);
        SceneManager.LoadScene("MainMenu");

    }
}