using UnityEngine;

public class Item : MonoBehaviour
{
    public ItemGraphics graphic;
    public Animator animator;
    public int index;

    public bool isFlipped;


    public void Init()
    {
        name = index + "";
        graphic.Init();
    }

    public void BtnClick()
    {
        if (isFlipped) return;
        animator.SetTrigger("Flip");
        isFlipped = true;
        ItemsHandler.inst.OnItemBtnClick(this);
    }

    internal void ResetItem()
    {
        //print("reset called: " + name) ;
        animator.SetTrigger("Reset");
        isFlipped = false;
    }

    internal void DestroyItem()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            var item = transform.GetChild(i);
            iTween.ScaleTo(item.gameObject, iTween.Hash("scale", Vector3.zero, "time", 0.5f, "easetype", iTween.EaseType.easeInElastic));
            UIManager.inst.DelayedAction(() =>
            {
                if(item) item.gameObject.SetActive(false);
            }, 0.7f);
        }
        ItemsHandler.inst.UpdateStageData(transform.GetSiblingIndex());
    }
}
