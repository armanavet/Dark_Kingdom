using DG.Tweening;
//using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;
using AudioSystem;

public class UIManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI goldText, timerText, waveText, activeStateText;
    [SerializeField] GameObject activeStatePanel, passiveStatePanel, towerPurchasePanel;
    [Tooltip("How far down the panel moves to hide behind the screen.")]
    [SerializeField] GameObject[] effects;
    [SerializeField] LayerMask towerMask, tileMask;
    [SerializeField] float towerPurchasePanelYHidden;
    [SerializeField] float towerPanelYOffset;
    [SerializeField] SoundData UISoundData;
    [HideInInspector] public float GameTimer;

    Camera mainCamera;
    GameObject activePanel, previousHit, effect;
    TowerPreview towerPreview;
    Button[] towerPurchaseButtons;
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
                _instance = GameObject.FindObjectOfType<UIManager>();
            }

            return _instance;
        }
    }
    private void Awake()
    {
        _instance = this;
    }
    #endregion
    public void Initialize()
    {
        TowerPurchasePanelYInitial = towerPurchasePanel.transform.position.y;
        towerPurchaseButtons = towerPurchasePanel.GetComponentsInChildren<Button>();

        activeStatePanel.SetActive(false);
        passiveStatePanel.SetActive(false);

        mainCamera = Camera.main;

        //UISoundData = AudioManager.Instance.SetData(UISoundData, SoundDataType.UI, MixerType.UI);
    }
    void LateUpdate()
    {
        goldText.text = EconomyManager.Instance.CurrentGold.ToString();
        timerText.text = Mathf.Round(GameTimer).ToString();

    }
    void Update()
    {
        ChangeUiButtonVisibility();

        if (Input.GetMouseButtonDown(0))
        {
            if (IsClickOnTowerPanelUI()) return; // Check if the click was performed on the Tower UI panel
            if (EventSystem.current.IsPointerOverGameObject()) //Check if the click was performed on a UI element
            {
                ShowTowerPanel(false);
                return;
            }

            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            if (towerPreview != null) PlaceTower(true);
            else if (Physics.Raycast(ray, out RaycastHit towerHit, Mathf.Infinity, towerMask)) ShowTowerPanel(true, towerHit.transform);
            else ShowTowerPanel(false);
        }
        else if (Input.GetMouseButton(1))
        {
            ShowTowerPanel(false);
            PlaceTower(false);

        }

        if (isPanelActive && activePanel != null)
        {
            activePanel.transform.rotation = LookAtCamera(mainCamera.transform);
        }
    }
    void ChangeUiButtonVisibility()
    {
        for (int i = 0; i < towerPurchaseButtons.Length; i++)
        {
            var tower = TowerManager.Instance.TowerPrefabs[i];
            if (tower.PurchasePrice < EconomyManager.Instance.CurrentGold)
            {
                towerPurchaseButtons[i].interactable = true;
            }
            else
            {
                towerPurchaseButtons[i].interactable = false;
            }
        }
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
            SetTowerPanelPosition(activePanel, tower);
            activePanel.SetActive(true);

            isPanelActive = true;
        }
        else
        {
            if (activePanel != null) activePanel.SetActive(false);
            isPanelActive = false;
        }
    }
    void SetTowerPanelPosition(GameObject panel, Tower tower)
    {
        Transform panelPos = panel.transform;
        float towerPositionX = tower.transform.position.x;
        float towerPositionY = tower.transform.position.y;
        float towerPositionZ = tower.transform.position.z;
        float yOffset = towerPanelYOffset;

        if (tower.CompareTag("ArcherTower")) yOffset = 2.6f;
        else if (tower.CompareTag("WizardTower")) yOffset = 2.6f;
        else if (tower.CompareTag("ArtilleryTower")) yOffset = 2.6f;
        else if (tower.CompareTag("GoldMine")) yOffset = 2.6f;

        SetPosition(panelPos, yOffset, towerPositionX, towerPositionY, towerPositionZ);

    }
    void SetPosition(Transform positionToPlace, float yOffsetNumber, float x, float y, float z)
    {
        positionToPlace.position = new Vector3(x, y + yOffsetNumber, z);
    }
    void ShowTowerPurchasePanel(bool value)
    {
        if (value == true)
        {
            DOTween.Kill("HidePanel");
            towerPurchasePanel.transform.DOMoveY(TowerPurchasePanelYInitial, 1).SetId("ShowPanel").SetEase(Ease.OutQuad);
            foreach (var button in towerPurchaseButtons)
            {
                button.interactable = true;
            }
        }
        else
        {
            DOTween.Kill("ShowPanel");
            towerPurchasePanel.transform.DOMoveY(towerPurchasePanelYHidden, 1).SetId("HidePanel").SetEase(Ease.OutQuad);
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
            //AudioManager.Instance.Play(UISoundData,ClipType.OnPurchaseButtonClick_UI);
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
                //AudioManager.Instance.Play(UISoundData, transform, ClipType.OnDenied_Tower);
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
    public void OnGameStateChanged(GameState newState, int currentWave)
    {
        if (newState == GameState.Passive)
        {
            activeStatePanel.SetActive(false);
            passiveStatePanel.SetActive(true);
        }
        else if (newState == GameState.Active)
        {
            activeStatePanel.SetActive(true);
            passiveStatePanel.SetActive(false);
            waveText.text = "Wave: " + currentWave.ToString();
            if (currentWave == WaveManager.Instance.waveLength)
            {
                activeStateText.text = string.Empty;
                activeStateText.text = "Destroy The Portal!";
                waveText.text = "Wave: " + currentWave.ToString();
            }
        }
        else if (newState == GameState.End)
        {
            activeStatePanel.SetActive(false);
            passiveStatePanel.SetActive(false);
        }
        else if (newState == GameState.Paused) { }
        /*
         * if state == end 
         * activesState true
         * passive false
         * wave.text + currentwave
         * activ.text + "destroy the portal to finish the game"
         */
    }
    public void ShowDamage(HitPointPopup damageTextPopup, Enemy target, float damage)
    {
        if (damageTextPopup == null || target == null) return;
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

}