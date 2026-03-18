using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpriteCollection", menuName = "SpriteCollection")]
public class SpriteCollection : ScriptableObject
{ 
    public List<Sprite> sprites = new List<Sprite>(); 

    public Sprite GetSprite(int _index)
    {
        if (_index >= 0 && _index < sprites.Count)
        {
            return sprites[_index];
        }

        Debug.LogWarning($"SpriteCollection: Index {_index} out of range. Sprite count: {sprites.Count}");
        return null;
    }

    public Sprite GetSprite(string _name, System.StringComparison _comparison = System.StringComparison.CurrentCultureIgnoreCase)
    {
        if (string.IsNullOrEmpty(_name))
        {
            Debug.LogWarning("SpriteCollection: Sprite name is null or empty");
            return null;
        }

        for (int i = 0; i < sprites.Count; i++)
        {
            if (sprites[i] != null && sprites[i].name.Equals(_name, _comparison))
            {
                return sprites[i];
            }
        }

        Debug.LogWarning($"SpriteCollection: Sprite with name '{_name}' not found");
        return null;
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
