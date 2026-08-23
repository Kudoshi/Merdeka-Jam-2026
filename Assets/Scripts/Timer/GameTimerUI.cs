
using UnityEngine;
using UnityEngine.UI;

public class GameTimerUI : MonoBehaviour
{
    [SerializeField] private Image _timerFill;

    private void Update()
    {

        _timerFill.fillAmount = Timer.Instance.TimeRemaining / Timer.Instance.MaxTime;

    }
}