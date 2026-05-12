using UnityEngine;

/// <summary>
/// 개별 과일을 나타내는 클래스
/// </summary>
public class Fruit : MonoBehaviour
{
    [SerializeField] private FruitType fruitType;
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

        // 스프라이트 렌더러의 색상 설정
        if (spriteRenderer != null)
        {
            spriteRenderer.color = fruitData.color;
        }

        // 원형 콜라이더 설정
        if (circleCollider != null)
        {
            circleCollider.radius = fruitData.radius;
        }

        // 스케일 설정
        Vector3 scale = Vector3.one * fruitData.radius * 2;
        transform.localScale = scale;

        // Rigidbody 설정
        if (rb != null)
        {
            rb.gravityScale = 1f;
            rb.constraints = RigidbodyConstraints2D.None;
        }

        isMerged = false;
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
