using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;

/// <summary>
/// 자동으로 게임 씬을 설정하는 에디터 스크립트
/// 메뉴: Tools > Suika Game > Setup Scene
/// </summary>
public class SceneSetup
{
    [MenuItem("Tools/Suika Game/Setup Scene for Issue #3")]
    public static void SetupScene()
    {
        Debug.Log("씬 자동 설정을 시작합니다...");

        Scene scene = EditorSceneManager.GetActiveScene();
        GameObject[] rootObjects = scene.GetRootGameObjects();

        foreach (GameObject obj in rootObjects)
        {
            if (obj.name != "Main Camera" && obj.name != "Canvas")
            {
                Object.DestroyImmediate(obj);
            }
        }

        Debug.Log("기존 게임 객체를 제거했습니다.");

        CreateFruitPrefab();
        GameObject container = CreateContainer();
        GameObject gameOverLine = CreateGameOverLine();
        GameObject gameManagerObj = CreateGameManager(gameOverLine);
        CreateFruitSpawner(container);
        SetupMainCamera();

        EditorSceneManager.SaveScene(scene);
        Debug.Log("✅ 씬 자동 설정이 완료되었습니다!");
        Debug.Log("Play 버튼을 눌러 게임을 테스트하세요.");
    }

    [MenuItem("Tools/Suika Game/Setup Scene for Issue #4")]
    public static void SetupSceneForIssue4()
    {
        Debug.Log("이슈 #4 - 물리 충돌 및 멈춤 구현 설정을 시작합니다...");

        // 먼저 이슈 #3 설정 실행
        SetupScene();

        // 이슈 #4 추가 설정
        CreateFloor();
        CreateWalls();

        Scene scene = EditorSceneManager.GetActiveScene();
        EditorSceneManager.SaveScene(scene);
        Debug.Log("✅ 이슈 #4 설정이 완료되었습니다!");
    }

    private static void CreateFruitPrefab()
    {
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/FruitPrefab.prefab");
        if (prefab != null)
        {
            Debug.Log("FruitPrefab이 이미 존재합니다.");
            return;
        }

        GameObject fruit = new GameObject("FruitPrefab");
        fruit.AddComponent<SpriteRenderer>();
        fruit.AddComponent<Rigidbody2D>();
        fruit.AddComponent<CircleCollider2D>();
        fruit.AddComponent<Fruit>();
        fruit.AddComponent<FruitCollisionHandler>();

        string prefabPath = "Assets/FruitPrefab.prefab";
        PrefabUtility.SaveAsPrefabAsset(fruit, prefabPath);
        Object.DestroyImmediate(fruit);

        Debug.Log("✅ FruitPrefab 생성 완료");
    }

    private static GameObject CreateContainer()
    {
        GameObject container = new GameObject("Container");
        container.transform.position = Vector3.zero;

        SpriteRenderer spriteRenderer = container.AddComponent<SpriteRenderer>();
        spriteRenderer.color = new Color(1f, 0.98f, 0.8f, 1f);

        BoxCollider2D boxCollider = container.AddComponent<BoxCollider2D>();
        boxCollider.size = new Vector2(10, 12);
        boxCollider.isTrigger = false;

        Debug.Log("✅ Container 생성 완료");
        return container;
    }

    private static GameObject CreateGameOverLine()
    {
        GameObject gameOverLine = new GameObject("GameOverLine");
        gameOverLine.transform.position = new Vector3(0, 6.5f, 0);

        Debug.Log("✅ GameOverLine 생성 완료");
        return gameOverLine;
    }

    private static GameObject CreateGameManager(GameObject gameOverLine)
    {
        GameObject gameManagerObj = new GameObject("GameManager");
        GameManager gameManager = gameManagerObj.AddComponent<GameManager>();

        SerializedObject so = new SerializedObject(gameManager);
        so.FindProperty("containerTop").objectReferenceValue = gameOverLine.transform;
        so.ApplyModifiedProperties();

        Debug.Log("✅ GameManager 생성 완료");
        return gameManagerObj;
    }

    private static void CreateFruitSpawner(GameObject container)
    {
        GameObject spawnerObj = new GameObject("FruitSpawner");
        FruitSpawner spawner = spawnerObj.AddComponent<FruitSpawner>();

        GameObject fruitPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(
            "Assets/FruitPrefab.prefab"
        );

        SerializedObject so = new SerializedObject(spawner);
        so.FindProperty("fruitPrefab").objectReferenceValue = fruitPrefab;
        so.FindProperty("dropZone").objectReferenceValue = container.transform;
        so.ApplyModifiedProperties();

        Debug.Log("✅ FruitSpawner 생성 완료");
    }

    private static void SetupMainCamera()
    {
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            mainCamera.transform.position = new Vector3(0, 0, -10);
            mainCamera.orthographicSize = 8f;
            mainCamera.nearClipPlane = 0.3f;
            mainCamera.farClipPlane = 1000f;

            Debug.Log("✅ Main Camera 설정 완료");
        }
    }

    /// <summary>
    /// 바닥(Floor) Collider를 생성한다 (이슈 #4)
    /// </summary>
    private static void CreateFloor()
    {
        GameObject floor = new GameObject("Floor");
        floor.transform.position = new Vector3(0, -6.2f, 0);

        BoxCollider2D boxCollider = floor.AddComponent<BoxCollider2D>();
        boxCollider.size = new Vector2(10.2f, 0.4f);
        boxCollider.isTrigger = false;

        // 바닥용 PhysicsMaterial2D 설정
        PhysicsMaterial2D floorMaterial = new PhysicsMaterial2D();
        floorMaterial.friction = 0.5f;
        floorMaterial.bounciness = 0.2f;
        boxCollider.sharedMaterial = floorMaterial;

        // 시각적 표시 (선택사항)
        SpriteRenderer spriteRenderer = floor.AddComponent<SpriteRenderer>();
        spriteRenderer.color = new Color(0.5f, 0.3f, 0.1f, 0.3f); // 반투명 갈색
        spriteRenderer.sortingOrder = -1;

        Debug.Log("✅ Floor Collider 생성 완료");
    }

    /// <summary>
    /// 좌/우 벽(Wall) Collider를 생성한다 (이슈 #4)
    /// </summary>
    private static void CreateWalls()
    {
        // 왼쪽 벽
        GameObject leftWall = new GameObject("LeftWall");
        leftWall.transform.position = new Vector3(-5.2f, 0, 0);

        BoxCollider2D leftWallCollider = leftWall.AddComponent<BoxCollider2D>();
        leftWallCollider.size = new Vector2(0.4f, 12.4f);
        leftWallCollider.isTrigger = false;

        // 왼쪽 벽 PhysicsMaterial2D 설정
        PhysicsMaterial2D wallMaterial = new PhysicsMaterial2D();
        wallMaterial.friction = 0.3f;
        wallMaterial.bounciness = 0.3f;
        leftWallCollider.sharedMaterial = wallMaterial;

        // 시각적 표시
        SpriteRenderer leftSpriteRenderer = leftWall.AddComponent<SpriteRenderer>();
        leftSpriteRenderer.color = new Color(0.5f, 0.3f, 0.1f, 0.3f);
        leftSpriteRenderer.sortingOrder = -1;

        // 오른쪽 벽
        GameObject rightWall = new GameObject("RightWall");
        rightWall.transform.position = new Vector3(5.2f, 0, 0);

        BoxCollider2D rightWallCollider = rightWall.AddComponent<BoxCollider2D>();
        rightWallCollider.size = new Vector2(0.4f, 12.4f);
        rightWallCollider.isTrigger = false;
        rightWallCollider.sharedMaterial = wallMaterial;

        // 시각적 표시
        SpriteRenderer rightSpriteRenderer = rightWall.AddComponent<SpriteRenderer>();
        rightSpriteRenderer.color = new Color(0.5f, 0.3f, 0.1f, 0.3f);
        rightSpriteRenderer.sortingOrder = -1;

        Debug.Log("✅ Left & Right Walls 생성 완료");
    }
}
#endif
