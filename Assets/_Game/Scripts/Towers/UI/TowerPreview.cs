using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerPreview : MonoBehaviour
{
    [SerializeField] GameObject canPlaceOutLine;
    [SerializeField] GameObject canNotPlaceOutLine;
    [SerializeField] LayerMask layerMask;
    [HideInInspector] public bool canPlace = false;
    [HideInInspector] public Tile tile;
    public TowerType Type;

    private void Start()
    {
        canPlaceOutLine.SetActive(false);
        canNotPlaceOutLine.SetActive(false);
    }

    private void Update()
    {
        TowerPreviewPlacement();
    }

    void TowerPreviewPlacement()
    {

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
        {
            transform.position = hit.transform.position;
            tile = hit.transform.GetComponent<Tile>();
            if ((tile.Type == TileType.Own || tile.Type == TileType.Claimed) && tile.isEmpty)
            {
                canPlace = true;
                canNotPlaceOutLine.SetActive(false);
                canPlaceOutLine.SetActive(true);
            }
            else
            {
                canPlace = false;
                canPlaceOutLine.SetActive(false);
                canNotPlaceOutLine.SetActive(true);
            }

        }
    }
}
