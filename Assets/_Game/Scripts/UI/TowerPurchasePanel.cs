using AudioSystem;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TowerPurchasePanel : MonoBehaviour
{
    [SerializeField] private Button[] purchaseButtons;
    [SerializeField] private TextMeshProUGUI[] priceTexts;
    [SerializeField] private GameObject descriptionPopup;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [Tooltip("How far down the panel moves to hide behind the screen.")]
    [SerializeField] private float yPosHidden;
    [HideInInspector] public bool IsPreviewing;
    private TowerPreview towerPreview;
    private SoundData soundData;
    private float yPosInitial;


    private void OnEnable()
    {
        StrategyManager.OnStrategyChanged += OnStrategyChanged;
    }

    private void OnDisable()
    {
        StrategyManager.OnStrategyChanged -= OnStrategyChanged;
    }

    public void Initialize()
    {
        yPosInitial = transform.position.y;
        for (int i = 0; i < purchaseButtons.Length; i++)
        {
            priceTexts[i].text = TowerManager.Instance.GetPrefabByType((TowerType)(i + 1)).PurchasePrice.ToString();
        }

        soundData = AudioManager.Instance.SetData(SoundDataType.UI);
        descriptionPopup.SetActive(false);
    }

    private void Update()
    {
        ToggleButtonStates();

        if (descriptionPopup.activeSelf)
        {
            descriptionPopup.transform.position = Input.mousePosition;
        }
    }

    private void ToggleButtonStates()
    {
        for (int i = 0; i < purchaseButtons.Length; i++)
        {
            var tower = TowerManager.Instance.TowerPrefabs[i];
            if (tower.PurchasePrice < EconomyManager.Instance.CurrentCrystals)
            {
                purchaseButtons[i].interactable = true;
            }
            else
            {
                purchaseButtons[i].interactable = false;
            }
        }
    }

    public void Show()
    {
        transform.DOKill();
        transform
            .DOMoveY(yPosInitial, 1)
            .SetId("ShowPanel")
            .SetEase(Ease.OutQuad);

        foreach (var button in purchaseButtons)
        {
            button.interactable = true;
        }
    }

    public void Hide()
    {
        transform.DOKill();
        transform
            .DOMoveY(yPosHidden, 1)
            .SetId("HidePanel")
            .SetEase(Ease.OutQuad);

        foreach (var button in purchaseButtons)
        {
            button.interactable = false;
        }
    }

    public void PurchaseTower(int type)
    {
        if (towerPreview != null) return;

        Hide();
        AudioManager.Instance.Play(UISFX_Type.TowerBuyButton, soundData);
        TowerPreview prefab = TowerManager.Instance.GetPreviewByType((TowerType)type);
        towerPreview = Instantiate(prefab);
        IsPreviewing = true;
    }

    public void TryPlaceTower()
    {
        if (towerPreview == null) return;

        if (towerPreview.canPlace)
        {
            Tower tower = TowerManager.Instance.BuildTower(towerPreview.Type, towerPreview.tile);
            StopPreview();
            Show();
        }
        else
        {
            AudioManager.Instance.Play(UISFX_Type.PlacementDenied, soundData);
        }
    }

    public void StopPreview()
    {
        if (towerPreview == null) return;

        Destroy(towerPreview.gameObject);
        IsPreviewing = false;
    }

    public void ShowDescription(int type)
    {
        descriptionPopup.SetActive(true);
        string description = TowerManager.Instance.TowerDescriptions.GetByType((TowerType)type);
        descriptionText.text = description;
    }

    public void HideDescription()
    {
        descriptionPopup.SetActive(false);
    }

    private void OnStrategyChanged(Strategy newStrategy)
    {
        if (newStrategy.Type == StrategyType.Construction)
        {
            Show();
        }
        else
        {
            StopPreview();
            Hide();
        }
    }
}
