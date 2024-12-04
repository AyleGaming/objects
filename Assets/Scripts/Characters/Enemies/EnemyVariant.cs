using UnityEngine;

public class EnemyVariant : MonoBehaviour
{
    [SerializeField] private EnemyType enemyType;           // Set in Inspector or dynamically
    [SerializeField] private int variantID = 1;             // Spitter, Biter, etc.
    [SerializeField] private string size = "Medium";        // Small, Medium, Large

    private SpriteRenderer spriteRenderer;


    void Awake()
    {
        // Initialize the spriteRenderer in Awake, as it's called before Start
        spriteRenderer = GetComponent<SpriteRenderer>();

    }

    public void LoadSpriteBasedOnEnemyType(EnemyType enemyType, int variant)
    {
        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer is missing on " + gameObject.name);
        }

        variantID = variant;
        string enemyTypeLetter = GetEnemyTypeByLetter(enemyType);
        if (string.IsNullOrEmpty(enemyTypeLetter))
        {
            return;
        }

        // Construct the sprite filename
        string spriteName = $"Enemy_{variantID}_{enemyTypeLetter}_{size}";
        Sprite loadedSprite = SpriteManager.Instance.GetSprite(spriteName);

        if (loadedSprite != null)
        {
            spriteRenderer.sprite = loadedSprite;
            transform.rotation = Quaternion.Euler(180f, 0, 0); // Flips it horizontally

            // set color
//            spriteRenderer.color = GetColorBasedOnEnemyType(enemyType);
        }
    }

    private string GetEnemyTypeByLetter(EnemyType enemyType)
    {
        return enemyType switch
        {
            EnemyType.Earth => "A",
            EnemyType.Ice => "B",
            EnemyType.Poison => "C",
            EnemyType.Fire => "D",
            _ => null,
        };
    }

    private Color GetColorBasedOnEnemyType(EnemyType enemyType)
    {
        switch (enemyType)
        {
            case EnemyType.Earth:
                return new Color(0f, 0.78f, 0.57f);
            case EnemyType.Fire:
                return new Color(1, 0.89f, 0.89f);
                //return new Color(1, 0.6f, 0.6f);
            case EnemyType.Ice:
                return new Color(0f, 0.99f, 1f);
            case EnemyType.Poison:
                return new Color(0.25f, 1f, 0.7f); ; // Greenish
            default:
                return Color.white; // Default to white
        }
    }
}
