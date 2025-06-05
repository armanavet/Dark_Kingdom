using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISaveable
{
    public string GetUniqueSaveID();
    public ISaveData SaveState();

    public void LoadState(ISaveData data);
}
