using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class TowerPlacement : MonoBehaviour
{
    [SerializeField] Transform towerParent;
    GameObject toPlacePrefab;
    Transform towerPreview;
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
            UIManager.Instance.ShowPanel();
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
                Economics.Instance.OnEconomicStructureChange(tower);
                Economics.Instance.ChangeGoldAmount(-tower.TowerPrice);
                toPlacePrefab = null;
                UIManager.Instance.ShowPanel();
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
            UIManager.Instance.HidePanel();
            towerPreview = Instantiate(previewObject).transform;
        }
    }
}
