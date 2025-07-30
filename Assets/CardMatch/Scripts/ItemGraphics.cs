using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemGraphics : MonoBehaviour
{
    public Image graphic;
    public Image bgImg;
    public SpriteCollection sprites;
    public int spriteIndex;
    public Sprite resetSprite;

    private void OnValidate()
    {
        bgImg = GetComponent<Image>();
    }

    internal void Init()
    {
        graphic.sprite = sprites.GetSprite(spriteIndex);
        bgImg.sprite = resetSprite;
    }

    public void SetSprite()
    {
        SetImageComps(true);
    }

    public void ResetSprite()
    {
        SetImageComps(false);
    }

    void SetImageComps(bool b)
    {
        graphic.gameObject.SetActive(b);
    }
}
