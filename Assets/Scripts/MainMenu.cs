using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private CustomButton _startBtn;
    [SerializeField] private CustomButton _quitBtn;

    private void OnEnable()
    {
        _startBtn.onClick.AddListener(StartBtnClicked);
        _quitBtn.onClick.AddListener(QuitBtnClicked);
    }


    private void StartBtnClicked()
    {
        SoundManager.Instance.PlaySound("sfx_engine_start");

        SceneManager.LoadScene("TransitionScene");
    }

    private void QuitBtnClicked()
    {
        Application.Quit();
    }


}
