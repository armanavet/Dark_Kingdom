using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    [SerializeField] float PanelYHidden;
    [SerializeField] Transform towersUIPanel;
    [SerializeField] Tower[] towers;
    [SerializeField] TextMeshProUGUI GoldQuantity;
    [SerializeField] LayerMask layerMask;
    float PanelYInitial;
    Button[] towerButtons;
    Transform previewsHit;

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

    }
    private void Update()
    {
        ChangeUiButtonVisibility();
        GoldQuantity.text = Economics.Instance.CurrentGold.ToString();
        if (Input.GetMouseButtonDown(0))
        {
            TowerPanelShowAndHide();
        }
    }
    void TowerPanelShowAndHide()
    {
        
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
       
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask))
        {
            Debug.Log("mta");
            if (previewsHit == null)
            {
                Debug.Log("mta null");
                previewsHit = hit.transform;
            } else if(previewsHit.GetComponentInChildren<Canvas>() != null )
            {
                Debug.Log("mta canvas voch null ");
                previewsHit.GetComponentInChildren<Canvas>().gameObject.SetActive(false); 
            }
            if (hit.transform.GetComponentInChildren<Canvas>())
            {
                Debug.Log("mta canvas true");
                hit.transform.GetComponentInChildren<Canvas>().gameObject.SetActive(true);
            }
            
            
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
