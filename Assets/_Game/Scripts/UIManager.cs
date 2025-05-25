using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
//using static UnityEngine.Rendering.DebugUI;

public class UIManager : MonoBehaviour
{

    [SerializeField] float PanelYHidden;
    [SerializeField] Transform towersUIPanel;
    [SerializeField] Tower[] towers;
    [SerializeField] TextMeshProUGUI GoldQuantity;
    [SerializeField] LayerMask towerLayerMask;
    [SerializeField] LayerMask tileLayerMask;
    [SerializeField] GameObject tilePanelPrefab;
    [SerializeField] float yCoord;
    float PanelYInitial;
    Button[] towerButtons;
    GameObject previewsHit,tilePanel;
    Transform activePanel;
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


    private void Start()
    {
        PanelYInitial = towersUIPanel.transform.position.y;
        towerButtons = towersUIPanel.GetComponentsInChildren<Button>();
        tilePanel = Instantiate(tilePanelPrefab);
        tilePanel.SetActive(false);
    }
    private void Update()
    {
        ChangeUiButtonVisibility();
        GoldQuantity.text = Economics.Instance.CurrentGold.ToString();
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }
            else
            {
                TowerPanelShowAndHide();
                TilePanel();
            }
        }
        if (isPanelActive && activePanel != null)
        {
            Vector3 direction = activePanel.transform.position - Camera.main.transform.position;
            Quaternion rotation = Quaternion.LookRotation(direction * 0.5f* Time.deltaTime);
            activePanel.transform.rotation = rotation;
        }
        
    }
    void TowerPanelShowAndHide()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, towerLayerMask))
        {
            GameObject towerPanel = hit.transform.GetComponent<Tower>().TowerPanel;
            Transform panel = towerPanel.transform.Find("SellUI");
            if (towerPanel == null) {
                return;
            }
            if (previewsHit == null)
            {
                previewsHit = towerPanel;
            }
            previewsHit.SetActive(false);
            towerPanel.SetActive(true);
            activePanel = panel;
            isPanelActive = true;
            previewsHit = towerPanel;

        } 
        else
        {
            if (previewsHit != null)
            {
                previewsHit.gameObject.SetActive(false);
                isPanelActive = false;
            }
        }
    }

    void TilePanel()
    {
        if (tilePanel == null)
        {
            return;
        }
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, tileLayerMask))
        {
            Tile tile_ = hit.transform.GetComponent<Tile>();
            bool selectedTile = tile_.MakeObstructedYellow(GameBoard.Instance.tiles, GameBoard.Instance.Width, GameBoard.Instance.Length);
            if(selectedTile == true)
            {
                tilePanel.SetActive(true);
                tilePanel.GetComponent<TileBuy>().tile = tile_;
                tilePanel.transform.position = new Vector3(tile_.transform.position.x, tile_.transform.position.y+yCoord, tile_.transform.position.z);
            }
            else
            {
                tilePanel.SetActive(false);
            }
        }
        else
        {
            tilePanel.SetActive(false);
        }
    }
    void ChangeUiButtonVisibility()
    {
        for (int i = 0; i < towers.Length; i++)
        {
            if (towers[i].TowerPrice > Economics.Instance.CurrentGold)
            {
                towerButtons[i].interactable = false;
            }
            else
            {
                towerButtons[i].interactable = true;
            }
        }
    }
    public void HidePanel()
    {
        DOTween.Kill("ShowPanel");
        towersUIPanel.DOMoveY(PanelYHidden, 1).SetId("HidePanel").SetEase(Ease.OutQuad);
        foreach (var button in towerButtons)
        {
            button.interactable = false;
        }
    }
    public void ShowPanel()
    {
        DOTween.Kill("HidePanel");
        towersUIPanel.DOMoveY(PanelYInitial, 1).SetId("ShowPanel").SetEase(Ease.OutQuad);
        foreach (var button in towerButtons)
        {
            button.interactable = true;
        }
    }


}
