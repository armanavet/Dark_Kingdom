using DG.Tweening;
//using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;
using AudioSystem;

public class UIManager : MonoBehaviour
{
    [SerializeField] public HealthBar MainTowerHB;
    [SerializeField] TextMeshProUGUI goldText, timerText, waveText;
    [SerializeField] GameObject activeStatePanel, passiveStatePanel, towerPurchasePanel;
    [SerializeField] private Slider activeStateSlider, passiveStateSlider;
    [SerializeField] Transform activeStateWarningParent;
    [SerializeField] Image activeStateWarningImage, activeStateWarningIcon;
    [SerializeField] TextMeshProUGUI activeStateWarningText;
    [SerializeField] private float activeStateEnableDuration = 3f;
    [SerializeField] private float activeStateUptime = 15.5f;
    [SerializeField] private Ease activeStateEnableEase = Ease.OutBack;
    [SerializeField] private float activeStateDisableDuration = 0.5f;
    [SerializeField] private Ease activeStateDisableEase = Ease.InBack;
    [Tooltip("How far down the panel moves to hide behind the screen.")]
    [SerializeField] GameObject[] effects;
    [SerializeField] LayerMask towerMask, tileMask;
    [SerializeField] float towerPurchasePanelYHidden;
    [SerializeField] float towerPanelYOffset;
    [SerializeField] SoundData UISoundData;
    [HideInInspector] public float GameTimer;

    GameState currentState;
    Camera mainCamera;
    GameObject activePanel, previousHit, effect, activeBar;
    TowerPreview towerPreview;
    Button[] towerPurchaseButtons;

    Dictionary<TowerType, float> towerOffset;

    float TowerPurchasePanelYInitial;

    bool isPanelActive = false;
    #region Singleton 
    private static UIManager _instance;
    public static UIManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<UIManager>();
            }

            return _instance;
        }
    }
    private void Awake()
    {
        _instance = this;
    }
    #endregion

    private void OnEnable()
    {
        StateManager.Instance.OnGameStateChanged += HandleStateChanged;
        StrategyManager.OnStrategyChanged += OnStrategyChanged;
        StateManager.Instance.OnGameStateChanged += HandleStateChanged;

    }

    private void OnDisable()
    {
        StrategyManager.OnStrategyChanged -= OnStrategyChanged;
        if (StateManager.Instance != null)
            StateManager.Instance.OnGameStateChanged -= HandleStateChanged;
    }

    public void Initialize()
    {
        TowerPurchasePanelYInitial = towerPurchasePanel.transform.position.y;
        towerPurchaseButtons = towerPurchasePanel.GetComponentsInChildren<Button>();

        activeStatePanel.SetActive(false);
        passiveStatePanel.SetActive(false);

        mainCamera = Camera.main;
        UISoundData = AudioManager.Instance.SetData(SoundDataType.UI);
    }

    void LateUpdate()
    {
        goldText.text = EconomyManager.Instance.CurrentCrystal.ToString();
    }

    void Update()
    {
        ChangeUiButtonVisibility();
        ShowTowerHealthBar();

        if (Input.GetMouseButtonDown(0))
        {
            if (IsClickOnTowerPanelUI()) { return; } // Check if the click was performed on the Tower UI panel
            if (EventSystem.current.IsPointerOverGameObject()) //Check if the click was performed on a UI element
            {
                ShowTowerPanel(false);
                return;
            }

            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            if (towerPreview != null) PlaceTower(true);
            else if (Physics.Raycast(ray, out RaycastHit towerHit, Mathf.Infinity, towerMask))
                ShowTowerPanel(true, towerHit.transform);
            else ShowTowerPanel(false);
        }
        else if (Input.GetMouseButton(1))
        {
            ShowTowerPanel(false);
            PlaceTower(false);

        }
    }
    void ChangeUiButtonVisibility()
    {
        for (int i = 0; i < towerPurchaseButtons.Length; i++)
        {
            var tower = TowerManager.Instance.TowerPrefabs[i];
            if (tower.PurchasePrice < EconomyManager.Instance.CurrentCrystal)
            {
                towerPurchaseButtons[i].interactable = true;
            }
            else
            {
                towerPurchaseButtons[i].interactable = false;
            }
        }
    }

    void ShowTowerHealthBar()
    {
        if (isPanelActive == true && activeBar != null)
        {
            activeBar.SetActive(false);
            return;
        }
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit towerHit, Mathf.Infinity, towerMask))
        {
            Tower tower = towerHit.transform.GetComponent<Tower>();
            if (tower.Type == TowerType.MainTower) return;

            GameObject healthBar = tower.HealthBar;
            if (healthBar == null) return;

            activeBar = healthBar;
            activeBar.SetActive(true);
        }
        else
        {
            if (activeBar != null) activeBar.SetActive(false);
        }
    }

    public void UpdateTimer(float remaining, float total)
    {
        if (currentState != GameState.Passive) return;

        timerText.text = $"{Mathf.FloorToInt(remaining / 60)}:" +
                         $"{Mathf.FloorToInt(remaining % 60f)}";
        passiveStateSlider.value = remaining / total;
    }

    public void UpdateEnemyCount(float remaining, float total)
    {
        if (currentState != GameState.Active) return;

        activeStateSlider.value = remaining / total;
    }

    void ShowTowerPanel(bool value, Transform selectedTower = null)
    {
        if (value == true)
        {
            Tower tower = selectedTower.GetComponent<Tower>();
            GameObject towerPanel = tower.TowerPanel;
            if (towerPanel == null) return;
            if (activePanel != null) activePanel.SetActive(false);

            activePanel = towerPanel;
            activePanel.SetActive(true);

            isPanelActive = true;
        }
        else
        {
            if (activePanel != null) activePanel.SetActive(false);
            isPanelActive = false;
        }
    }
    void ShowTowerPurchasePanel(bool value)
    {
        if (value == true)
        {
            DOTween.Kill("HidePanel");
            towerPurchasePanel.transform
                .DOMoveY(TowerPurchasePanelYInitial, 1)
                .SetId("ShowPanel")
                .SetEase(Ease.OutQuad);
            foreach (var button in towerPurchaseButtons)
            {
                button.interactable = true;
            }
        }
        else
        {
            DOTween.Kill("ShowPanel");
            towerPurchasePanel.transform
                .DOMoveY(towerPurchasePanelYHidden, 1)
                .SetId("HidePanel")
                .SetEase(Ease.OutQuad);
            foreach (var button in towerPurchaseButtons)
            {
                button.interactable = false;
            }
        }
    }
    public void PurchaseTower(int type)
    {
        if (towerPreview == null)
        {
            AudioManager.Instance.Play(UISFX_Type.TowerBuyButton, UISoundData);
            ShowTowerPurchasePanel(false);
            TowerPreview prefab = TowerManager.Instance.GetPreviewByType((TowerType)type);
            towerPreview = Instantiate(prefab);
        }
    }
    void PlaceTower(bool value)
    {
        if (value == true)
        {
            if (towerPreview == null) return;

            if (towerPreview.canPlace)
            {
                Tower tower = TowerManager.Instance.BuildTower(towerPreview.Type, towerPreview.tile);
                EconomyManager.Instance.ChangeCrystelAmount(-tower.PurchasePrice);
                Destroy(towerPreview.gameObject);
                ShowTowerPurchasePanel(true);
            }
            else
            {
                AudioManager.Instance.Play(UISFX_Type.PlacementDenied, UISoundData);
            }
        }
        else
        {
            if (towerPreview != null)
            {
                Destroy(towerPreview.gameObject);
                ShowTowerPurchasePanel(true);
            }
        }
    }
    public void OnGameStateChanged()
    {
        if (currentState == GameState.Passive)
        {
            activeStatePanel.SetActive(false);
            passiveStatePanel.SetActive(true);
        }
        else if (currentState == GameState.Active)
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
        else if (currentState == GameState.End)
        {
            activeStatePanel.SetActive(false);
            passiveStatePanel.SetActive(false);
        }
        else if (currentState == GameState.Paused) { }
        /*
         * if state == end 
         * activesState true
         * passive false
         * wave.text + currentwave
         * activ.text + "destroy the portal to finish the game"
         */
    }

    private void OnStrategyChanged(StrategyType newStrategy)
    {
        if (newStrategy == StrategyType.Construction)
        {
            ShowTowerPurchasePanel(true);
        }
        else
        {
            ShowTowerPurchasePanel(false);
            PlaceTower(false);
        }
    }

    public void ShowDamage(HitPointPopup damageTextPopup, Enemy target, float damage)
    {
        if (damageTextPopup == null || target == null || target.hitPointStartPos == null) return;
        Vector3 top = target.hitPointStartPos.transform.position;
        HitPointPopup HitPointPopup = Instantiate(damageTextPopup, top, Quaternion.identity);
        HitPointPopup.transform.rotation = LookAtCamera(mainCamera.transform);
        HitPointPopup.HitPointText(damage);
    }

    Quaternion LookAtCamera(Transform cameraTransform)
    {
        return Quaternion.LookRotation(cameraTransform.forward);
    }

    bool IsClickOnTowerPanelUI()
    {
        var eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        return results.Any(r => r.gameObject.CompareTag("TowerUIPanel"));
    }

    void HandleStateChanged(GameState state)
    {
        currentState = state;
    }
}