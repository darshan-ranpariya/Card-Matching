using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpriteCollection", menuName = "SpriteCollection")]
public class SpriteCollection : ScriptableObject
{ 
    public List<Sprite> sprites = new List<Sprite>(); 

    public Sprite GetSprite(int _index)
    {
        try
        {
            return sprites[_index];
        }
        catch
        {
            return null;
        }
    }

    public Sprite GetSprite(string _name, System.StringComparison _comparison = System.StringComparison.CurrentCultureIgnoreCase)
    {
        try
        {
            return sprites.Find((s)=> { return s.name.Equals(_name, _comparison); });
        }
        catch
        {
            return null;
        }
    }

    static Texture2D _transparentTexture;
    public static Texture2D transparentTexture
    {
        get
        {
            if (_transparentTexture == null)
            {
                _transparentTexture = new Texture2D(1, 1);
                _transparentTexture.SetPixel(0, 0, new Color(0, 0, 0, 0f));
                _transparentTexture.Apply();
            }
            return _transparentTexture;
        }
    }
    static Sprite _transparentSprite;
    public static Sprite transparentSprite
    {
        get
        {
            if (_transparentSprite == null)
            {
                _transparentSprite = Sprite.Create(transparentTexture, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f));
            }
            return _transparentSprite;
        }
    }
}
