using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Economics : MonoBehaviour
{
    [SerializeField] int CurrentGold;
    [SerializeField] List<Tower> towers;
    [SerializeField] TextMeshProUGUI GoldQuantity;
    List<Tower> EconomicBuilding = new List<Tower>();
    float timer = 0;
    #region 
    private static Economics _instance;
    public static Economics Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<Economics>();
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
        GoldQuantity.text = CurrentGold.ToString();
        foreach (var item in towers)
        {
            item.TowerButton = GameObject.FindGameObjectWithTag(item.ButtonTag).GetComponent<Button>();
        }
    }
    private void Update()
    {
        ChangeUiButtonVisibility();
        timer += Time.deltaTime;
        if(timer >= 1)
        {
            GenerateGold();
            timer = 0;
        }
    }
    void ChangeUiButtonVisibility()
    {

        foreach (var item in towers)
        {
            if (item.TowerButton == null) continue;
            if (item.TowerPrice > CurrentGold)
            {
                item.TowerButton.interactable = false;
            }
            else
            {
                item.TowerButton.interactable = true;
            }
        }
    }
    public void ChangeGoldAmount(int amount)
    {
        CurrentGold += amount;
        GoldQuantity.text = CurrentGold.ToString();
    }

    public void OnEconomicStructureChange(Tower structure)
    {
        if (structure.Type != TowerType.GoldMine && structure.Type != TowerType.MainTower) return;
        if (EconomicBuilding.Contains(structure))
        {
            EconomicBuilding.Remove(structure);
            return;
        }
        EconomicBuilding.Add(structure);
    }

    void GenerateGold()
    {
        foreach (var building in EconomicBuilding)
        {
            ChangeGoldAmount(building.GoldGenerated);
        }

       
    }
}




