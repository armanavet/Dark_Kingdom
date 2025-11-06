using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using static UnityEngine.GraphicsBuffer;

public class UIManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI goldText, timerText, waveText;
    [SerializeField] GameObject activeStatePanel, passiveStatePanel, towerPurchasePanel;
    [Tooltip("How far down the panel moves to hide behind the screen.")]
    [SerializeField] float towerPurchasePanelYHidden;
    [SerializeField] LayerMask towerMask, tileMask;
    [SerializeField] float towerPanelYOffset;
    [SerializeField] GameObject[] effects;
    [HideInInspector] public float GameTimer;
    GameObject activePanel, previousHit, effect;
    Button[] towerPurchaseButtons;
    TowerPreview towerPreview;
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
            
            else
            {
                ShowTowerPanel(false);
            }
        }
        else if (Input.GetMouseButton(1) || Input.GetKeyDown(KeyCode.Escape))
        {
            ShowTowerPanel(false);
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
        if (tower.CompareTag("ArcherTower"))
        {
            towerPanelYOffset = 2.6f;
            panel.transform.position = new Vector3(tower.transform.position.x, tower.transform.position.y + towerPanelYOffset, tower.transform.position.z);
        }
        else if (tower.CompareTag("WizardTower"))
        {
            towerPanelYOffset = 2.25f;
            panel.transform.position = new Vector3(tower.transform.position.x, tower.transform.position.y + towerPanelYOffset, tower.transform.position.z);
        }
        else if (tower.CompareTag("ArtilleryTower"))
        {
            towerPanelYOffset = 2.3f;
            panel.transform.position = new Vector3(tower.transform.position.x, tower.transform.position.y + towerPanelYOffset, tower.transform.position.z);
        }
        else if (tower.CompareTag("GoldMine"))
        {
            towerPanelYOffset = 1.3f;
            panel.transform.position = new Vector3(tower.transform.position.x, tower.transform.position.y + towerPanelYOffset, tower.transform.position.z);

        }
        else panel.transform.position = new Vector3(tower.transform.position.x, tower.transform.position.y + towerPanelYOffset, tower.transform.position.z);

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
                AudioManager.instance.PlayTowerPlaceSound();
                Tower tower = TowerManager.Instance.BuildTower(towerPreview.Type, towerPreview.tile);
                EconomyManager.Instance.ChangeGoldAmount(-tower.PurchasePrice);
                Destroy(towerPreview.gameObject);
                ShowTowerPurchasePanel(true);
                AudioManager.instance.PlayTowerPuffEffectSoundDelayed();
                effect = Instantiate(effects[1], new Vector3(tower.transform.position.x, 0.15f, tower.transform.position.z), Quaternion.Euler(-90f, tower.transform.rotation.y, tower.transform.rotation.z));
                Destroy(effect, 2f);
                effect = Instantiate(effects[0], new Vector3(tower.transform.position.x, 0.8f, tower.transform.position.z), Quaternion.identity);
                Destroy(effect, 2f);
            }
            else
            {
                AudioManager.instance.PlayTowerPlacementDeniedSound();
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