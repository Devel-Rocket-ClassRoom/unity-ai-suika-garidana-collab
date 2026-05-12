using UnityEngine;

/// <summary>
/// 마우스 입력으로 과일을 생성하고 떨어뜨리는 클래스
/// </summary>
public class FruitSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject fruitPrefab;

    [SerializeField]
    private Transform dropZone; // 과일이 떨어질 영역 (게임 컨테이너)

    [SerializeField]
    private float dropHeight = 10f; // 과일이 생성될 높이

    [SerializeField]
    private float containerWidth = 10f; // 게임 컨테이너의 너비

    [SerializeField]
    private FruitType initialFruitType = FruitType.Cherry;

    private FruitType nextFruitType;
    private Camera mainCamera;
    private bool canDrop = true;

    private void Start()
    {
        mainCamera = Camera.main;
        nextFruitType = initialFruitType;

        if (fruitPrefab == null)
        {
            Debug.LogError("FruitPrefab이 할당되지 않았습니다!");
        }

        if (dropZone == null)
        {
            Debug.LogError("DropZone(게임 컨테이너)이 할당되지 않았습니다!");
        }
    }

    private void Update()
    {
        // 마우스 클릭 감지
        if (Input.GetMouseButtonDown(0) && canDrop)
        {
            DropFruit();
        }

        // 키 입력으로 과일 떨어뜨리기 (선택사항)
        HandleKeyInput();
    }

    /// <summary>
    /// 마우스 위치에 따라 과일을 생성하고 떨어뜨린다
    /// </summary>
    private void DropFruit()
    {
        // 마우스의 월드 좌표를 구한다
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 10f; // 카메라 앞 거리
        Vector3 worldPos = mainCamera.ScreenToWorldPoint(mousePos);

        // 마우스 X좌표를 컨테이너 범위 내로 제한한다
        float clampedX = Mathf.Clamp(
            worldPos.x,
            dropZone.position.x - containerWidth / 2,
            dropZone.position.x + containerWidth / 2
        );

        // 과일 생성 위치 설정
        Vector3 spawnPos = new Vector3(clampedX, dropHeight, 0);

        // 과일 생성
        SpawnFruitAtPosition(spawnPos, nextFruitType);

        // 다음 과일 타입 결정 (일반적으로 랜덤이지만, 현재는 고정)
        SetNextFruitType(initialFruitType);
    }

    /// <summary>
    /// 특정 위치에 과일을 생성한다
    /// </summary>
    private void SpawnFruitAtPosition(Vector3 position, FruitType fruitType)
    {
        if (fruitPrefab == null)
        {
            Debug.LogError("과일 프리팹이 할당되지 않았습니다!");
            return;
        }

        // 과일 게임 객체 생성
        GameObject fruitObj = Instantiate(fruitPrefab, position, Quaternion.identity);
        fruitObj.name = $"{FruitDatabase.GetFruitData(fruitType).name}";

        // Fruit 컴포넌트 초기화
        Fruit fruit = fruitObj.GetComponent<Fruit>();
        if (fruit != null)
        {
            fruit.Initialize(fruitType);
        }
        else
        {
            Debug.LogError("Fruit 컴포넌트를 찾을 수 없습니다!");
        }

        // Rigidbody가 활성화되었는지 확인
        Rigidbody2D rb = fruitObj.GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody2D 컴포넌트를 찾을 수 없습니다!");
        }
    }

    /// <summary>
    /// 다음 과일 타입을 설정한다
    /// </summary>
    public void SetNextFruitType(FruitType fruitType)
    {
        // 직접 드롭할 수 없는 과일인 경우 체리로 설정
        FruitData data = FruitDatabase.GetFruitData(fruitType);
        if (data != null && data.canDrop)
        {
            nextFruitType = fruitType;
        }
        else
        {
            nextFruitType = initialFruitType;
        }

        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateNextFruit(nextFruitType);
        }
    }

    /// <summary>
    /// 현재 다음 과일 타입을 반환한다
    /// </summary>
    public FruitType GetNextFruitType()
    {
        return nextFruitType;
    }

    /// <summary>
    /// 과일 드롭을 활성화/비활성화한다
    /// </summary>
    public void SetCanDrop(bool canDrop)
    {
        this.canDrop = canDrop;
    }

    /// <summary>
    /// 키 입력 처리 (선택사항)
    /// </summary>
    private void HandleKeyInput()
    {
        // 방향키로 과일 위치 조정 (선택사항)
        if (Input.GetKeyDown(KeyCode.Space) && canDrop)
        {
            // 스페이스바로 화면 중앙에 과일 떨어뜨리기
            Vector3 centerPos = dropZone.position;
            centerPos.y = dropHeight;
            SpawnFruitAtPosition(centerPos, nextFruitType);
            SetNextFruitType(initialFruitType);
        }
    }
}
