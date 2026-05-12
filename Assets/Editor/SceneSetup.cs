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

        GameObject fruitPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/FruitPrefab.prefab");

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
}
#endif
