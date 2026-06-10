
public class GoldMineTower : Tower
{
    private void Update()
    {
        CheckStrategy();
    }

    protected override void CheckStrategy()
    {
        foreach (var effect in sleepFX)
        {
            if (StrategyManager.Instance.CurrentStrategy == StrategyType.Economy)
            {
                effect.SetActive(false);
            }
            else
            {
                effect.SetActive(true);
            }
        }
    }
}
