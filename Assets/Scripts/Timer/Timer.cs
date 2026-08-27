using Kudoshi.Utilities;
using UnityEngine;

public class Timer : Singleton<Timer>
{
    [SerializeField] private float _startingTimer;
    [SerializeField] private float _timerDecreaseMultiplier = .75f;
    [SerializeField] private bool _adjustTimeAccordingToRound = true;

    private float _timeRemaining;
    private float _maxTime;
    private bool _timerPlay = true;

    public float TimeRemaining { get => _timeRemaining; }
    public float MaxTime { get => _maxTime; }

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
        if (state != GameState.MINIGAME_SCENE)
        {
            _timerPlay = false;
        }
    }

    private void Start()
    {
        _maxTime = _startingTimer;

        if (_adjustTimeAccordingToRound)
        {
            int round = MinigameManager.Instance.MinigameRound;
            _timeRemaining = _startingTimer * (Mathf.Pow(_timerDecreaseMultiplier, round - 1));
        }
        else
        {
            _timeRemaining = _startingTimer;
        }


        Debug.Log($"[Timer] Starting Timer: {_startingTimer} | Current Timer: {_timeRemaining}");
    }

    private void Update()
    {
        if (!_timerPlay) return;

        
        _timeRemaining -= Time.deltaTime;
        
        if (_timeRemaining <= 0)
        {
            MinigameManager.Instance.LoseMinigame();
            _timeRemaining = 0;
            _timerPlay = false;
        }
    }
}