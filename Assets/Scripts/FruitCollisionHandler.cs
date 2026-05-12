using UnityEngine;

/// <summary>
/// 과일 간의 충돌을 감지하고 머지 처리를 담당하는 클래스
/// </summary>
public class FruitCollisionHandler : MonoBehaviour
{
    [SerializeField]
    private float mergeDelay = 0.2f;

    [SerializeField]
    private float bounceRestitution = 0.3f;

    private Fruit fruit;
    private float lastMergeTime = -1f;
    private Rigidbody2D rb;

    private void Start()
    {
        fruit = GetComponent<Fruit>();
        rb = GetComponent<Rigidbody2D>();

        if (fruit == null)
            Debug.LogError("Fruit 컴포넌트를 찾을 수 없습니다!");

        if (rb != null)
        {
            PhysicsMaterial2D physicsMaterial = new PhysicsMaterial2D();
            physicsMaterial.friction = 0.4f;
            physicsMaterial.bounciness = bounceRestitution;
            rb.sharedMaterial = physicsMaterial;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (fruit == null || fruit.IsMerged())
            return;

        // 수박은 최종 단계 — 더 이상 머지 없음
        if (fruit.GetFruitType() == FruitType.Watermelon)
            return;

        Fruit otherFruit = collision.gameObject.GetComponent<Fruit>();
        if (otherFruit == null || otherFruit.IsMerged())
            return;

        if (fruit.GetFruitType() == otherFruit.GetFruitType())
        {
            if (Time.time - lastMergeTime > mergeDelay)
            {
                MergeFruits(fruit, otherFruit);
                lastMergeTime = Time.time;
            }
        }
    }

    /// <summary>
    /// 두 과일을 머지하고 다음 단계 과일을 생성한다
    /// </summary>
    private void MergeFruits(Fruit fruit1, Fruit fruit2)
    {
        GameObject prefab = GameManager.Instance?.GetFruitPrefab();
        if (prefab == null)
        {
            Debug.LogError("GameManager에 FruitPrefab이 할당되지 않았습니다!");
            return;
        }

        FruitType currentType = fruit1.GetFruitType();
        FruitType nextType = FruitDatabase.GetNextFruitType(currentType);
        Vector3 mergePosition = (fruit1.transform.position + fruit2.transform.position) / 2f;

        fruit1.MarkAsMerged();
        fruit2.MarkAsMerged();

        FruitData nextData = FruitDatabase.GetFruitData(nextType);
        GameManager.Instance.AddScore(nextData.score);

        fruit1.Destroy();
        fruit2.Destroy();

        GameObject newFruitObj = Instantiate(prefab, mergePosition, Quaternion.identity);
        newFruitObj.name = nextData.name;
        Fruit newFruit = newFruitObj.GetComponent<Fruit>();
        if (newFruit != null)
            newFruit.Initialize(nextType);

        Debug.Log($"{nextData.name} 생성! 점수 +{nextData.score}");
    }
}
