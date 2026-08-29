using AYellowpaper.SerializedCollections;
using System;
using System.Linq;
using TMPro;
using UnityEngine;

public class DriveHitTargetController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _hitText;
    [SerializeField] private SerializedDictionary<ObstacleType, int> _obstacleAllowHits;

    private ObstacleType _hitType;
    private int _hitCount;
    private int _hitGoalAmount;

    private void Start()
    {
        int hitTypeDecision = UnityEngine.Random.Range(0, _obstacleAllowHits.Count);
        _hitType = _obstacleAllowHits.ElementAt(hitTypeDecision).Key;
        _hitGoalAmount = _obstacleAllowHits.ElementAt(hitTypeDecision).Value;

        string displayName = _hitType.ToString().Replace("_", " ").ToLower();
        displayName = char.ToUpper(displayName[0]) + displayName.Substring(1);

        _hitText.text = $"Hit {_hitGoalAmount} {displayName}";
    }

    public bool HitTarget(Obstacle obstacle)
    {
        if (obstacle.GetObstacleType() != _hitType) return false;

        _hitCount++;

        if (_hitCount >= _hitGoalAmount)
        {
            MinigameManager.Instance.WinMinigame();
        }

        string displayName = _hitType.ToString().Replace("_", " ").ToLower();
        displayName = char.ToUpper(displayName[0]) + displayName.Substring(1);
        _hitText.text = $"Hit {_hitGoalAmount - _hitCount} {displayName}";


        return true;
    }
}
