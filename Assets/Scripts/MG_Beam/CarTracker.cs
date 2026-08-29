using Kudoshi.Utilities;
using System;
using System.Collections;
using UnityEngine;

public class CarTracker : Singleton<CarTracker>
{
    [SerializeField] private Transform _carPf;
    [SerializeField] private Transform[] _movePoints;
    [SerializeField] private float _carMoveSpeed;
    [SerializeField] private float _carTurnSpeed;
    [SerializeField] private Vector2 _honkRequired;
    [SerializeField] private int _carShooAmtRequired;

    [SerializeField] private GameObject _beamLight;
    [SerializeField] private float _beamDuration;

    private Transform _trackedCar;
    private bool _enableHonk;
    private int _honkCount;
    private AudioSource _honkAudio;
    private int _carShooAmount;

    private Coroutine _turnCr;

    private void Awake()
    {
        _honkAudio = GetComponent<AudioSource>();
    }

    private void Start()
    {
        _carShooAmount = _carShooAmtRequired;
        StartCoroutine(StartCar());
    }


    private IEnumerator StartCar()
    {
        _trackedCar = Instantiate(_carPf, _movePoints[0].position, _movePoints[0].rotation);

        while (Vector3.Distance(_trackedCar.position, _movePoints[1].position) >= 0.1f)
        {
            float positionZ = Mathf.Lerp(_trackedCar.position.z, _movePoints[1].position.z, _carMoveSpeed * Time.deltaTime);

            Vector3 pos = _trackedCar.position;
            pos.z = positionZ;
            _trackedCar.position = pos;

            yield return null;
        }

        _enableHonk = true;

        _honkCount = UnityEngine.Random.Range((int) _honkRequired.x, (int) _honkRequired.y);
    }

    private IEnumerator AwayCar()
    {
        Transform car = _trackedCar;

        while (Vector3.Distance(car.position, _movePoints[2].position) >= 0.1f)
        {
            car.position = Vector3.Lerp(car.position, _movePoints[2].position, _carTurnSpeed * Time.deltaTime);

            yield return null;
        }

        while (Vector3.Distance(car.position, _movePoints[3].position) >= 0.1f)
        {
            car.position = Vector3.Lerp(car.position, _movePoints[3].position, _carTurnSpeed * Time.deltaTime);

            yield return null;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Honk();
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            Unhonk();
        }
      
    }

    

    private void Honk()
    {
        if (_enableHonk)
        {
            _honkCount--;

            if (_honkCount <= 0)
            {
                _enableHonk = false;

                _turnCr = StartCoroutine(AwayCar());

                _carShooAmount--;

                if (_carShooAmount > 0)
                {
                    StartCoroutine(StartCar());
                }
                // Start move away
            }
        }

        _beamLight.gameObject.SetActive(true);
        _honkAudio.Play();
    }

    private void Unhonk()
    {
        _beamLight.gameObject.SetActive(false);
        _honkAudio.Stop();
    }
}
