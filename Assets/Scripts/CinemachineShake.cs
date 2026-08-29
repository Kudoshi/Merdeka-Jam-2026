using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Cinemachine;
using Kudoshi.Utilities;


public class CinemachineShake : Singleton<CinemachineShake> {

    private CinemachineCamera _camera;
    private CinemachineBasicMultiChannelPerlin _noise;

    private float _shakeTimer;
    private float _shakeTimerTotal;
    private float _startingIntensity;

    private void Awake()
    {
        _camera = GetComponent<CinemachineCamera>();
        _noise = _camera.GetComponent<CinemachineBasicMultiChannelPerlin>();
    }

    public void ShakeCamera(float intensity, float time)
    {
        _startingIntensity = intensity;
        _shakeTimerTotal = time;
        _shakeTimer = time;

        _noise.AmplitudeGain = intensity;
    }

    private void Update()
    {
        if (_shakeTimer <= 0f)
            return;

        _shakeTimer -= Time.deltaTime;

        float normalizedTime = 1f - (_shakeTimer / _shakeTimerTotal);
        _noise.AmplitudeGain = Mathf.Lerp(_startingIntensity, 0f, normalizedTime);

        if (_shakeTimer <= 0f)
            _noise.AmplitudeGain = 0f;
    }
}
