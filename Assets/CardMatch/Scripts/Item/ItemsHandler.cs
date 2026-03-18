using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemsHandler : MonoBehaviour, IGameManager
{
    public static ItemsHandler inst;

    public Item itemPrefab;
    public Transform itemPerent;
    public SpriteCollection sprites;
    public Sprite redSprite;
    public Sprite yellowSprite;

    private GridLayoutGroup gridLayout;
    private RectTransform gridLayoutRectTrans;
    private int totalItemCount = 0;
    private List<Item> allItemList = new List<Item>();
    private Item currFlippedItem;
    private Item currItem;
    private Coroutine resetItemsCoroutine;
    private int itemCount;
    private StageData currStageData;

    private ComboService comboService;
    private ItemFactory itemFactory;
    private ISaveLoadService saveLoadService;
    private IUIService uiService;

    private void Awake()
    {
        inst = this;
        gridLayout = itemPerent.GetComponent<GridLayoutGroup>();
        gridLayoutRectTrans = itemPerent.GetComponent<RectTransform>();
        currStageData = new StageData();
        currStageData.cardItems = new List<CardItemData>();

        itemFactory = new ItemFactory(itemPrefab, itemPerent, sprites, redSprite, yellowSprite);
        saveLoadService = new SaveLoadService(Utility.stageDataPath, Utility.StageDataSavePrefKey);
    }

    private void Start()
    {
        uiService = UIManager.inst;
        comboService = new ComboService(uiService);
    }

    #region Item Generation
    public void GenerateItemAndIndex()
    {
        allItemList = new List<Item>();
        itemFactory.ClearAllItems();
        allItemList = itemFactory.CreateItems(totalItemCount, currStageData, this);
        itemFactory.AssignIndexesToItems(allItemList, totalItemCount, currStageData);

        foreach (var item in allItemList)
        {
            item.SetUIService(uiService);
        }

        itemCount = totalItemCount;
    }

    private void GenerateItemsFromStageData()
    {
        itemFactory.ClearAllItems();
        allItemList = itemFactory.CreateItemsFromStageData(currStageData, totalItemCount, this, out int destroyedCount);

        foreach (var item in allItemList)
        {
            item.SetUIService(uiService);
        }

        itemCount = totalItemCount - destroyedCount;
    }
    #endregion

    #region Item Matching Logic
    public void OnItemFlipped(Item item)
    {
        if (currFlippedItem != null && currItem != null)
        {
            if (resetItemsCoroutine != null)
            {
                StopCoroutine(resetItemsCoroutine);
                resetItemsCoroutine = null;
            }

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
                    HandleMatch(currFlippedItem, currItem);
                    currFlippedItem = null;
                    currItem = null;

                    if (itemCount == 0)
                    {
                        comboService.EndCombo();
                        AudioManager.inst?.PlayWin();
                        uiService.ShowWinPanel();
                    }
                }
                else
                {
                    AudioManager.inst?.PlayMismatch();
                    comboService.EndCombo();
                    resetItemsCoroutine = StartCoroutine(ResetFaultItems());
                }
            }
            else
            {
                currFlippedItem = currItem;
                currItem = null;
            }
        }
    }

    private void HandleMatch(Item item1, Item item2)
    {
        comboService.HandleMatch();

        uiService.DelayedAction(item1.DestroyItem, 1f);
        uiService.DelayedAction(item2.DestroyItem, 1f);
        itemCount -= 2;
    }

    private IEnumerator ResetFaultItems()
    {
        yield return new WaitForSeconds(0.5f);

        if (currItem != null)
        {
            iTween.PunchPosition(currItem.gameObject, iTween.Hash("x", 0.2f, "time", 0.5f));
        }

        if (currFlippedItem != null)
        {
            iTween.PunchPosition(currFlippedItem.gameObject, iTween.Hash("x", 0.2f, "time", 0.5f));
        }

        yield return new WaitForSeconds(1f);
        ResetItems();
        resetItemsCoroutine = null;
    }

    private void ResetItems()
    {
        if (currItem) currItem.ResetItem();
        if (currFlippedItem) currFlippedItem.ResetItem();
        currFlippedItem = null;
        currItem = null;
    }
    #endregion

    #region Grid Configuration
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
    #endregion

    #region Save/Load System
    internal void LoadGameSavedData()
    {
        StageData stageData = saveLoadService.LoadStageData();
        currStageData = stageData;
        UIManager.inst.currScore = stageData.score;
        UIManager.inst.currTime = UIManager.inst.totalTime = stageData.time;
        UIManager.inst.currScoreTxt.text = stageData.score.ToString();
        UIManager.inst.StartGame();
        OnGridSwitchOn(stageData.row, stageData.column, true);
        GenerateItemsFromStageData();
    }

    internal void SaveStageData()
    {
        comboService.EndCombo();
        currStageData.time = (int)UIManager.inst.currTime;
        currStageData.score = (int)UIManager.inst.currScore;
        saveLoadService.SaveStageData(currStageData);
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
