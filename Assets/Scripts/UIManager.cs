using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField]
    private Text scoreText;

    [SerializeField]
    private Image nextFruitImage;

    [SerializeField]
    private GameObject gameOverPanel;

    [SerializeField]
    private Text finalScoreText;

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
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }

    public void UpdateScore(int score)
    {
        if (scoreText != null)
            scoreText.text = $"SCORE: {score}";
    }

    public void UpdateNextFruit(FruitType type)
    {
        if (nextFruitImage == null)
            return;

        FruitData data = FruitDatabase.GetFruitData(type);
        if (data != null && data.sprite != null)
        {
            nextFruitImage.sprite = data.sprite;
            nextFruitImage.color = Color.white;
            nextFruitImage.enabled = true;
        }
        else if (data != null)
        {
            // 스프라이트가 없으면 과일 색상으로 표시
            nextFruitImage.sprite = null;
            nextFruitImage.color = data.color;
            nextFruitImage.enabled = true;
        }
        else
        {
            nextFruitImage.enabled = false;
        }
    }

    /// <summary>
    /// 게임오버 패널을 표시하고 최종 점수를 보여준다
    /// </summary>
    public void ShowGameOver(int score)
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        if (finalScoreText != null)
            finalScoreText.text = $"최종 점수: {score}";
    }

    public void HideGameOver()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }
}
