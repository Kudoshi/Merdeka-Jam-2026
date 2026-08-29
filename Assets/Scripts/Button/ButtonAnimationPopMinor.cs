using UnityEngine;
using DG.Tweening;

public class ButtonAnimationPopMinor : ButtonAnimationBase
{
    private Tween _currentTween;
    private Tween _idleTween;

    private Vector3 _baseScale;

    private float _selectedScaleMultiplier = 1.1f;
    private float _pressScaleMultiplier = 0.85f;

    private Color _baseColor;
    private Color _selectedColor = Color.white;
    private Color _pressedColor = new Color(0.75f, 0.75f, 0.75f);

    private Vector3 _idleBasePos;
    private Vector3 _idleBaseRot;

    private bool _transformCached;

    public ButtonAnimationPopMinor(MonoBehaviour mb, ButtonAnimationType animationType, CustomButton button)
        : base(mb, animationType, button)
    {
        _baseScale = _button.transform.localScale;

        if (_target != null)
            _baseColor = _target.color;

    }

    private Vector3 GetScale(float multiplier)
    {
        return _baseScale * multiplier;
    }

    private void KillAll()
    {
        _currentTween?.Kill();
        _idleTween?.Kill();
        _button.transform.DOKill();
    }

    private void CacheBaseTransform()
    {
        _idleBasePos = _button.transform.localPosition;
        _idleBaseRot = _button.transform.localEulerAngles;

        _transformCached = true;
    }

    private void ResetTransform()
    {
        _button.transform.localScale = _baseScale;
        _button.transform.localPosition = _idleBasePos;
        _button.transform.localEulerAngles = _idleBaseRot;
    }

    private void StopIdle()
    {
        _idleTween?.Kill();
        _idleTween = null;
    }

    public override void OnButtonHighlighted()
    {
        if (!_transformCached)
            CacheBaseTransform();

        KillAll();
        StopIdle();

        ResetTransform();

        Sequence seq = DOTween.Sequence();
        seq.SetUpdate(true);

        // SMASH IN
        seq.Append(_button.transform
            .DOScale(GetScale(_selectedScaleMultiplier * 1.12f), 0.12f)
            .SetEase(Ease.OutQuad));

        // SETTLE
        seq.Append(_button.transform
            .DOScale(GetScale(_selectedScaleMultiplier), 0.22f)
            .SetEase(Ease.OutBack));

        // IMPACT PUNCH (main hit)
        seq.Join(_button.transform.DOPunchPosition(
            new Vector3(0f, 16f, 0f),
            0.28f,
            16,
            1f
        ));

        // EXTRA SHAKE (highlight energy burst)
        //seq.Join(_button.transform.DOShakePosition(
        //    0.1f,
        //    new Vector3(8f, 8f, 0f),
        //    20,
        //    90f,
        //    false,
        //    true
        //));

        // subtle color pop
        if (_target != null)
            seq.Join(_target.DOColor(_selectedColor * 1.1f, 0.15f));

        _currentTween = seq;

        //StartIdleBreathing();
    }

    public override void OnButtonDeselect()
    {
        if (!_transformCached)
            CacheBaseTransform();

        KillAll();
        StopIdle();

        ResetTransform();

        Sequence seq = DOTween.Sequence();
        seq.SetUpdate(true);

        seq.Append(_button.transform
            .DOScale(_baseScale, 0.25f)
            .SetEase(Ease.InOutBack));

        seq.Join(_button.transform
            .DOLocalRotate(_idleBaseRot, 0.2f)
            .SetEase(Ease.OutSine));

        seq.Join(_button.transform.DOPunchRotation(
            new Vector3(0f, 0f, -8f),
            0.2f,
            12,
            1f
        ));

        if (_target != null)
            seq.Join(_target.DOColor(_baseColor, 0.4f));

        _currentTween = seq;
    }

    public override void OnButtonClicked()
    {
        if (!_transformCached)
            CacheBaseTransform();

        KillAll();
        StopIdle();

        Sequence seq = DOTween.Sequence();
        seq.SetUpdate(true);

        seq.Append(_button.transform
            .DOScale(GetScale(_pressScaleMultiplier), 0.06f)
            .SetEase(Ease.OutQuad));

        seq.Append(_button.transform
            .DOScale(GetScale(_selectedScaleMultiplier * 1.05f), 0.14f)
            .SetEase(Ease.OutBack));

        seq.Append(_button.transform
            .DOScale(GetScale(_selectedScaleMultiplier), 0.18f)
            .SetEase(Ease.OutElastic));

        if (_target != null)
        {
            seq.Join(_target.DOColor(_pressedColor, 0.06f));
            seq.Append(_target.DOColor(_selectedColor, 0.15f));
        }

        seq.Join(_button.transform.DOShakePosition(
            0.14f,
            new Vector3(6f, 6f, 0f),
            22,
            120f,
            false,
            true
        ));

        seq.Join(_button.transform.DOPunchRotation(
            new Vector3(0f, 0f, 18f),
            0.22f,
            16,
            1f
        ));

        _currentTween = seq;

    }
}