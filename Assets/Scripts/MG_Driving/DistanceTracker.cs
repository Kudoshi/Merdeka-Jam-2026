using TMPro;
using UnityEngine;

public class DistanceTracker : MonoBehaviour
{
    [SerializeField] private float _distanceGoal;
    [SerializeField] private TextMeshProUGUI _distanceText;

    [SerializeField, ReadOnly] private float _distance;

    

    private void Update()
    {
        _distance += CarDrivingSpeed.CurrentSpeed * Time.deltaTime;

        if (_distance >= _distanceGoal)
        {
            MinigameManager.Instance.WinMinigame();
        }


        _distanceText.text = $"{(_distanceGoal - _distance).ToString("F1")}m left";
    }

}
