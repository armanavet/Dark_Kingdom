using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerPlacement : MonoBehaviour
{
    [SerializeField] GameObject previewPrefab;
    [SerializeField] GameObject prefabToPlace;
    GameObject canPlaceOutLine;
    GameObject canNotPlaceOutLine;
    [SerializeField] LayerMask layerMask;
    Transform towerParent;
    Transform towerPreview;
    Tile tile;
    bool isPreview;


    private void Update()
    {

        if(isPreview)
            TowerPreviewPlacement();
    }


    void TowerPreviewPlacement()
    {
        
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
        {
            towerPreview.position = hit.transform.position;
            tile = hit.transform.GetComponent<Tile>();
            Debug.Log(tile.Type);
            if (tile.Type == GameTileContentType.Own && tile.isEmpty)
            {
                canNotPlaceOutLine.SetActive(false);
                canPlaceOutLine.SetActive(true);
            }
            else
            {
                canPlaceOutLine.SetActive(false);
                canNotPlaceOutLine.SetActive(true);
            }

        }
    }
    public void ButtonStartPreview()
    {
        isPreview = true;
        towerPreview = Instantiate(previewPrefab, towerParent).transform;
        Transform[] children = towerPreview.GetComponentsInChildren<Transform>(true); // 'true' includes inactive

        foreach (Transform child in children)
        {
            if (child.CompareTag("CanPlace"))
            {
                canPlaceOutLine = child.gameObject;

            }
            if (child.CompareTag("CanNotPlace"))
            {
                canNotPlaceOutLine = child.gameObject;
            }
        }
        canPlaceOutLine.SetActive(false);
        canNotPlaceOutLine.SetActive(false);

    }

}
