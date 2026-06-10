using AudioSystem;
using UnityEngine;
using UnityEngine.UI;

public class TowerPanel : MonoBehaviour
{
    [SerializeField] Button UpgradeButton;
    [SerializeField] Button SellButton;
    public Tower Tower;
    private SoundData soundData;

    private void Start()
    {
        soundData = AudioManager.Instance.SetData(Tower.Type, SoundDataType.Gameplay);
    }

    private void Update()
    {
        ChangeButtonVisibility();
    }

    void ChangeButtonVisibility()
    {
        if (EconomyManager.Instance.CurrentCrystals < Tower.UpgradePrice ||
            Tower.IsMaxLevel || 
            StrategyManager.Instance.CurrentStrategy != StrategyType.Construction)
        {
            UpgradeButton.interactable = false;
        }
        else
        {
            UpgradeButton.interactable = true;
        }

        if (Tower.Type != TowerType.MainTower && StrategyManager.Instance.CurrentStrategy != StrategyType.Construction)
        {
            SellButton.interactable = false;
        }
        else if (Tower.Type != TowerType.MainTower)
        {
            SellButton.interactable = true;
        }
    }

    public void ButtonTowerSell()
    {
        if (StrategyManager.Instance.CurrentStrategy != StrategyType.Construction) return;

        AudioManager.Instance.Play(UISFX_Type.TowerSellButton, soundData);
        Tower.Sell();
    }

    public void ButtonTowerUpgrade()
    {
        if (StrategyManager.Instance.CurrentStrategy != StrategyType.Construction) return;

        AudioManager.Instance.Play(UISFX_Type.TowerUpgradeButton, soundData);
        Tower.Upgrade();
    }
}
