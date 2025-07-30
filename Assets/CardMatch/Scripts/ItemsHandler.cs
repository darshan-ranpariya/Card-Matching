using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemsHandler : MonoBehaviour
{
    public static ItemsHandler inst;

    public Item itemPrefab;
    public Transform itemPerent;
    public SpriteCollection sprites;
    public Sprite redSprite;
    public Sprite yellowSprite;

    GridLayoutGroup gridLayout;
    RectTransform gridLayoutRectTrans;

    int totalItemCount = 0;

    [Header("ItemRelated")]
    int[] spriteIndexes;
    List<Item> allItemList = new List<Item>();

    internal Item currFlippedItem;

    [Header("Combo System")]
    private int currentComboCount = 0;
    private int comboScore = 0;
    private bool isInCombo = false;


    [HideInInspector]
    int itemCount;

    StageData currStageData;
    private void Awake()
    {
        inst = this;
        gridLayout = itemPerent.GetComponent<GridLayoutGroup>();
        gridLayoutRectTrans = itemPerent.GetComponent<RectTransform>();
        currStageData = new StageData();
        currStageData.cardItems = new List<CardItemData>();
    }

    #region Generation Mathods
    public void GenerateItemAndIndex()
    {
        spriteIndexes = Rand.GetIntUniqueArray(totalItemCount / 2, 0, sprites.sprites.Count);
        allItemList = new List<Item>();
        ClearChildren(itemPerent);
        GenerateItems();
        GenerateIndexes();
        itemCount = totalItemCount;
    }

    private void GenerateItems()
    {
        Sprite s = (Rand.GetFloat(0f, 1f) < 0.5f ? yellowSprite : redSprite);
        currStageData.isRedSprite = s == redSprite;
        currStageData.cardItems.Clear();
        for (int i = 0; i < totalItemCount; i++)
        {
            Item item = Instantiate(itemPrefab, itemPerent);
            item.graphic.resetSprite = s;
            allItemList.Add(item);
            currStageData.cardItems.Add(new CardItemData());
        }
    }

    void GenerateIndexes()
    {
        int[] totalIndexes = Rand.GetIntUniqueArray(totalItemCount, 0, totalItemCount + 1);
        for (int i = 0, j = 0; i < totalItemCount - 1; i += 2, j++)
        {
            int idx0 = totalIndexes[i] == totalItemCount ? 0 : totalIndexes[i];
            int idx1 = totalIndexes[i + 1] == totalItemCount ? 0 : totalIndexes[i + 1];
            int spriteIdx = spriteIndexes[j];

            allItemList[idx0].graphic.spriteIndex = allItemList[idx0].index = currStageData.cardItems[idx0].index = spriteIdx;
            allItemList[idx0].Init();

            allItemList[idx1].graphic.spriteIndex = allItemList[idx1].index = currStageData.cardItems[idx1].index = spriteIdx;
            allItemList[idx1].Init();
        }
    }

    void GenerateItemsFromStageData()
    {
        ClearChildren(itemPerent);
        int destroiedCount = 0;
        for (int i = 0; i < totalItemCount; i++)
        {
            Item item = Instantiate(itemPrefab, itemPerent);
            item.graphic.resetSprite = currStageData.isRedSprite ? redSprite : yellowSprite;
            item.graphic.spriteIndex = item.index = currStageData.cardItems[i].index;
            if (currStageData.cardItems[i].isDestroyed)
            {
                item.DestroyItem();
                destroiedCount++;
            }
            item.Init();
            allItemList.Add(item);
        }
        itemCount = totalItemCount - destroiedCount;
    }

    public void ClearChildren(Transform transformToClearChildOf)
    {
        List<GameObject> children = new List<GameObject>();

        for (int i = 0; i < transformToClearChildOf.childCount; i++)
            children.Add(transformToClearChildOf.GetChild(i).gameObject);

        for (int i = 0; i < children.Count; i++)
        {
            if (Application.isPlaying)
            {
                Destroy(children[i]);
            }
            else
            {
                DestroyImmediate(children[i]);
            }
        }
    }
    #endregion

    Item currItem;
    Coroutine ResetItemsCoroutine;
    public void OnItemBtnClick(Item item)
    {
        if (currFlippedItem != null && currItem != null)
        {
            StopCoroutine(ResetItemsCoroutine);
            currItem.ResetItem();
            currFlippedItem.ResetItem();
            currFlippedItem = null;
            currItem = null;
            currFlippedItem = item;
        }
        else
        {
            currItem = item;
            if (currFlippedItem != null)
            {
                if (currFlippedItem.index == currItem.index)
                {
                    // Match found - handle combo immediately
                    HandleMatch(currFlippedItem, currItem);

                    // UIManager.inst.UpdateScore(1);
                    // UIManager.inst.DelayedAction(currItem.DestroyItem, 1f);
                    // UIManager.inst.DelayedAction(currFlippedItem.DestroyItem, 1f);
                    // itemCount -= 2;
                    currFlippedItem = null;
                    currItem = null;
                    if (itemCount == 0)
                    {
                        EndCombo();

                        UIManager.inst.DelayedAction(() =>
                        {
                            UIManager.inst.TurnOffAllPanels();
                            UIManager.inst.winPanel.gameObject.SetActive(true);
                        }, .5f);
                    }
                }
                else
                {
                    // Mismatch - end combo immediately and award score
                    EndCombo();

                    ResetItemsCoroutine = StartCoroutine(ResetFaultItems());
                }
            }
            else
            {
                currFlippedItem = currItem;
                currItem = null;
            }
        }
    }

    #region Combo System


    // Handle successful match
    void HandleMatch(Item item1, Item item2)
    {
        // Start or continue combo
        if (!isInCombo)
        {
            StartCombo();
        }

        currentComboCount++;

        // Calculate base score + combo bonus
        int baseScore = 1;
        int comboBonus = currentComboCount > 1 ? (currentComboCount - 1) : 0;
        int matchScore = baseScore + comboBonus;

        comboScore += matchScore;

        // Show combo feedback
        ShowComboFeedback(currentComboCount, matchScore);

        // Destroy matched items
        UIManager.inst.DelayedAction(item1.DestroyItem, 1f);
        UIManager.inst.DelayedAction(item2.DestroyItem, 1f);
        itemCount -= 2;

        Debug.Log($"Match! Combo: {currentComboCount}, Match Score: {matchScore}, Total Combo Score: {comboScore}");
    }

    // Start a new combo
    void StartCombo()
    {
        isInCombo = true;
        currentComboCount = 0;
        comboScore = 0;
        Debug.Log("Combo Started!");
    }

    // Show ongoing combo feedback
    void ShowComboFeedback(int comboCount, int matchScore)
    {
        if (comboCount == 1)
        {
            Debug.Log("First match!");
            UIManager.inst.comboTxt.gameObject.SetActive(false);
        }
        else
        {
            Debug.Log($"COMBO x{comboCount}! +{matchScore} points");
            UIManager.inst.comboTxt.text = $"COMBO x {comboCount} !";
            UIManager.inst.comboTxt.gameObject.SetActive(true);
        }
    }

    // End combo and award accumulated score
    void EndCombo()
    {
        if (!isInCombo) return;

        // Award the accumulated combo score
        UIManager.inst.UpdateScore(comboScore);

        Debug.Log($"Combo Ended! Total pairs: {currentComboCount}, Final score awarded: {comboScore}");

        // Reset combo state
        isInCombo = false;
        currentComboCount = 0;
        comboScore = 0;
    }


    #endregion

    IEnumerator ResetFaultItems()
    {
        yield return new WaitForSeconds(.5f);
        if (currItem) iTween.PunchPosition(currItem.gameObject, iTween.Hash("x", .2f, "time", 0.5f));
        if (currFlippedItem) iTween.PunchPosition(currFlippedItem.gameObject, iTween.Hash("x", .2f, "time", 0.5f));
        yield return new WaitForSeconds(1f);
        ResetItems();
    }

    void ResetItems()
    {
        if (currItem) currItem.ResetItem();
        if (currFlippedItem) currFlippedItem.ResetItem();
        currFlippedItem = null;
        currItem = null;
    }

    public void OnGridSwitchOn(int row, int column, bool isLoadingSavedData = false)
    {
        float size = (gridLayoutRectTrans.rect.width - column - 1) / column;
        gridLayout.cellSize = new Vector2(size, size);
        gridLayout.constraintCount = column;
        totalItemCount = row * column;
        UIManager.inst.totalTime = totalItemCount * 3;
        currStageData.row = row;
        currStageData.column = column;
        if (!isLoadingSavedData)
        {
            UIManager.inst.currTime = UIManager.inst.totalTime;
            GenerateItemAndIndex();
        }
    }

    #region LoadnSave the Stage data 
    internal void LoadGameSavedData()
    {
        // Load stored data
        Debug.Log("Loading saved data");
        string json = System.IO.File.ReadAllText(Utility.stageDataPath);
        StageData stageData = JsonUtility.FromJson<StageData>(json);
        currStageData = stageData;
        UIManager.inst.currScore = stageData.score;
        UIManager.inst.currTime = UIManager.inst.totalTime = stageData.time;
        UIManager.inst.currScoreTxt.text = stageData.score.ToString();
        UIManager.inst.StartGame();
        OnGridSwitchOn(stageData.row, stageData.column, true);
        GenerateItemsFromStageData();
        PlayerPrefs.SetInt(Utility.StageDataSavePrefKey, 0);
    }

    internal void SaveStageData()
    {
        Debug.Log("Saving data");
        EndCombo();
        PlayerPrefs.SetInt(Utility.StageDataSavePrefKey, 1);
        currStageData.time = (int)UIManager.inst.currTime;
        currStageData.score = (int)UIManager.inst.currScore;

        string json = JsonUtility.ToJson(currStageData);

        System.IO.File.WriteAllText(Utility.stageDataPath, json);
    }

    public void UpdateStageData(int index)
    {
        currStageData.cardItems[index].isDestroyed = true;
    }
    #endregion
}

[Serializable]
public class CardItemData
{
    public int index;
    public bool isDestroyed = false;
}
[Serializable]
public class StageData
{
    public int row;
    public int column;
    public bool isRedSprite;
    public int time;
    public int score;
    public List<CardItemData> cardItems;
}
