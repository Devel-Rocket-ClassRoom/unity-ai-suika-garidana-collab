using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField]
    private Text scoreText;

    [SerializeField]
    private Image nextFruitImage;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void UpdateScore(int score)
    {
        if (scoreText != null)
        {
            scoreText.text = $"SCORE: {score}";
        }
    }

    public void UpdateNextFruit(FruitType type)
    {
        if (nextFruitImage != null)
        {
            FruitData data = FruitDatabase.GetFruitData(type);
            if (data != null && data.sprite != null)
            {
                nextFruitImage.sprite = data.sprite;
                nextFruitImage.enabled = true;
            }
            else
            {
                nextFruitImage.enabled = false;
            }
        }
    }
}
