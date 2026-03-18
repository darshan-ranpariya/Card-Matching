using UnityEngine;
using UnityEngine.UI;

public class ItemGraphics : MonoBehaviour
{
    public Image graphic;
    public Image bgImg;
    public SpriteCollection sprites;
    public int spriteIndex;
    public Sprite resetSprite;

    private GameObject graphicGameObject;

    private void Awake()
    {
        if (bgImg == null)
        {
            bgImg = GetComponent<Image>();
        }

        if (graphic != null)
        {
            graphicGameObject = graphic.gameObject;
        }
    }

    private void OnValidate()
    {
        if (bgImg == null)
        {
            bgImg = GetComponent<Image>();
        }
    }

    public void Init()
    {
        if (sprites != null && graphic != null)
        {
            graphic.sprite = sprites.GetSprite(spriteIndex);
        }

        if (bgImg != null && resetSprite != null)
        {
            bgImg.sprite = resetSprite;
        }
    }

    public void SetSprite()
    {
        SetImageActive(true);
    }

    public void ResetSprite()
    {
        SetImageActive(false);
    }

    private void SetImageActive(bool isActive)
    {
        if (graphicGameObject != null)
        {
            graphicGameObject.SetActive(isActive);
        }
    }
}
