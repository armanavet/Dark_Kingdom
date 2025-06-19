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
    [SerializeField] float towerPurchasePanelYHidden;
    [SerializeField] LayerMask towerMask, tileMask;
    [SerializeField] float tilePanelYOffset;
    GameObject tilePanel, activePanel, previousHit;
    Button[] towerPurchaseButtons;
    TowerPreview towerPreview;
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


    void Start()
    {
        TowerPurchasePanelYInitial = towerPurchasePanel.transform.position.y;
        towerPurchaseButtons = towerPurchasePanel.GetComponentsInChildren<Button>();
        tilePanel = Instantiate(tilePanelPrefab);
        tilePanel.SetActive(false);
        activeStatePanel.SetActive(false);
        passiveStatePanel.SetActive(false);
    }
    void Update()
    {
        ChangeUiButtonVisibility();
        goldText.text = EconomyManager.Instance.CurrentGold.ToString();
        timerText.text = Mathf.Round(GameTimer).ToString();


        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject()) return; //Check if the click was performed on a UI element

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (towerPreview != null) PlaceTower(true);
            else if (Physics.Raycast(ray, out RaycastHit towerHit, Mathf.Infinity, towerMask)) ShowTowerPanel(true, towerHit.transform); 
            else if (Physics.Raycast(ray, out RaycastHit tileHit, Mathf.Infinity, tileMask)) ShowTilePanel(true, tileHit.transform);
            else
            {
                ShowTowerPanel(false);
                ShowTilePanel(false);
            }
        }
        else if (Input.GetMouseButton(1) || Input.GetKeyDown(KeyCode.Escape))
        {
            ShowTowerPanel(false);
            ShowTilePanel(false);
            PlaceTower(false);
        }
        if (isPanelActive && activePanel != null)
        {
            Quaternion rotation = Quaternion.LookRotation(Camera.main.transform.forward);
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

    void ShowTowerPanel(bool value, Transform tower = null)
    {
        if (value == true)
        {
            GameObject towerPanel = tower.GetComponent<Tower>().TowerPanel;
            if (towerPanel == null) return;
            if(activePanel != null) activePanel.SetActive(false);
            
            activePanel = towerPanel;
            towerPanel.SetActive(true);
            
            isPanelActive = true;
        }
        else
        {
            if (activePanel != null) activePanel.SetActive(false);
            isPanelActive = false;
        }
    }

    void ShowTilePanel(bool value, Transform selectedTile = null)
    {
        if (tilePanel == null) return;

        if (value == true)
        {
            Tile tile = selectedTile.GetComponent<Tile>();
            if (tile.CanBePurchased())
            {
                if (activePanel != null) activePanel.SetActive(false);
                activePanel = tilePanel;
                isPanelActive = true;

                tilePanel.SetActive(true);
                tilePanel.GetComponent<TileBuy>().tile = tile;
                tilePanel.transform.position = tile.transform.position + new Vector3(0,tilePanelYOffset,0);
                return;
            }
        }

        if (activePanel != null)  activePanel.SetActive(false);
        isPanelActive = false;
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
                EconomyManager.Instance.ChangeGoldAmount(-tower.PurchasePrice);

                Destroy(towerPreview.gameObject);
                ShowTowerPurchasePanel(true);
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
        }
    }
}