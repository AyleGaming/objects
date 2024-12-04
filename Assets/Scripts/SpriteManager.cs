using System.Collections.Generic;
using UnityEngine;

public class SpriteManager : MonoBehaviour
{
    public static SpriteManager Instance { get; private set; }
    private Dictionary<string, Sprite> spriteCache = new Dictionary<string, Sprite>();
    [SerializeField] private Sprite[] allSprites; // Assign all relevant sprites via Inspector

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Ensure persistence across scenes
            PreloadSprites();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void PreloadSprites()
    {
        foreach (var sprite in allSprites)
        {
            if (!spriteCache.ContainsKey(sprite.name))
            {
                spriteCache.Add(sprite.name, sprite);
            }
        }
    }

    public Sprite GetSprite(string spriteName)
    {
        if (spriteCache.TryGetValue(spriteName, out Sprite sprite))
        {
            return sprite;
        }

        Debug.LogError($"Sprite not found: {spriteName}");
        return null;
    }
}
