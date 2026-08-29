
using UnityEngine;

public class CarDrivingSpeed : MonoBehaviour
{
    [SerializeField] private float _startingSpeed;
    [SerializeField] private float _maxSpeed;
    [SerializeField] private float _increaseSpeed;
    [SerializeField] private float _shakeIntensity;
    [SerializeField] private float _shakeDuration;

    public static float CurrentSpeed;
    public static float MaxSpeed;

    private void Start()
    {
        CurrentSpeed = _startingSpeed;
        MaxSpeed = _maxSpeed;
    }

    private void Update()
    {
        CurrentSpeed += _increaseSpeed * Time.deltaTime;

        if (CurrentSpeed > _maxSpeed)
        {
            CurrentSpeed = _maxSpeed;
        }
    }

    public void HitObstacle()
    {
        CurrentSpeed /= 2;

        if (CurrentSpeed < 0)
        {
            CurrentSpeed = 0;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Car"))
        {
            HitObstacle();
            CinemachineShake.Instance.ShakeCamera(_shakeIntensity, _shakeDuration);
        }
    }
}