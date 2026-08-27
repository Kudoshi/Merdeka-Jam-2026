using UnityEngine;

public class Parking : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            MinigameManager.Instance.WinMinigame();
    }
}
