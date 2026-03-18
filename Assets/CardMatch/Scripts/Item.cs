using UnityEngine;

public class Item : MonoBehaviour
{
    public ItemGraphics graphic;
    public Animator animator;
    public int index;
    public bool isFlipped;

    private IGameManager gameManager;
    private IUIService uiService;
    private Transform cachedTransform;

    private static readonly int FlipTriggerHash = Animator.StringToHash("Flip");
    private static readonly int ResetTriggerHash = Animator.StringToHash("Reset");

    private void Awake()
    {
        cachedTransform = transform;
    }

    public void SetGameManager(IGameManager gameManager)
    {
        this.gameManager = gameManager;
    }

    public void SetUIService(IUIService uiService)
    {
        this.uiService = uiService;
    }

    public void Init()
    {
        name = index.ToString();
        graphic.Init();
    }

    public void BtnClick()
    {
        if (isFlipped) return;
        animator.SetTrigger(FlipTriggerHash);
        isFlipped = true;
        gameManager?.OnItemFlipped(this);
    }

    public void ResetItem()
    {
        animator.SetTrigger(ResetTriggerHash);
        isFlipped = false;
    }

    public void DestroyItem()
    {
        int childCount = cachedTransform.childCount;
        for (int i = 0; i < childCount; i++)
        {
            Transform childTransform = cachedTransform.GetChild(i);
            GameObject childObj = childTransform.gameObject;

            iTween.ScaleTo(childObj, iTween.Hash("scale", Vector3.zero, "time", 0.5f, "easetype", iTween.EaseType.easeInElastic));

            int capturedIndex = i;
            uiService?.DelayedAction(() =>
            {
                Transform child = cachedTransform.GetChild(capturedIndex);
                if (child != null)
                {
                    child.gameObject.SetActive(false);
                }
            }, 0.7f);
        }
        gameManager?.UpdateStageData(cachedTransform.GetSiblingIndex());
    }
}
