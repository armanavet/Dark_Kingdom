using AudioSystem;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StateUIManager : MonoBehaviour
{
    [SerializeField] private SoundData UISoundData;
    [HideInInspector] public float GameTimer;

    [Header("Active State")]
    [SerializeField] private GameObject activeStatePanel;
    [SerializeField] private Slider activeStateSlider;
    [SerializeField] private Transform activeStateWarningParent;
    [SerializeField] private Image activeStateWarningImage, activeStateWarningIcon;
    [SerializeField] private TextMeshProUGUI activeStateWarningText;
    [SerializeField] private TextMeshProUGUI waveText;
    [SerializeField] private float activeStateEnableDuration = 2f;
    [SerializeField] private float activeStateUptime = 5f;
    [SerializeField] private Ease activeStateEnableEase = Ease.OutBack;
    [SerializeField] private float activeStateDisableDuration = 0.5f;
    [SerializeField] private Ease activeStateDisableEase = Ease.InBack;

    [Header("Passive State")]
    [SerializeField] private GameObject passiveStatePanel;
    [SerializeField] private Slider passiveStateSlider;
    [SerializeField] private TextMeshProUGUI timerText;

    public void Initialize()
    {
        activeStatePanel.SetActive(false);
        passiveStatePanel.SetActive(false);
        UISoundData = AudioManager.Instance.SetData(SoundDataType.UI);
    }

    public void OnGameStateChanged(GameState newState)
    {
        if (newState == GameState.Passive)
        {
            activeStatePanel.SetActive(false);
            passiveStatePanel.SetActive(true);
        }
        else if (newState == GameState.Active)
        {
            activeStateWarningParent.gameObject.SetActive(true);
            activeStateWarningParent.localScale = Vector3.zero;
            Sequence seq = DOTween.Sequence();
            seq.Append(activeStateWarningParent.DOScale(1f, activeStateEnableDuration)).SetEase(activeStateEnableEase)
               .Join(activeStateWarningImage.DOFade(1f, activeStateEnableDuration).From(0.25f))
               .Join(activeStateWarningIcon.DOFade(1f, activeStateEnableDuration).From(0.25f))
               .Join(activeStateWarningText.DOFade(1f, activeStateEnableDuration).From(0.25f))
               .AppendInterval(activeStateUptime)
               .Append(activeStateWarningImage.DOFade(0.25f, activeStateDisableDuration))
               .Join(activeStateWarningIcon.DOFade(0.25f, activeStateDisableDuration))
               .Join(activeStateWarningText.DOFade(0.25f, activeStateDisableDuration))
               .Join(activeStateWarningParent.DOScale(0f, activeStateDisableDuration).SetEase(activeStateDisableEase))
               .OnComplete(() =>
               {
                   activeStateWarningParent.gameObject.SetActive(false);
                   activeStatePanel.SetActive(true);
                   passiveStatePanel.SetActive(false);
                   waveText.text = "Wave: " + WaveManager.Instance.CurrentWave.ToString();
                   activeStateSlider.value = 1f;
               });
        }
        else if (newState == GameState.End)
        {
            activeStatePanel.SetActive(false);
            passiveStatePanel.SetActive(false);
        }
    }

    public void UpdateTimer(float remaining, float total)
    {
        remaining = Mathf.Max(remaining, 0);

        timerText.text = $"{Mathf.FloorToInt(remaining / 60)}:" +
                         $"{Mathf.FloorToInt(remaining % 60f)}";
        passiveStateSlider.value = remaining / total;
    }
}
