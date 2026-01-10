
using UnityEngine;


[System.Serializable]
public class SaveMetaData
{
    public int PlayTime;
    public int CurrentCrystel;
    public int CurrentWave;

    public SaveMetaData(int playTime, int currentCrystel, int currentWave)
    {
        PlayTime = playTime;
        CurrentCrystel = currentCrystel;
        CurrentWave = currentWave;
    }
    
}
