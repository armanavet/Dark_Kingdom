using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.Rendering.DebugUI;
//using 

public class TowerPlacement : MonoBehaviour
{
    [SerializeField] Transform towerParent;
    [SerializeField] Transform towersUIPanel;
    [SerializeField] float PanelYHidden;
    float PanelYInitial;
    GameObject toPlacePrefab;
    Transform towerPreview;

    private void Start()
    {
        PanelYInitial = towersUIPanel.transform.position.y;
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            PlaceTower();
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
                    Instantiate(toPlacePrefab,towerPreview.position,Quaternion.identity,towerParent);
                    ShowPanel();
                
            }
            if (Input.GetKeyDown("escape"))
            {
                Destroy(previewInstance.gameObject);
                ShowPanel();

            }
        }

    }
    public void ButtonGetPrefabToPlace(GameObject toPlaceObject) 
    {
        toPlacePrefab = toPlaceObject;
    }
    public void ButtonStartPreview(GameObject previewObject)
    {
        HidePanel();
        towerPreview = Instantiate(previewObject).transform;

    }
    void HidePanel()
    {
        DOTween.Kill("ShowPanel");
        towersUIPanel.DOMoveY(PanelYHidden, 1).SetId("HidePanel").SetEase(Ease.OutQuad);
    }
    void ShowPanel()
    {
        DOTween.Kill("HidePanel");
        towersUIPanel.DOMoveY(PanelYInitial, 1).SetId("ShowPanel").SetEase(Ease.OutQuad);
    }
}
