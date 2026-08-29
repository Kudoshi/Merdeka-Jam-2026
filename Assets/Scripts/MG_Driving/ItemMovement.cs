using UnityEngine;

public class ItemMovement : MonoBehaviour
{
    [SerializeField] private float _extraSpeed;

    private void Update()
    {
        transform.position = transform.position + ((-transform.forward * CarDrivingSpeed.CurrentSpeed * Time.deltaTime) + (transform.forward * _extraSpeed * Time.deltaTime));
    }
}
