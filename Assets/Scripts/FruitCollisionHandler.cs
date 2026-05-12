using UnityEngine;

/// <summary>
/// 과일 간의 충돌을 감지하고 머지 처리를 담당하는 클래스
/// </summary>
public class FruitCollisionHandler : MonoBehaviour
{
    [SerializeField]
    private GameObject fruitPrefab;

    [SerializeField]
    private float mergeDelay = 0.2f; // 머지 후 지연 시간 (재머지 방지)

    [SerializeField]
    private float bounceRestitution = 0.3f; // 반발력

    private Fruit fruit;
    private float lastMergeTime = -1f;
    private Rigidbody2D rb;

    private void Start()
    {
        fruit = GetComponent<Fruit>();
        rb = GetComponent<Rigidbody2D>();

        if (fruit == null)
        {
            Debug.LogError("Fruit 컴포넌트를 찾을 수 없습니다!");
        }

        // PhysicsMaterial2D 설정
        if (rb != null)
        {
            PhysicsMaterial2D physicsMaterial = new PhysicsMaterial2D();
            physicsMaterial.friction = 0.4f; // 마찰력 설정
            physicsMaterial.bounciness = bounceRestitution; // 반발력 설정
            rb.sharedMaterial = physicsMaterial;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (fruit == null || fruit.IsMerged())
            return;

        Fruit otherFruit = collision.gameObject.GetComponent<Fruit>();
        if (otherFruit == null || otherFruit.IsMerged())
            return;

        // 같은 종류의 과일인지 확인
        if (fruit.GetFruitType() == otherFruit.GetFruitType())
        {
            // 머지 시간이 충분히 지났는지 확인 (같은 프레임의 중복 머지 방지)
            if (Time.time - lastMergeTime > mergeDelay)
            {
                MergeFruits(fruit, otherFruit);
                lastMergeTime = Time.time;
            }
        }
    }

    /// <summary>
    /// 두 과일을 머지한다
    /// </summary>
    private void MergeFruits(Fruit fruit1, Fruit fruit2)
    {
        FruitType currentType = fruit1.GetFruitType();
        FruitType nextType = FruitDatabase.GetNextFruitType(currentType);

        // 두 과일의 중심 위치에서 새 과일 생성
        Vector3 mergePosition = (fruit1.transform.position + fruit2.transform.position) / 2f;

        // 새로운 과일 생성
        GameObject newFruitObj = Instantiate(fruitPrefab, mergePosition, Quaternion.identity);
        FruitData nextData = FruitDatabase.GetFruitData(nextType);
        newFruitObj.name = nextData.name;

        Fruit newFruit = newFruitObj.GetComponent<Fruit>();
        if (newFruit != null)
        {
            newFruit.Initialize(nextType);
        }

        // 머지된 과일들을 표시
        fruit1.MarkAsMerged();
        fruit2.MarkAsMerged();

        // 점수 추가
        GameManager.Instance.AddScore(nextData.score);

        // 머지된 과일들 삭제
        fruit1.Destroy();
        fruit2.Destroy();

        Debug.Log($"{nextData.name} 생성! 점수 +{nextData.score}");
    }
}
