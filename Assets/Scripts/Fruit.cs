using UnityEngine;

public class Fruit : MonoBehaviour
{
    [SerializeField]
    private FruitType fruitType;
    private Rigidbody2D rb;
    private CircleCollider2D circleCollider;
    private SpriteRenderer spriteRenderer;
    private FruitData fruitData;

    private bool isMerged = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        circleCollider = GetComponent<CircleCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.gravityScale = 1f;
            rb.linearDamping = 0.05f;
            rb.angularDamping = 0.5f;
            rb.constraints = RigidbodyConstraints2D.None;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        }
    }

    public void Initialize(FruitType type)
    {
        fruitType = type;
        fruitData = FruitDatabase.GetFruitData(type);

        if (fruitData == null)
        {
            Debug.LogError($"과일 데이터를 찾을 수 없습니다: {type}");
            return;
        }

        if (spriteRenderer != null)
        {
            if (fruitData.sprite != null)
            {
                spriteRenderer.sprite = fruitData.sprite;
                spriteRenderer.color = Color.white;

                float localR = fruitData.spriteLocalRadius;
                if (localR > 0f)
                {
                    transform.localScale = Vector3.one * (fruitData.radius / localR);
                    if (circleCollider != null)
                    {
                        circleCollider.radius = localR;
                        circleCollider.isTrigger = false;
                    }
                }
                else
                {
                    ApplyDefaultSizeAndCollider();
                }
            }
            else
            {
                spriteRenderer.color = fruitData.color;
                ApplyDefaultSizeAndCollider();
            }
        }

        if (rb != null)
        {
            rb.gravityScale = 1f;
            rb.mass = fruitData.radius * fruitData.radius;
            rb.linearDamping = 0.05f;
            rb.angularDamping = 0.5f;
            rb.constraints = RigidbodyConstraints2D.None;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        }

        isMerged = false;
    }

    private void ApplyDefaultSizeAndCollider()
    {
        transform.localScale = Vector3.one * fruitData.radius * 2f;
        if (circleCollider != null)
        {
            circleCollider.radius = 0.5f;
            circleCollider.isTrigger = false;
        }
    }

    public FruitType GetFruitType() => fruitType;
    public FruitData GetFruitData() => fruitData;

    public void MarkAsMerged() { isMerged = true; }
    public bool IsMerged() => isMerged;

    public void SetVelocity(Vector2 velocity)
    {
        if (rb != null)
            rb.linearVelocity = velocity;
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }
}
