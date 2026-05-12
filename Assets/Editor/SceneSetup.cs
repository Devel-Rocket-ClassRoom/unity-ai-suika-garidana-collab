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
        CreateGameManager(gameOverLine);
        CreateFruitSpawner(container);
        CreateUI();
        SetupMainCamera();

        EditorSceneManager.SaveScene(scene);
        Debug.Log("✅ 씬 자동 설정이 완료되었습니다!");
        Debug.Log("Play 버튼을 눌러 게임을 테스트하세요.");
    }

    [MenuItem("Tools/Suika Game/Setup Scene for Issue #4")]
    public static void SetupSceneForIssue4()
    {
        Debug.Log("이슈 #4 - 물리 충돌 및 멈춤 구현 설정을 시작합니다...");

        SetupScene();

        CreateFloor();
        CreateWalls();

        Scene scene = EditorSceneManager.GetActiveScene();
        EditorSceneManager.SaveScene(scene);
        Debug.Log("✅ 이슈 #4 설정이 완료되었습니다!");
    }

    [MenuItem("Tools/Suika Game/Setup Scene for Issue #5 & #6")]
    public static void SetupSceneForIssue5And6()
    {
        Debug.Log("이슈 #5/#6 - 머지 및 게임오버 설정을 시작합니다...");

        SetupSceneForIssue4();

        // GameManager에 FruitPrefab 연결
        GameManager gm = Object.FindObjectOfType<GameManager>();
        if (gm != null)
        {
            GameObject fruitPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/FruitPrefab.prefab");
            SerializedObject so = new SerializedObject(gm);
            so.FindProperty("fruitPrefab").objectReferenceValue = fruitPrefab;
            so.ApplyModifiedProperties();
            Debug.Log("✅ GameManager에 FruitPrefab 연결 완료");
        }

        Scene scene = EditorSceneManager.GetActiveScene();
        EditorSceneManager.SaveScene(scene);
        Debug.Log("✅ 이슈 #5/#6 설정이 완료되었습니다!");
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

        // BoxCollider2D를 추가하지 않음 — Floor/LeftWall/RightWall이 물리 경계를 담당
        // 닫힌 박스 콜라이더를 추가하면 과일이 컨테이너 상단에 막혀 게임오버가 즉시 발생함
        SpriteRenderer spriteRenderer = container.AddComponent<SpriteRenderer>();
        spriteRenderer.color = new Color(1f, 0.98f, 0.8f, 1f);

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

        GameObject fruitPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/FruitPrefab.prefab");

        SerializedObject so = new SerializedObject(gameManager);
        so.FindProperty("containerTop").objectReferenceValue = gameOverLine.transform;
        so.FindProperty("fruitPrefab").objectReferenceValue = fruitPrefab;
        so.ApplyModifiedProperties();

        Debug.Log("✅ GameManager 생성 완료 (FruitPrefab 포함)");
        return gameManagerObj;
    }

    /// <summary>
    /// Canvas, 점수 텍스트, 다음 과일 이미지, 게임오버 패널을 생성한다
    /// </summary>
    private static void CreateUI()
    {
        // 기존 Canvas 재활용 또는 새로 생성
        Canvas canvas = Object.FindObjectOfType<Canvas>();
        GameObject canvasObj;
        if (canvas == null)
        {
            canvasObj = new GameObject("Canvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<UnityEngine.UI.CanvasScaler>();
            canvasObj.AddComponent<UnityEngine.UI.GraphicRaycaster>();
        }
        else
        {
            canvasObj = canvas.gameObject;
        }

        // UIManager 연결
        UIManager uiManager = canvasObj.GetComponent<UIManager>();
        if (uiManager == null)
            uiManager = canvasObj.AddComponent<UIManager>();

        // 점수 텍스트
        GameObject scoreObj = new GameObject("ScoreText");
        scoreObj.transform.SetParent(canvasObj.transform, false);
        UnityEngine.UI.Text scoreText = scoreObj.AddComponent<UnityEngine.UI.Text>();
        scoreText.text = "SCORE: 0";
        scoreText.fontSize = 28;
        scoreText.color = Color.black;
        scoreText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        scoreText.alignment = TextAnchor.UpperLeft;
        RectTransform scoreTf = scoreObj.GetComponent<RectTransform>();
        scoreTf.anchorMin = new Vector2(0, 1);
        scoreTf.anchorMax = new Vector2(0, 1);
        scoreTf.pivot = new Vector2(0, 1);
        scoreTf.anchoredPosition = new Vector2(20, -20);
        scoreTf.sizeDelta = new Vector2(220, 50);

        // 다음 과일 이미지
        GameObject nextFruitObj = new GameObject("NextFruitImage");
        nextFruitObj.transform.SetParent(canvasObj.transform, false);
        UnityEngine.UI.Image nextFruitImg = nextFruitObj.AddComponent<UnityEngine.UI.Image>();
        nextFruitImg.color = Color.white;
        RectTransform nextFruitTf = nextFruitObj.GetComponent<RectTransform>();
        nextFruitTf.anchorMin = new Vector2(1, 1);
        nextFruitTf.anchorMax = new Vector2(1, 1);
        nextFruitTf.pivot = new Vector2(1, 1);
        nextFruitTf.anchoredPosition = new Vector2(-20, -20);
        nextFruitTf.sizeDelta = new Vector2(80, 80);

        // 게임오버 패널
        GameObject gameOverPanel = new GameObject("GameOverPanel");
        gameOverPanel.transform.SetParent(canvasObj.transform, false);
        UnityEngine.UI.Image panelBg = gameOverPanel.AddComponent<UnityEngine.UI.Image>();
        panelBg.color = new Color(0, 0, 0, 0.75f);
        RectTransform panelTf = gameOverPanel.GetComponent<RectTransform>();
        panelTf.anchorMin = Vector2.zero;
        panelTf.anchorMax = Vector2.one;
        panelTf.offsetMin = Vector2.zero;
        panelTf.offsetMax = Vector2.zero;

        // 게임오버 타이틀
        GameObject titleObj = new GameObject("GameOverTitle");
        titleObj.transform.SetParent(gameOverPanel.transform, false);
        UnityEngine.UI.Text titleText = titleObj.AddComponent<UnityEngine.UI.Text>();
        titleText.text = "GAME OVER";
        titleText.fontSize = 52;
        titleText.color = Color.white;
        titleText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        titleText.alignment = TextAnchor.MiddleCenter;
        RectTransform titleTf = titleObj.GetComponent<RectTransform>();
        titleTf.anchorMin = new Vector2(0.5f, 0.5f);
        titleTf.anchorMax = new Vector2(0.5f, 0.5f);
        titleTf.pivot = new Vector2(0.5f, 0.5f);
        titleTf.anchoredPosition = new Vector2(0, 80);
        titleTf.sizeDelta = new Vector2(400, 70);

        // 최종 점수 텍스트
        GameObject finalScoreObj = new GameObject("FinalScoreText");
        finalScoreObj.transform.SetParent(gameOverPanel.transform, false);
        UnityEngine.UI.Text finalScoreText = finalScoreObj.AddComponent<UnityEngine.UI.Text>();
        finalScoreText.text = "최종 점수: 0";
        finalScoreText.fontSize = 34;
        finalScoreText.color = Color.yellow;
        finalScoreText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        finalScoreText.alignment = TextAnchor.MiddleCenter;
        RectTransform finalScoreTf = finalScoreObj.GetComponent<RectTransform>();
        finalScoreTf.anchorMin = new Vector2(0.5f, 0.5f);
        finalScoreTf.anchorMax = new Vector2(0.5f, 0.5f);
        finalScoreTf.pivot = new Vector2(0.5f, 0.5f);
        finalScoreTf.anchoredPosition = new Vector2(0, 10);
        finalScoreTf.sizeDelta = new Vector2(400, 50);

        // 재시작 버튼
        GameObject restartBtn = new GameObject("RestartButton");
        restartBtn.transform.SetParent(gameOverPanel.transform, false);
        UnityEngine.UI.Button btnComp = restartBtn.AddComponent<UnityEngine.UI.Button>();
        UnityEngine.UI.Image btnImg = restartBtn.AddComponent<UnityEngine.UI.Image>();
        btnImg.color = new Color(0.2f, 0.8f, 0.2f, 1f);
        RectTransform btnTf = restartBtn.GetComponent<RectTransform>();
        btnTf.anchorMin = new Vector2(0.5f, 0.5f);
        btnTf.anchorMax = new Vector2(0.5f, 0.5f);
        btnTf.pivot = new Vector2(0.5f, 0.5f);
        btnTf.anchoredPosition = new Vector2(0, -70);
        btnTf.sizeDelta = new Vector2(220, 60);

        GameObject btnTextObj = new GameObject("Text");
        btnTextObj.transform.SetParent(restartBtn.transform, false);
        UnityEngine.UI.Text btnText = btnTextObj.AddComponent<UnityEngine.UI.Text>();
        btnText.text = "다시 시작";
        btnText.fontSize = 28;
        btnText.color = Color.white;
        btnText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        btnText.alignment = TextAnchor.MiddleCenter;
        RectTransform btnTextTf = btnTextObj.GetComponent<RectTransform>();
        btnTextTf.anchorMin = Vector2.zero;
        btnTextTf.anchorMax = Vector2.one;
        btnTextTf.offsetMin = Vector2.zero;
        btnTextTf.offsetMax = Vector2.zero;

        // 버튼 클릭 이벤트 연결 (RestartGame)
        GameManager gm = Object.FindObjectOfType<GameManager>();
        if (gm != null)
        {
            UnityEditor.Events.UnityEventTools.AddPersistentListener(
                btnComp.onClick,
                gm.RestartGame
            );
        }

        gameOverPanel.SetActive(false);

        // UIManager 필드 연결
        SerializedObject soUI = new SerializedObject(uiManager);
        soUI.FindProperty("scoreText").objectReferenceValue = scoreText;
        soUI.FindProperty("nextFruitImage").objectReferenceValue = nextFruitImg;
        soUI.FindProperty("gameOverPanel").objectReferenceValue = gameOverPanel;
        soUI.FindProperty("finalScoreText").objectReferenceValue = finalScoreText;
        soUI.ApplyModifiedProperties();

        Debug.Log("✅ UI (점수·다음과일·게임오버 패널) 생성 완료");
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
        so.FindProperty("dropHeight").floatValue = 7.5f; // 게임오버 라인(6.5) 바로 위에서 생성
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
