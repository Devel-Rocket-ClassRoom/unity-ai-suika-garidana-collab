using UnityEngine;

/// <summary>
/// 마우스 입력으로 과일을 생성하고 떨어뜨리는 클래스
/// </summary>
public class FruitSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject fruitPrefab;

    [SerializeField]
    private Transform dropZone;

    [SerializeField]
    private float dropHeight = 7.5f; // 게임오버 라인(Y=6.5) 바로 위

    [SerializeField]
    private float containerWidth = 10f;

    // 드롭 가능한 과일 목록과 등장 가중치 (낮은 단계일수록 자주 등장)
    private static readonly FruitType[] droppableFruits =
    {
        FruitType.Cherry,
        FruitType.Strawberry,
        FruitType.Grape,
        FruitType.Mandarin,
        FruitType.Persimmon,
    };
    private static readonly float[] dropWeights = { 30f, 25f, 20f, 15f, 10f };

    private FruitType nextFruitType;
    private Camera mainCamera;
    private bool canDrop = true;

    private void Start()
    {
        mainCamera = Camera.main;

        if (fruitPrefab == null)
            Debug.LogError("FruitPrefab이 할당되지 않았습니다!");
        if (dropZone == null)
            Debug.LogError("DropZone(게임 컨테이너)이 할당되지 않았습니다!");

        // 시작 시 첫 번째 과일도 랜덤으로 결정
        SetNextFruitType(SelectRandomDroppableFruit());
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && canDrop)
            DropFruit();

        HandleKeyInput();
    }

    private void DropFruit()
    {
        if (mainCamera == null)
        {
            Debug.LogError("Main Camera를 찾을 수 없습니다!");
            return;
        }

        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 10f;
        Vector3 worldPos = mainCamera.ScreenToWorldPoint(mousePos);

        float clampedX = Mathf.Clamp(
            worldPos.x,
            dropZone.position.x - containerWidth / 2,
            dropZone.position.x + containerWidth / 2
        );

        SpawnFruitAtPosition(new Vector3(clampedX, dropHeight, 0), nextFruitType);

        // 드롭 후 다음 과일을 랜덤으로 결정
        SetNextFruitType(SelectRandomDroppableFruit());
    }

    private void SpawnFruitAtPosition(Vector3 position, FruitType fruitType)
    {
        if (fruitPrefab == null)
        {
            Debug.LogError("과일 프리팹이 할당되지 않았습니다!");
            return;
        }

        GameObject fruitObj = Instantiate(fruitPrefab, position, Quaternion.identity);
        fruitObj.name = FruitDatabase.GetFruitData(fruitType).name;

        Fruit fruit = fruitObj.GetComponent<Fruit>();
        if (fruit != null)
            fruit.Initialize(fruitType);
        else
            Debug.LogError("Fruit 컴포넌트를 찾을 수 없습니다!");
    }

    /// <summary>
    /// 가중치 기반 랜덤으로 드롭 가능한 과일 타입을 선택한다
    /// </summary>
    private FruitType SelectRandomDroppableFruit()
    {
        float totalWeight = 0f;
        foreach (float w in dropWeights)
            totalWeight += w;

        float roll = Random.Range(0f, totalWeight);
        float cumulative = 0f;
        for (int i = 0; i < droppableFruits.Length; i++)
        {
            cumulative += dropWeights[i];
            if (roll < cumulative)
                return droppableFruits[i];
        }
        return droppableFruits[0];
    }

    public void SetNextFruitType(FruitType fruitType)
    {
        nextFruitType = fruitType;
        if (UIManager.Instance != null)
            UIManager.Instance.UpdateNextFruit(nextFruitType);
    }

    public FruitType GetNextFruitType() => nextFruitType;

    public void SetCanDrop(bool value) => canDrop = value;

    private void HandleKeyInput()
    {
        if (Input.GetKeyDown(KeyCode.Space) && canDrop)
        {
            Vector3 centerPos = dropZone.position;
            centerPos.y = dropHeight;
            SpawnFruitAtPosition(centerPos, nextFruitType);
            SetNextFruitType(SelectRandomDroppableFruit());
        }
    }
}
