using Kudoshi.Utilities;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MinigameManager : Singleton<MinigameManager>
{
    public static event Action<GameState> OnGameStateChanged;

    [SerializeField] private SO_Minigame _minigameSO;
    [SerializeField] private int _lifeMaxCount = 5;
    [SerializeField] private float _timeWaitAfterMinigameEnd = 3.0f;

    private GameState _gameLevelType;
    private MinigameData _currentMinigame = new MinigameData();

    // Data
    [SerializeField, ReadOnly] private int _minigameRound;
    [SerializeField, ReadOnly] private int _currentLife;

    public int MinigameRound { get => _minigameRound; }
    public int CurrentLife { get => _currentLife; }

    private void Awake()
    {
        SetSingletonDontDestroyOnLoad(this);
        StartGameSession();
    }

    public void StartGameSession()
    {
        _gameLevelType = GameState.TRANSITION_SCENE;
        _minigameRound = 1;
        _currentLife = _lifeMaxCount;
    }

    public void LoseMinigame()
    {
        if (_gameLevelType != GameState.TRANSITION_SCENE)
        {
            _currentLife--;
        }
        Debug.Log("Game End");

        OnGameStateChanged?.Invoke(GameState.MINIGAME_SCENE_LOSE);

        MinigameEnd();
    }

    public void WinMinigame()
    {
        OnGameStateChanged?.Invoke(GameState.MINIGAME_SCENE_WIN);
        Debug.Log("Game Win");

        Util.WaitForSeconds(this, () =>
        {
            MinigameEnd();
        }, _timeWaitAfterMinigameEnd);
    }

    private void MinigameEnd()
    {
        Util.WaitForSeconds(this, () =>
        {
            if (_currentLife <= 0)
            {
                GameEnd();
                return;
            }

            GoNextScene();
        }, _timeWaitAfterMinigameEnd);
    }

    private void GoNextScene()
    {
        // Go into minigame
        if (_gameLevelType == GameState.TRANSITION_SCENE)
        {
            _gameLevelType =  GameState.MINIGAME_SCENE;
            MinigameData minigame = _minigameSO.RandomizeMinigameData(_currentMinigame.SceneName);
            SceneManager.LoadScene(minigame.SceneName);
        }
        // Go into transition scene
        else
        {
            _gameLevelType = GameState.TRANSITION_SCENE ;
            _minigameRound++;

            SceneManager.LoadScene("TransitionScene");
        }
    }


    private void GameEnd()
    {
        Debug.Log("Game entirely ends");
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Z) && Input.GetKeyDown(KeyCode.O))
        {
            LoseMinigame();
        }
        if (Input.GetKey(KeyCode.Z) && Input.GetKeyDown(KeyCode.P))
        {
            WinMinigame();
        }
        if (Input.GetKey(KeyCode.Z) && Input.GetKeyDown(KeyCode.L))
        {
            _gameLevelType = GameState.TRANSITION_SCENE;
            MinigameEnd();
        }

    }
}

public enum GameState
{
    TRANSITION_SCENE, MINIGAME_SCENE, MINIGAME_SCENE_LOSE, MINIGAME_SCENE_WIN
}