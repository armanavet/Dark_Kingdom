using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;
using UnityEngine.Rendering;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] public HealthBar MainTowerHB;
    [SerializeField] private TowerPurchasePanel towerPurchasePanel;

    [Header("World UI")]
    [SerializeField] private LayerMask towerMask;
    [SerializeField] private LayerMask tileMask;
    [SerializeField] private HitPointPopup unitHitPopUp;
    GameObject activePanel, activeBar;
    Ray ray;

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

    public void Initialize()
    {
        if (mainCamera == null) mainCamera = Camera.main;
        towerPurchasePanel.Initialize();
    }

    void Update()
    {
        if (StateManager.Instance.State == GameState.Paused) return;

        ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        bool raycastHit = Physics.Raycast(ray, out RaycastHit towerHit, Mathf.Infinity, towerMask);
        ShowTowerHealthBar(towerHit);

        if (Input.GetMouseButtonDown(0))
        {
            if (IsClickOnTowerPanelUI()) { return; } // Check if the click was performed on the Tower UI panel
            if (EventSystem.current.IsPointerOverGameObject()) //Check if the click was performed on a UI element
            {
                ShowTowerPanel(false);
                return;
            }


            if (towerPurchasePanel.IsPreviewing)
                towerPurchasePanel.TryPlaceTower();
            else if (raycastHit)
                ShowTowerPanel(true, towerHit.transform);
            else
                ShowTowerPanel(false);
        }
        else if (Input.GetMouseButton(1))
        {
            ShowTowerPanel(false);
            towerPurchasePanel.StopPreview();
            towerPurchasePanel.Show();
        }
    }

    void ShowTowerHealthBar(RaycastHit target)
    {
        if (isPanelActive || 
            target.transform == null || 
            !target.transform.TryGetComponent(out Tower tower) || 
            tower.Type == TowerType.MainTower)
        {
            if (activeBar != null) activeBar.SetActive(false);
            return;
        }

        GameObject healthBar = tower.HealthBar.gameObject;
        if (healthBar == null) return;

        if (activeBar != null && activeBar != healthBar)
        {
            activeBar.SetActive(false);
        }

        activeBar = healthBar;
        activeBar.SetActive(true);
    }

    public void UpdateEnemyCount(float remaining, float total)
    {
        //activeStateSlider.value = remaining / total;
    }

    void ShowTowerPanel(bool value, Transform selectedTower = null)
    {
        if (value == true)
        {
            Tower tower = selectedTower.GetComponent<Tower>();
            GameObject towerPanel = tower.TowerPanel.gameObject;
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

    public void ShowDamage(Enemy target, float damage)
    {
        if (target == null || target.hitPointStartPos == null) return;

        Vector3 top = target.hitPointStartPos.transform.position;
        HitPointPopup HitPointPopup = Instantiate(unitHitPopUp, top, Quaternion.identity);
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