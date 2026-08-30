using TMPro;
using UnityEngine;

public class DistanceTracker : MonoBehaviour
{
    [SerializeField] private float _distanceGoal;
    [SerializeField] private TextMeshProUGUI _distanceText;

    [SerializeField, ReadOnly] private float _distance;

    bool _gameEnded;
    

    private void Update()
    {
        if (_gameEnded) return;

        _distance += CarDrivingSpeed.CurrentSpeed * Time.deltaTime;

        if (_distance >= _distanceGoal)
        {
            MinigameManager.Instance.WinMinigame();
            _gameEnded = true;

            _distanceText.text = $"{0.ToString("F1")}m left";

            return;
        }


        _distanceText.text = $"{(_distanceGoal - _distance).ToString("F1")}m left";
    }

}
