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
    Dictionary<Tower, int> EconomicBuilding = new Dictionary<Tower, int>();
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
        CurrentGold = 0;
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

    public void OnEconomicStructureChange(Tower structure, int gold)
    {
        Debug.Log("mta OnEconomicStructureChange");
        if (structure.Type != TowerType.GoldMine && structure.Type != TowerType.MainTower) return;
        if (EconomicBuilding.ContainsKey(structure))
        {
            Debug.Log("chack if");
            EconomicBuilding.Remove(structure);
            return;
        }
        Debug.Log("if anca");
        
        EconomicBuilding.Add(structure, gold);
    }

    public void OnEconomicStructureUpgarde(Tower structure, int gold)
    {
        if (!EconomicBuilding.ContainsKey(structure)) return;

        EconomicBuilding[structure] = gold;
        

    }

    void GenerateGold()
    {
        foreach (var item in EconomicBuilding.Keys)
        {
            ChangeGoldAmount(EconomicBuilding[item]);
        }

       
    }
}




