
using UnityEngine;

public class ParkingRandomizer : MonoBehaviour
{
    [SerializeField] private GameObject[] _parkings;

    private void Start()
    {
        int decision = UnityEngine.Random.Range(0, _parkings.Length);

        for (int i = 0; i < _parkings.Length; i++)
        {
            if (decision == i)
            {
                _parkings[i].SetActive(true);
            }
            else
            {
                _parkings[i].SetActive(false);
            }
        }
    }

}