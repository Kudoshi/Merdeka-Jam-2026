
using UnityEngine;

[RequireComponent(typeof(CarInvincibility))]
public class CarDrivingSpeed : MonoBehaviour
{
    [SerializeField] private float _startingSpeed;
    [SerializeField] private float _maxSpeed;
    [SerializeField] private float _increaseSpeed;
    [SerializeField] private float _shakeIntensity;
    [SerializeField] private float _shakeDuration;
    [SerializeField] private CarInvincibility _invincibility;
    [SerializeField] private float _invincibilityTime;
    [SerializeField] private float _minimumSpeed = 2f;

    [Header("Grass shake")]
    [SerializeField] private float _shakeIntensityGrass;
    [SerializeField] private float _shakeDurationGrass;
    [SerializeField] private float _grassSlowSpeed;


    public static float CurrentSpeed;
    public static float MaxSpeed;
    private bool _invincible = false;

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
        else if (CurrentSpeed < _minimumSpeed)
        {
            CurrentSpeed = _minimumSpeed;
        }

    }

    public void HitObstacle()
    {
        CinemachineShake.Instance.ShakeCamera(_shakeIntensity, _shakeDuration);

        _invincibility.TriggerInvincibility(_invincibilityTime);
        _invincible = true;

        Util.WaitForSeconds(this, () =>
        {
            _invincible = false;
        }, _invincibilityTime);

        CurrentSpeed /= 2;

        if (CurrentSpeed < 0)
        {
            CurrentSpeed = 0;
        }
    }

    public void HitGrassLand()
    {
        CinemachineShake.Instance.ShakeCamera(_shakeIntensityGrass, _shakeDurationGrass);

        CurrentSpeed -= _grassSlowSpeed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_invincible) return;

        if (other.CompareTag("Car"))
        {
            HitObstacle();
        }
    }
}