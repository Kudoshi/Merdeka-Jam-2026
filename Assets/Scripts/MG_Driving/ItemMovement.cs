using UnityEngine;

public class ItemMovement : MonoBehaviour
{
    [SerializeField] private float _extraSpeed;
    [SerializeField] private bool _isConsistentSpeed = false;
    [SerializeField] private float _consistentSpeed;

    private void Update()
    {
        if (_isConsistentSpeed)
        {
            transform.position = transform.position + ((-transform.forward * _consistentSpeed * Time.deltaTime));

        }
        else
            transform.position = transform.position + ((-transform.forward * CarDrivingSpeed.CurrentSpeed * Time.deltaTime) + (transform.forward * _extraSpeed * Time.deltaTime));
    }
}
