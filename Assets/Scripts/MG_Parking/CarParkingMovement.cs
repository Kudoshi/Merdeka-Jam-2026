using System;
using UnityEngine;

public class CarParkingMovement : MonoBehaviour
{
    [SerializeField] private float _driveSpeed;
    [SerializeField] private float _maxSpeed;

    [SerializeField] private float _turnSpeed;

    [SerializeField] private float _speedAffectRotationAmount;
    [SerializeField] private float _grip = 10f;
    private Rigidbody _rb;
    private Vector2 _input;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        _input = new Vector2();

        if (Input.GetKey(KeyCode.W))
        {
            _input.y = 1.0f;
        }
        else if (Input.GetKey(KeyCode.S)) 
        {
            _input.y = -1f;
        }

        if (Input.GetKey(KeyCode.D))
        {
            _input.x = 1.0f;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            _input.x = -1f;
        }

        Steer();
        Accelerate();
        PreventSkid();
    }
   

    private void Steer()
    {
        Vector3 velocity = _rb.linearVelocity;
        float speed = velocity.magnitude;

        if (speed > 0.1f)
        {
            float speedPercent = Mathf.Clamp01(speed / _maxSpeed);

            // Highest at 50% speed, lowest at 0% and 100%
            float speedMultiplier = 1f - Mathf.Abs(speedPercent * 2f - 1f);

            float rotationAmt = _input.x * _turnSpeed * speedMultiplier * Time.deltaTime;

            _rb.MoveRotation(_rb.rotation * Quaternion.Euler(0f, rotationAmt, 0f));
        }
    }

    private void Accelerate()
    {
        if (_rb.linearVelocity.magnitude > _maxSpeed) return;

        Vector3 force = transform.forward * _input.y * _driveSpeed * Time.deltaTime;

        _rb.AddForce(force, ForceMode.Force);
    }

    private void PreventSkid()
    {
        Vector3 localVelocity = transform.InverseTransformDirection(_rb.linearVelocity);

        localVelocity.x = Mathf.Lerp(localVelocity.x, 0f, _grip * Time.deltaTime);

        _rb.linearVelocity = transform.TransformDirection(localVelocity);
    }
}
