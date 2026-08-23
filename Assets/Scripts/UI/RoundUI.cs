using TMPro;
using UnityEngine;

public class RoundUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _roundText;
    private void Start()
    {
        _roundText.text = "ROUND " + MinigameManager.Instance.MinigameRound.ToString();
    }
}
