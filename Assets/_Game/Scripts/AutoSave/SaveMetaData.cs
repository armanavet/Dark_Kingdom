
using UnityEngine;


[System.Serializable]
public class SaveMetaData
{
    public int PlayTime;
    public int CurrentGold;
    public int CurrentWave;

    public SaveMetaData(int playTime, int currentGold, int currentWave)
    {
        PlayTime = playTime;
        CurrentGold = currentGold;
        CurrentWave = currentWave;
    }
}
