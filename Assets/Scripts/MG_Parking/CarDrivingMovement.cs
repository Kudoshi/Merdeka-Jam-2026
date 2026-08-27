using System;
using UnityEngine;

public class CarDrivingMovement : MonoBehaviour
{
    [SerializeField] private float _driveSpeed;
    [SerializeField] private float _driveHitSlow;
    [SerializeField] private float _steerSpeed;
    [SerializeField] private float _rotateSpeed;
    [SerializeField] private float _rotateSpeedBack;
    [SerializeField] private float _maxRotateAngle;

    private Rigidbody _rb;
    private Vector2 _input;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        MinigameManager.OnGameStateChanged += GameStateChanged;
    }

    private void OnDisable()
    {
        MinigameManager.OnGameStateChanged -= GameStateChanged;
    }

    private void GameStateChanged(GameState state)
    {
        if (state != GameState.MINIGAME_SCENE)
        {
            this.enabled = false;
        }
    }

    private void Update()
    {
        _input = new Vector2();

        //if (Input.GetKey(KeyCode.W))
        //{
        //    _input.y = 1.0f;
        //}
        //else if (Input.GetKey(KeyCode.S))
        //{
        //    _input.y = -1f;
        //}

        if (Input.GetKey(KeyCode.D))
        {
            _input.x = 1.0f;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            _input.x = -1f;
        }

        Steer();
        RotateModel();
    }

    private void Steer()
    {
        _rb.transform.position += new Vector3(_input.x * Time.deltaTime * _steerSpeed, 0, 0);
    }

    private void RotateModel()
    {
        if (_input.x != 0)
        {
            float maxRotateAngle = _input.x > 0 ? _maxRotateAngle : -_maxRotateAngle;
            float rotationY = Mathf.LerpAngle(_rb.rotation.eulerAngles.y, maxRotateAngle, _rotateSpeed * Time.deltaTime);
            Vector3 rotation = _rb.rotation.eulerAngles;
            rotation.y = rotationY;
            _rb.rotation = Quaternion.Euler(rotation);
        }
        else
        {
            float rotationY = Mathf.LerpAngle(_rb.rotation.eulerAngles.y, 0, _rotateSpeedBack * Time.deltaTime);
            Vector3 rotation = _rb.rotation.eulerAngles;
            rotation.y = rotationY;
            _rb.rotation = Quaternion.Euler(rotation);
        }
    }
}
