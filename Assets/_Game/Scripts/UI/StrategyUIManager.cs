using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class StrategyUIManager : MonoBehaviour
{
    [SerializeField] private RectTransform currentStrategy;
    [SerializeField] private RectTransform allStrategies;
    [SerializeField] private RectTransform shadow;
    [SerializeField] private Button changeStrategyButton;
    [SerializeField] private Button[] allStrategyButtons;
    [SerializeField] private UnityEngine.UI.Outline currentStrategyOutline;
    [SerializeField] private Image currentStrategyIcon;
    [SerializeField] private Image strategyCooldown;
    [SerializeField] private float enableDuration = 0.3f;
    [SerializeField] private Ease enableEase = Ease.OutBack;
    [SerializeField] private float disableDuration = 0.3f;
    [SerializeField] private Ease disableEase = Ease.InBack;
    private Sequence showStrategiesSeq;
    private Sequence hideStrategiesSeq;

    public void Initialize()
    {
        SetUpAnimations();
        currentStrategy.gameObject.SetActive(true);
        currentStrategyOutline.enabled = true;
        strategyCooldown.fillAmount = 0;
        changeStrategyButton.interactable = true;
        allStrategies.gameObject.SetActive(false);
    }

    private void SetUpAnimations()
    {
        showStrategiesSeq = DOTween.Sequence().SetUpdate(true).SetAutoKill(false);
        showStrategiesSeq.AppendCallback(() =>
        {
            currentStrategyOutline.enabled = false;
            allStrategies.gameObject.SetActive(true);
            shadow.DOKill(false);
            allStrategies.DOKill(false);
        })
        .Join(allStrategies.DOAnchorMax(Vector2.one, enableDuration).From(new Vector2(1 / 3f, 1f)).SetEase(enableEase))
        .Join(shadow.DOAnchorMax(Vector2.one, enableDuration).From(new Vector2(1 / 3f, 1f)).SetEase(enableEase))
        .OnComplete(() =>
        {
            currentStrategy.gameObject.SetActive(false);
        });
        showStrategiesSeq.Pause();



        hideStrategiesSeq = DOTween.Sequence().SetUpdate(true).SetAutoKill(false);
        hideStrategiesSeq.AppendCallback(() =>
        {
            currentStrategy.gameObject.SetActive(true);
            allStrategies.DOKill(false);
            shadow.DOKill(false);
        })
        .Join(allStrategies.DOAnchorMax(new Vector2(1 / 3f, 1f), disableDuration).SetEase(disableEase))
        .Join(shadow.DOAnchorMax(new Vector2(1 / 3f, 1f), disableDuration).SetEase(disableEase))
        .OnComplete(() =>
        {
            allStrategyButtons[(int)StrategyManager.Instance.CurrentStrategy].transform.SetSiblingIndex(0);
            LayoutRebuilder.ForceRebuildLayoutImmediate(allStrategies);
            allStrategies.gameObject.SetActive(false); currentStrategyOutline.enabled = true;
        });
        hideStrategiesSeq.Pause();
    }

    public void ShowStrategies()
    {
        showStrategiesSeq.Restart();
    }

    public void HideStrategies()
    {
        hideStrategiesSeq.Restart();
    }

    public void UpdateCooldown(float remaining, float total)
    {
        if (remaining <= 0)
        {
            strategyCooldown.fillAmount = 0;
            changeStrategyButton.interactable = true;
            return;
        }

        strategyCooldown.fillAmount = remaining / total;
    }

    public void OnStrategyChanged(Strategy newStrategy)
    {
        currentStrategyIcon.sprite = newStrategy.Icon;
        changeStrategyButton.interactable = false;
    }
}
