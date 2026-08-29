using DG.Tweening;
using UnityEngine;

public class MyviAnimation : MonoBehaviour
{
    [SerializeField] private float _rumbleDuration = 0.1f;
    [SerializeField] private float _positionStrength = 0.02f;
    [SerializeField] private float _rotationStrength = 1.5f;
    [SerializeField] private int _vibrato = 8;
    [SerializeField] private float _randomness = 90f;

    private Tween _rumbleTween;

    private void Start()
    {
        StartRumble();
    }

    public void StartRumble()
    {
        _rumbleTween?.Kill();

        _rumbleTween = DOTween.Sequence()
            .Append(transform.DOShakePosition(_rumbleDuration, _positionStrength, _vibrato, _randomness, false, true))
            .Join(transform.DOShakeRotation(_rumbleDuration, new Vector3(0f, 0f, _rotationStrength), _vibrato, _randomness, false))
            .SetLoops(-1, LoopType.Restart);
    }

    public void StopRumble()
    {
        _rumbleTween?.Kill();
        _rumbleTween = null;
    }
}
