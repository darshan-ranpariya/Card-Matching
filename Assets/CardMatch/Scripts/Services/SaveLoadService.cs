using UnityEngine;

public class SaveLoadService : ISaveLoadService
{
    private readonly string stageDataPath;
    private readonly string saveKey;

    public SaveLoadService(string stageDataPath, string saveKey)
    {
        this.stageDataPath = stageDataPath;
        this.saveKey = saveKey;
    }

    public bool HasSavedData()
    {
        return PlayerPrefs.GetInt(saveKey, 0) != 0;
    }

    public StageData LoadStageData()
    {
        Debug.Log("Loading saved data");
        string json = System.IO.File.ReadAllText(stageDataPath);
        StageData stageData = JsonUtility.FromJson<StageData>(json);
        PlayerPrefs.SetInt(saveKey, 0);
        return stageData;
    }

    public void SaveStageData(StageData stageData)
    {
        Debug.Log("Saving data");
        PlayerPrefs.SetInt(saveKey, 1);
        string json = JsonUtility.ToJson(stageData);
        System.IO.File.WriteAllText(stageDataPath, json);
    }

    public void ClearSaveData()
    {
        PlayerPrefs.SetInt(saveKey, 0);
    }
}
