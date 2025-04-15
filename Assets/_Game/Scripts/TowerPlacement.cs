using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
//using static UnityEngine.Rendering.DebugUI;
using UnityEngine.UI;
//using 

public class TowerPlacement : MonoBehaviour
{
    [SerializeField] Transform towerParent;
    [SerializeField] Transform towersUIPanel;
    [SerializeField] float PanelYHidden;
    float PanelYInitial;
    GameObject toPlacePrefab;
    Transform towerPreview;
    Button[] towerButtons;

    private void Start()
    {
        PanelYInitial = towersUIPanel.transform.position.y;
        towerButtons = towersUIPanel.GetComponentsInChildren<Button>();
        
        
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            
            PlaceTower();
        }
        if (towerPreview != null && Input.GetMouseButtonDown(1))
        {
            Destroy(towerPreview.gameObject);
            toPlacePrefab = null;
            ShowPanel();
        }
    }
    void PlaceTower()
    {

        if (towerPreview != null)
        {
            TowerPreview previewInstance = towerPreview.GetComponent<TowerPreview>();
            if (previewInstance.canPlace)
            {

                previewInstance.tile.isEmpty = false;
                Destroy(previewInstance.gameObject);
                Tower tower = Instantiate(toPlacePrefab,towerPreview.position,Quaternion.identity,towerParent).GetComponent<Tower>();
                Economics.Instance.OnEconomicStructureChange(tower, tower.GoldGenerated);
                Economics.Instance.ChangeGoldAmount(-tower.TowerPrice);
                toPlacePrefab = null;
                ShowPanel();
            }
        }
    }
    public void ButtonGetPrefabToPlace(GameObject toPlaceObject) 
    {
        if (toPlacePrefab == null)
        {
            toPlacePrefab = toPlaceObject;
        }
    }
    public void ButtonStartPreview(GameObject previewObject)
    {
        
        if (towerPreview == null)
        {
            HidePanel();
            towerPreview = Instantiate(previewObject).transform;
           
        }

    }
    void HidePanel()
    {
        DOTween.Kill("ShowPanel");
        towersUIPanel.DOMoveY(PanelYHidden, 1).SetId("HidePanel").SetEase(Ease.OutQuad); 
        foreach (var button in towerButtons)
        {
            button.interactable = false;
        }
    }
    void ShowPanel()
    {
        DOTween.Kill("HidePanel");
        towersUIPanel.DOMoveY(PanelYInitial, 1).SetId("ShowPanel").SetEase(Ease.OutQuad);
        foreach (var button in towerButtons)
        {
            button.interactable = true;
        }
    }
}
