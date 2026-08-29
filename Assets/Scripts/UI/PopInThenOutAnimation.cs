using DG.Tweening;
using UnityEngine;

public class PopInThenOutAnimation : MonoBehaviour
{
    [SerializeField] private float _popInDuration = 0.2f;
    [SerializeField] private float _popOvershoot = 1.15f;
    [SerializeField] private float _popSettleDuration = 0.1f;
    [SerializeField] private float _stayDuration = 3f;
    [SerializeField] private float _popOutDuration = 0.2f;

    private Vector3 _originalScale;

    private void Awake()
    {
        _originalScale = transform.localScale;
    }

    private void Start()
    {
        PlayPop();
    }

    public void PlayPop()
    {
        transform.DOKill();
        transform.localScale = Vector3.zero;

        Sequence sequence = DOTween.Sequence();

        sequence.Append(transform.DOScale(_originalScale * _popOvershoot, _popInDuration).SetEase(Ease.OutBack));
        sequence.Append(transform.DOScale(_originalScale, _popSettleDuration).SetEase(Ease.OutQuad));
        sequence.AppendInterval(_stayDuration);
        sequence.Append(transform.DOScale(Vector3.zero, _popOutDuration).SetEase(Ease.InBack));
    }

}
