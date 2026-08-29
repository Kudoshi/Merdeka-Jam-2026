
using UnityEngine;

public class GrassTrigger : MonoBehaviour
{
    private CarDrivingSpeed _carSpeed;

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (_carSpeed == null)
            {
                _carSpeed = other.gameObject.GetComponent<CarDrivingSpeed>();
            }

            _carSpeed.HitGrassLand();
        }
    }
}