using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Economics : MonoBehaviour
{
    [SerializeField] int CurrentGold;
    [SerializeField] List<Tower> towers;
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
        foreach (var item in towers)
        {
            item.TowerButton = GameObject.FindGameObjectWithTag(item.ButtonTag).GetComponent<Button>();
        }
    }
    private void Update()
    {
        foreach (var item in towers)
        {
            if (item.TowerPrice > CurrentGold) 
            {
                item.TowerButton.interactable = false;
            }else
            {
                item.TowerButton.interactable = true;
            }
        }
    }

    public void ChangeGoldAmount(int amount)
    {
        CurrentGold += amount;
    }


}




