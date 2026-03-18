public interface ISaveLoadService
{
    bool HasSavedData();
    StageData LoadStageData();
    void SaveStageData(StageData stageData);
    void ClearSaveData();
}
