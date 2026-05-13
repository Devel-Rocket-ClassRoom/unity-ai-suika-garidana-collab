using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 게임 전체 상태를 관리하는 클래스
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField]
    private FruitSpawner fruitSpawner;

    [SerializeField]
    private Transform containerTop; // 게임 오버 라인

    [SerializeField]
    private GameObject fruitPrefab; // FruitCollisionHandler에서 머지 시 참조

    [SerializeField]
    private float gameOverCheckInterval = 0.5f;

    [SerializeField]
    private float gameOverDelay = 1f; // 게임오버 라인 위에 머무는 시간 임계값 (초)

    private int score = 0;
    private float gameOverCheckTimer = 0f;
    private bool isGameOver = false;
    private float aboveLineStartTime = -1f; // 과일이 라인 위에 처음 올라간 시간 (-1 = 없음)

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        if (fruitSpawner == null)
            fruitSpawner = FindObjectOfType<FruitSpawner>();

        if (containerTop == null)
            Debug.LogWarning("ContainerTop이 할당되지 않았습니다!");

        if (fruitPrefab == null)
            Debug.LogWarning("FruitPrefab이 GameManager에 할당되지 않았습니다!");

        InitializeWallSprites();
    }

    private void InitializeWallSprites()
    {
        GameObject[] walls = { GameObject.Find("LeftWall"), GameObject.Find("RightWall"), GameObject.Find("Floor") };

        foreach (GameObject wall in walls)
        {
            if (wall != null)
            {
                SpriteRenderer sr = wall.GetComponent<SpriteRenderer>();
                if (sr != null && sr.sprite == null)
                {
                    sr.sprite = CreateSimpleSprite();
                    Debug.Log($"벽 스프라이트 생성: {wall.name}");
                }
            }
            else
            {
                Debug.LogWarning($"벽 오브젝트를 찾을 수 없습니다: {wall?.name}");
            }
        }
    }

    private Sprite CreateSimpleSprite()
    {
        Texture2D texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
        texture.SetPixels(new Color[] { Color.white, Color.white, Color.white, Color.white });
        texture.Apply();
        return Sprite.Create(texture, new Rect(0, 0, 2, 2), new Vector2(0.5f, 0.5f));
    }

    private void Update()
    {
        if (!isGameOver)
        {
            gameOverCheckTimer += Time.deltaTime;
            if (gameOverCheckTimer >= gameOverCheckInterval)
            {
                CheckGameOverCondition();
                gameOverCheckTimer = 0f;
            }
        }
    }

    /// <summary>
    /// 게임오버 라인 위에 과일이 gameOverDelay 초 이상 머물면 게임 오버 처리
    /// </summary>
    private void CheckGameOverCondition()
    {
        if (containerTop == null)
            return;

        bool anyAboveLine = false;
        Fruit[] allFruits = FindObjectsOfType<Fruit>();
        foreach (Fruit fruit in allFruits)
        {
            if (fruit.transform.position.y <= containerTop.position.y)
                continue;

            // 빠르게 낙하 중인 과일(방금 생성된 과일)은 판정에서 제외
            Rigidbody2D rb = fruit.GetComponent<Rigidbody2D>();
            if (rb != null && rb.linearVelocity.y < -0.5f)
                continue;

            anyAboveLine = true;
            break;
        }

        if (anyAboveLine)
        {
            if (aboveLineStartTime < 0f)
                aboveLineStartTime = Time.time;
            else if (Time.time - aboveLineStartTime >= gameOverDelay)
                GameOver();
        }
        else
        {
            aboveLineStartTime = -1f;
        }
    }

    public void AddScore(int points)
    {
        score += points;
        if (UIManager.Instance != null)
            UIManager.Instance.UpdateScore(score);
        Debug.Log($"점수 추가: +{points}, 현재 점수: {score}");
    }

    public int GetScore() => score;

    /// <summary>
    /// FruitCollisionHandler에서 머지 시 사용할 프리팹을 반환한다
    /// </summary>
    public GameObject GetFruitPrefab() => fruitPrefab;

    public void GameOver()
    {
        if (isGameOver)
            return;

        isGameOver = true;
        Debug.Log("게임 오버! 최종 점수: " + score);

        if (fruitSpawner != null)
            fruitSpawner.SetCanDrop(false);

        if (UIManager.Instance != null)
            UIManager.Instance.ShowGameOver(score);
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public bool IsGameOver() => isGameOver;
}
