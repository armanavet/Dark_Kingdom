using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIManager : MonoBehaviour
{

    [SerializeField] TextMeshProUGUI goldText, timerText, waveText;
    [SerializeField] GameObject activeStatePanel, passiveStatePanel, towerPurchasePanel, tilePanelPrefab;
    [SerializeField] float tilePanelYOffset;
    [SerializeField] float towerPurchasePanelYHidden;
    [SerializeField] LayerMask towerMask, tileMask;
    GameObject tilePanel, activePanel, previousHit;
    Button[] towerPurchaseButtons;
    TowerPreview towerPreview;
    UIAction currentAction;
    float TowerPurchasePanelYInitial;
    bool isPanelActive = false;
    [HideInInspector] public float GameTimer;

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


    private void Start()
    {
        TowerPurchasePanelYInitial = towerPurchasePanel.transform.position.y;
        towerPurchaseButtons = towerPurchasePanel.GetComponentsInChildren<Button>();
        tilePanel = Instantiate(tilePanelPrefab);
        tilePanel.SetActive(false);
        activeStatePanel.SetActive(false);
        passiveStatePanel.SetActive(false);
    }
    private void Update()
    {
        ChangeUiButtonVisibility();
        goldText.text = EconomyManager.Instance.CurrentGold.ToString();
        timerText.text = Mathf.Round(GameTimer).ToString();


        if (Input.GetMouseButtonDown(0))
        {
            switch (currentAction)
            {
                case UIAction.PurchaseTower:
                    PlaceTower();
                    break;
                default:
                    if (EventSystem.current.IsPointerOverGameObject()) break; //Check if the click was performed on a UI element
                    ShowTowerPanel(true);
                    ShowTilePanel(true);
                    break;
            }

        }

        else if (Input.GetMouseButton(1))
        {
            switch (currentAction)
            {
                case UIAction.PurchaseTile:
                    ShowTilePanel(false);
                    break;
                case UIAction.PurchaseTower:
                    if (towerPreview != null)
                    {
                        Destroy(towerPreview.gameObject);
                        ShowTowerPurchasePanel(true);
                    }
                    break;
                case UIAction.UpgradeTower:
                    ShowTowerPanel(false);
                    break;
                default:
                    break;
            }
            currentAction = UIAction.None;
        }
        if (isPanelActive && activePanel != null)
        {
            Vector3 direction = activePanel.transform.position - Camera.main.transform.position;
            Quaternion rotation = Quaternion.LookRotation(direction * 0.5f * Time.deltaTime);
            activePanel.transform.rotation = rotation;
        }

    }
    void ChangeUiButtonVisibility()
    {
        foreach (var tower in TowerManager.Instance.TowerPrefabs)
        {
            if (tower.PurchasePrice < EconomyManager.Instance.CurrentGold)
            {
                towerPurchaseButtons[(int)tower.Type].interactable = true;
            }
            else
            {
                towerPurchaseButtons[(int)tower.Type].interactable = false;
            }
        }
    }

    void ShowTowerPanel(bool value)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, towerMask))
        {
            GameObject towerPanel = hit.transform.GetComponent<Tower>().TowerPanel;
            Transform panel = towerPanel.transform.Find("SellUI");
            if (towerPanel == null)
            {
                return;
            }
            if (previousHit == null)
            {
                previousHit = towerPanel;
            }
            previousHit.SetActive(false);
            towerPanel.SetActive(true);
            activePanel = panel.gameObject;
            isPanelActive = true;
            previousHit = towerPanel;
        }
        else
        {
            if (previousHit != null)
            {
                previousHit.gameObject.SetActive(false);
                isPanelActive = false;
            }
        }
    }

    void ShowTilePanel(bool value)
    {
        if (tilePanel == null) return;

        if (value == true)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, tileMask))
            {
                Tile tile_ = hit.transform.GetComponent<Tile>();
                bool selectedTile = tile_.CanBePurchased();
                if (selectedTile == true)
                {
                    tilePanel.SetActive(true);
                    tilePanel.GetComponent<TileBuy>().tile = tile_;
                    tilePanel.transform.position = new Vector3(tile_.transform.position.x, tile_.transform.position.y + tilePanelYOffset, tile_.transform.position.z);
                    return;
                }
            }
        }

        tilePanel.SetActive(false);
    }

    public void ShowTowerPurchasePanel(bool value)
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
            currentAction = UIAction.PurchaseTower;
            ShowTowerPurchasePanel(false);
            TowerPreview prefab = TowerManager.Instance.GetPreviewByType((TowerType)type);
            towerPreview = Instantiate(prefab);
        }
    }

    void PlaceTower()
    {
        if (towerPreview == null) return;

        if (towerPreview.canPlace)
        {
            Tower tower = TowerManager.Instance.BuildTower(towerPreview.Type, towerPreview.tile);
            Destroy(towerPreview.gameObject);

            EconomyManager.Instance.ChangeGoldAmount(-tower.PurchasePrice);
            ShowTowerPurchasePanel(true);
            currentAction = UIAction.None;
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
        }
    }
}

public enum UIAction
{
    None,
    PurchaseTile,
    PurchaseTower,
    UpgradeTower
}
