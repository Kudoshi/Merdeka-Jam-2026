using System;
using TMPro;
using UnityEngine;

public class WinLoseUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _winLoseText;

    private void OnEnable()
    {
        MinigameManager.OnGameStateChanged += OnGameStateChanged;
    }

    private void OnDisable()
    {
        MinigameManager.OnGameStateChanged -= OnGameStateChanged;

    }

    private void OnGameStateChanged(GameState state)
    {
        if (state == GameState.MINIGAME_SCENE_LOSE)
        {
            _winLoseText.text = "KENA SAMAN!";
            _winLoseText.gameObject.SetActive(true);
        }
        else if (state == GameState.MINIGAME_SCENE_WIN)
        {
            _winLoseText.text = "MYVI KING!";
            _winLoseText.gameObject.SetActive(true);

        }
    }

    private void Start()
    {
        _winLoseText.gameObject.SetActive(false);
    }
}
