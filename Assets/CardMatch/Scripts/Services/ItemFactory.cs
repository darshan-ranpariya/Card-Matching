using System.Collections.Generic;
using UnityEngine;

public class ItemFactory
{
    private readonly Item itemPrefab;
    private readonly Transform itemParent;
    private readonly SpriteCollection sprites;
    private readonly Sprite redSprite;
    private readonly Sprite yellowSprite;

    public ItemFactory(Item itemPrefab, Transform itemParent, SpriteCollection sprites, Sprite redSprite, Sprite yellowSprite)
    {
        this.itemPrefab = itemPrefab;
        this.itemParent = itemParent;
        this.sprites = sprites;
        this.redSprite = redSprite;
        this.yellowSprite = yellowSprite;
    }

    public List<Item> CreateItems(int totalItemCount, StageData stageData, IGameManager gameManager)
    {
        List<Item> allItemList = new List<Item>(totalItemCount);
        Sprite backgroundSprite = (Rand.GetFloat(0f, 1f) < 0.5f ? yellowSprite : redSprite);
        stageData.isRedSprite = backgroundSprite == redSprite;
        stageData.cardItems.Clear();

        // Pre-allocate list capacity
        if (stageData.cardItems.Capacity < totalItemCount)
        {
            stageData.cardItems.Capacity = totalItemCount;
        }

        for (int i = 0; i < totalItemCount; i++)
        {
            Item item = Object.Instantiate(itemPrefab, itemParent, false);
            item.SetGameManager(gameManager);
            item.graphic.resetSprite = backgroundSprite;
            allItemList.Add(item);
            stageData.cardItems.Add(new CardItemData());
        }

        return allItemList;
    }

    public void AssignIndexesToItems(List<Item> items, int totalItemCount, StageData stageData)
    {
        int[] spriteIndexes = Rand.GetIntUniqueArray(totalItemCount / 2, 0, sprites.sprites.Count);
        int[] totalIndexes = Rand.GetIntUniqueArray(totalItemCount, 0, totalItemCount + 1);

        for (int i = 0, j = 0; i < totalItemCount - 1; i += 2, j++)
        {
            int idx0 = totalIndexes[i] == totalItemCount ? 0 : totalIndexes[i];
            int idx1 = totalIndexes[i + 1] == totalItemCount ? 0 : totalIndexes[i + 1];
            int spriteIdx = spriteIndexes[j];

            items[idx0].graphic.spriteIndex = items[idx0].index = stageData.cardItems[idx0].index = spriteIdx;
            items[idx0].Init();

            items[idx1].graphic.spriteIndex = items[idx1].index = stageData.cardItems[idx1].index = spriteIdx;
            items[idx1].Init();
        }
    }

    public List<Item> CreateItemsFromStageData(StageData stageData, int totalItemCount, IGameManager gameManager, out int destroyedCount)
    {
        List<Item> allItemList = new List<Item>(totalItemCount);
        destroyedCount = 0;
        Sprite backgroundSprite = stageData.isRedSprite ? redSprite : yellowSprite;

        for (int i = 0; i < totalItemCount; i++)
        {
            Item item = Object.Instantiate(itemPrefab, itemParent, false);
            item.SetGameManager(gameManager);
            item.graphic.resetSprite = backgroundSprite;
            item.graphic.spriteIndex = item.index = stageData.cardItems[i].index;

            if (stageData.cardItems[i].isDestroyed)
            {
                item.DestroyItem();
                destroyedCount++;
            }

            item.Init();
            allItemList.Add(item);
        }

        return allItemList;
    }

    public void ClearAllItems()
    {
        int childCount = itemParent.childCount;

        if (Application.isPlaying)
        {
            for (int i = childCount - 1; i >= 0; i--)
            {
                Object.Destroy(itemParent.GetChild(i).gameObject);
            }
        }
        else
        {
            for (int i = childCount - 1; i >= 0; i--)
            {
                Object.DestroyImmediate(itemParent.GetChild(i).gameObject);
            }
        }
    }
}
