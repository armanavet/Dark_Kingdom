using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SaveManager
{
    static int saveSlot;
    static List<ISaveable> objectsToSave = new List<ISaveable>();
    static SaveData saveData = new SaveData();

    static string folderPath = Application.persistentDataPath + "/Saves";
    static string fileExtension = ".save";
    static string fileName => "save" + saveSlot + fileExtension;
    static string filePath => Path.Combine(folderPath, fileName);
    public static void SetSlot(int slot) => saveSlot = slot;

    public static void RegisterSaveable(ISaveable saveable)
    {
        if (!objectsToSave.Contains(saveable)) 
            objectsToSave.Add(saveable);
    }

    public static void Save()
    {
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        foreach (var obj in objectsToSave)
        {
            saveData.Add(obj.GetUniqueSaveID(), obj.SaveState());
        }

        BinaryFormatter formatter = new BinaryFormatter();
        using (FileStream fs = new FileStream(filePath, FileMode.Create))
        {
            formatter.Serialize(fs, saveData);
        }
    }

    public static void SaveMetaData()
    {
        string metaFileName = "save" + saveSlot + "_meta" + ".json";
        string metaFilePath = Path.Combine(folderPath, metaFileName);

        SaveMetaData metaData = new SaveMetaData();
        using (FileStream fs = new FileStream(metaFilePath, FileMode.Create))
        {
            //json serialize
            //write to file
        }
    }

    public static void Load()
    {
        if (!File.Exists(filePath)) return;

        try
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream fs = new FileStream(filePath, FileMode.Open))
            {
                saveData = formatter.Deserialize(fs) as SaveData;
                foreach (var saveable in objectsToSave)
                {
                    string id = saveable.GetUniqueSaveID();
                    saveable.LoadState(saveData.Get(id));
                }
            }
        }
        catch
        {
            return;
        }
    }

    public static void OnGameStart()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    
    public static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        Load();
    }
}
