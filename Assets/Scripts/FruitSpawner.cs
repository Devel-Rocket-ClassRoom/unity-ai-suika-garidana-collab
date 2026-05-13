using UnityEngine;

public class FruitSpawner : MonoBehaviour
{
    [SerializeField] private GameObject fruitPrefab;
    [SerializeField] private Transform dropZone;
    [SerializeField] private float dropHeight = 7.5f;
    [SerializeField] private float containerWidth = 10f;
    [SerializeField] private float containerBottom = -6f;

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

    // 드롭 가이드라인 (수직 점선)
    private LineRenderer guideLineRenderer;
    // 드롭 위치 프리뷰 (반투명 과일)
    private GameObject previewObj;
    private SpriteRenderer previewRenderer;

    private void Start()
    {
        mainCamera = Camera.main;

        if (fruitPrefab == null)
            Debug.LogError("FruitPrefab이 할당되지 않았습니다!");
        if (dropZone == null)
            Debug.LogError("DropZone(게임 컨테이너)이 할당되지 않았습니다!");

        CreateGuideLine();
        CreatePreview();

        SetNextFruitType(SelectRandomDroppableFruit());
    }

    private void CreateGuideLine()
    {
        GameObject lineObj = new GameObject("DropGuideLine");
        lineObj.transform.SetParent(transform);
        guideLineRenderer = lineObj.AddComponent<LineRenderer>();
        guideLineRenderer.positionCount = 2;
        guideLineRenderer.startWidth = 0.05f;
        guideLineRenderer.endWidth = 0.05f;
        guideLineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        guideLineRenderer.startColor = new Color(1f, 1f, 1f, 0.4f);
        guideLineRenderer.endColor = new Color(1f, 1f, 1f, 0.1f);
        guideLineRenderer.sortingOrder = 10;
        // 점선 효과: 텍스처 타일링
        guideLineRenderer.textureMode = LineTextureMode.Tile;
    }

    private void CreatePreview()
    {
        previewObj = new GameObject("DropPreview");
        previewObj.transform.SetParent(transform);
        previewRenderer = previewObj.AddComponent<SpriteRenderer>();
        previewRenderer.color = new Color(1f, 1f, 1f, 0.55f);
        previewRenderer.sortingOrder = 9;
    }

    private void Update()
    {
        UpdateDropPosition();

        if (Input.GetMouseButtonDown(0) && canDrop)
            DropFruit();

        HandleKeyInput();
    }

    private void UpdateDropPosition()
    {
        if (mainCamera == null) return;

        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 10f;
        Vector3 worldPos = mainCamera.ScreenToWorldPoint(mousePos);

        float clampedX = Mathf.Clamp(
            worldPos.x,
            dropZone.position.x - containerWidth / 2f,
            dropZone.position.x + containerWidth / 2f
        );

        // 가이드라인: dropHeight → containerBottom
        guideLineRenderer.SetPosition(0, new Vector3(clampedX, dropHeight, 0));
        guideLineRenderer.SetPosition(1, new Vector3(clampedX, containerBottom, 0));
        guideLineRenderer.enabled = canDrop;

        // 프리뷰: dropHeight 위치에 반투명 과일
        previewObj.transform.position = new Vector3(clampedX, dropHeight, 0);
        previewObj.SetActive(canDrop);
    }

    private void DropFruit()
    {
        if (mainCamera == null) return;

        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 10f;
        Vector3 worldPos = mainCamera.ScreenToWorldPoint(mousePos);

        float clampedX = Mathf.Clamp(
            worldPos.x,
            dropZone.position.x - containerWidth / 2f,
            dropZone.position.x + containerWidth / 2f
        );

        SpawnFruitAtPosition(new Vector3(clampedX, dropHeight, 0), nextFruitType);
        SetNextFruitType(SelectRandomDroppableFruit());
    }

    private void SpawnFruitAtPosition(Vector3 position, FruitType fruitType)
    {
        if (fruitPrefab == null) return;

        GameObject fruitObj = Instantiate(fruitPrefab, position, Quaternion.identity);
        fruitObj.name = FruitDatabase.GetFruitData(fruitType).name;

        Fruit fruit = fruitObj.GetComponent<Fruit>();
        if (fruit != null)
            fruit.Initialize(fruitType);
        else
            Debug.LogError("Fruit 컴포넌트를 찾을 수 없습니다!");
    }

    private FruitType SelectRandomDroppableFruit()
    {
        float totalWeight = 0f;
        foreach (float w in dropWeights) totalWeight += w;

        float roll = Random.Range(0f, totalWeight);
        float cumulative = 0f;
        for (int i = 0; i < droppableFruits.Length; i++)
        {
            cumulative += dropWeights[i];
            if (roll < cumulative) return droppableFruits[i];
        }
        return droppableFruits[0];
    }

    public void SetNextFruitType(FruitType fruitType)
    {
        nextFruitType = fruitType;

        FruitData data = FruitDatabase.GetFruitData(fruitType);
        if (previewRenderer != null && data != null)
        {
            if (data.sprite != null)
            {
                previewRenderer.sprite = data.sprite;
                previewRenderer.color = new Color(1f, 1f, 1f, 0.55f);
            }
            else
            {
                previewRenderer.sprite = null;
                Color c = data.color;
                previewRenderer.color = new Color(c.r, c.g, c.b, 0.55f);
            }

            // 프리뷰 크기를 실제 과일과 동일하게
            float localR = data.spriteLocalRadius;
            if (localR > 0f && data.sprite != null)
                previewObj.transform.localScale = Vector3.one * (data.radius / localR);
            else
                previewObj.transform.localScale = Vector3.one * data.radius * 2f;
        }

        if (UIManager.Instance != null)
            UIManager.Instance.UpdateNextFruit(nextFruitType);
    }

    public FruitType GetNextFruitType() => nextFruitType;

    public void SetCanDrop(bool value)
    {
        canDrop = value;
        if (guideLineRenderer != null) guideLineRenderer.enabled = value;
        if (previewObj != null) previewObj.SetActive(value);
    }

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
