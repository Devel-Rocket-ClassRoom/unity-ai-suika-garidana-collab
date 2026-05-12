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
    private float gameOverCheckInterval = 0.5f;

    private int score = 0;
    private float gameOverCheckTimer = 0f;
    private bool isGameOver = false;

    private void Awake()
    {
        // 싱글톤 패턴
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
        {
            fruitSpawner = FindObjectOfType<FruitSpawner>();
        }

        if (containerTop == null)
        {
            Debug.LogWarning("ContainerTop이 할당되지 않았습니다!");
        }
    }

    private void Update()
    {
        if (!isGameOver)
        {
            // 일정 간격으로 게임 오버 조건 확인
            gameOverCheckTimer += Time.deltaTime;
            if (gameOverCheckTimer >= gameOverCheckInterval)
            {
                CheckGameOverCondition();
                gameOverCheckTimer = 0f;
            }
        }
    }

    /// <summary>
    /// 게임 오버 조건을 확인한다 (과일이 상단 경계를 넘었는지)
    /// </summary>
    private void CheckGameOverCondition()
    {
        if (containerTop == null)
            return;

        Fruit[] allFruits = FindObjectsOfType<Fruit>();
        foreach (Fruit fruit in allFruits)
        {
            if (fruit.transform.position.y > containerTop.position.y)
            {
                GameOver();
                return;
            }
        }
    }

    /// <summary>
    /// 점수를 추가한다
    /// </summary>
    public void AddScore(int points)
    {
        score += points;
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateScore(score);
        }
        Debug.Log($"점수 추가: +{points}, 현재 점수: {score}");
    }

    /// <summary>
    /// 현재 점수를 반환한다
    /// </summary>
    public int GetScore()
    {
        return score;
    }

    /// <summary>
    /// 게임 오버 처리
    /// </summary>
    public void GameOver()
    {
        if (isGameOver)
            return;

        isGameOver = true;
        Debug.Log("게임 오버! 최종 점수: " + score);

        if (fruitSpawner != null)
        {
            fruitSpawner.SetCanDrop(false);
        }

        // UI 표시 또는 다른 처리...
        // TODO: 게임 오버 UI 구현
    }

    /// <summary>
    /// 게임을 재시작한다
    /// </summary>
    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /// <summary>
    /// 게임이 오버되었는지 확인한다
    /// </summary>
    public bool IsGameOver()
    {
        return isGameOver;
    }
}
