
using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;

public class SamanUI : MonoBehaviour
{
    [SerializeField] private Image[] _samans;
    private Vector3 _originalScale;

    private void Awake()
    {
        _originalScale = transform.localScale;
    }
    private void OnEnable()
    {
        MinigameManager.OnGameStateChanged += OnGameStateChanged;
    }



    private void OnDisable()
    {
        MinigameManager.OnGameStateChanged -= OnGameStateChanged;

    }

    private void Start()
    {
        int lifeLeft = MinigameManager.Instance.CurrentLife;
        int samans = 5-lifeLeft;

        for (int i = 0; i < samans; i++)
        {
            _samans[i].gameObject.SetActive(true);
        }
    }

    private void OnGameStateChanged(GameState state)
    {
        if (state == GameState.MINIGAME_SCENE_LOSE)
        {
            int lifeLeft = MinigameManager.Instance.CurrentLife;
            int samans = 5 - lifeLeft;

            for (int i = 0; i < samans; i++)
            {
                if (!_samans[i].gameObject.activeInHierarchy)
                {
                    _samans[i].gameObject.SetActive(true);

                    _samans[i].transform.DOKill();
                    _samans[i].transform.localScale = Vector3.zero;

                    Sequence pop = DOTween.Sequence();

                    pop.Append(_samans[i].transform.DOScale(_originalScale * 1.25f, 0.15f).SetEase(Ease.OutBack));
                    pop.Append(_samans[i].transform.DOScale(_originalScale * 0.9f, 0.08f).SetEase(Ease.InOutQuad));
                    pop.Append(_samans[i].transform.DOScale(_originalScale, 0.08f).SetEase(Ease.OutBack));
                }

            }
        }
    }
}