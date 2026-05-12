using UnityEngine;

/// <summary>
/// 개별 과일을 나타내는 클래스
/// </summary>
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

        // 기본 물리 설정
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.gravityScale = 1f;
            rb.linearDamping = 0f; // 직선 운동 저항 없음
            rb.angularDamping = 0.05f; // 회전 저항 (약간만 적용)





            rb.constraints = RigidbodyConstraints2D.FreezeRotation; // 회전 고정
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;





            
        }
    }

    /// <summary>
    /// 과일을 초기화한다
    /// </summary>
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

                // spriteLocalRadius: 픽셀 분석으로 측정한 실제 콘텐츠 반경 (로컬 공간)
                // sprite.bounds.extents는 투명 여백 포함 전체 텍스처 크기를 반환하므로 사용하지 않음
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

        // Rigidbody 설정
        if (rb != null)
        {
            rb.gravityScale = 1f;
            rb.mass = fruitData.radius;
            rb.linearDamping = 0f;
            rb.angularDamping = 0.05f;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        }

        isMerged = false;
    }

    /// <summary>
    /// 스프라이트 없을 때 또는 bounds를 읽지 못할 때 사용하는 기본 크기/콜라이더 설정
    /// scale = diameter, 콜라이더 로컬 반경 = 0.5 → 월드 반경 = fruitData.radius
    /// </summary>
    private void ApplyDefaultSizeAndCollider()
    {
        transform.localScale = Vector3.one * fruitData.radius * 2f;
        if (circleCollider != null)
        {
            circleCollider.radius = 0.5f;
            circleCollider.isTrigger = false;
        }
    }

    /// <summary>
    /// 과일의 타입을 반환한다
    /// </summary>
    public FruitType GetFruitType()
    {
        return fruitType;
    }

    /// <summary>
    /// 과일의 데이터를 반환한다
    /// </summary>
    public FruitData GetFruitData()
    {
        return fruitData;
    }

    /// <summary>
    /// 과일을 머지 표시한다 (다시 머지되지 않도록)
    /// </summary>
    public void MarkAsMerged()
    {
        isMerged = true;
    }

    /// <summary>
    /// 과일이 이미 머지되었는지 확인한다
    /// </summary>
    public bool IsMerged()
    {
        return isMerged;
    }

    /// <summary>
    /// 과일의 속도를 설정한다
    /// </summary>
    public void SetVelocity(Vector2 velocity)
    {
        if (rb != null)
        {
            rb.linearVelocity = velocity;
        }
    }

    /// <summary>
    /// 과일을 삭제한다
    /// </summary>
    public void Destroy()
    {
        Destroy(gameObject);
    }
}
