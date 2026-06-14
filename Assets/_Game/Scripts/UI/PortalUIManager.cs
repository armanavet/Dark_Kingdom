using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class PortalUIManager : MonoBehaviour
{
    [SerializeField] private Button toggleButton;
    [SerializeField] private RectTransform mainPanel;
    [SerializeField] private Vector2 openedPosition = new Vector2(-190, -265);
    [SerializeField] private Vector2 closedPosition = new Vector2(-163, -58);
    [SerializeField] private float enableDuration = 0.3f;
    [SerializeField] private float disableDuration = 0.3f;
    [SerializeField] private Ease enableEase = Ease.OutBack;
    [SerializeField] private Ease disableEase = Ease.InBack;

    private void OnEnable()
    {
        toggleButton.onClick.AddListener(TogglePanel);
    }

    private void OnDisable()
    {
        toggleButton.onClick.RemoveAllListeners();
    }

    private void Start()
    {
        closedPosition = toggleButton.transform.position;
        mainPanel.position = closedPosition;
        mainPanel.gameObject.SetActive(false);
    }

    public void TogglePanel()
    {
        Sequence seq = DOTween.Sequence();

        if (!mainPanel.gameObject.activeSelf)
        {
            seq.AppendCallback(() => mainPanel.gameObject.SetActive(true))
               .Append(mainPanel.DOScale(1f, enableDuration).SetEase(enableEase))
               .Join(mainPanel.DOAnchorPos(openedPosition, enableDuration).SetEase(enableEase));
        }
        else
        {
            seq.Append(mainPanel.DOScale(0, disableDuration).SetEase(disableEase))
               .Join(mainPanel.DOMove(closedPosition, disableDuration).SetEase(disableEase))
               .AppendCallback(() => mainPanel.gameObject.SetActive(false));
        }
    }
}
