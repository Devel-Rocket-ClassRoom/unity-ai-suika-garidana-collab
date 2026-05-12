using UnityEngine;

/// <summary>
/// 과일의 종류를 나타내는 열거형
/// </summary>
public enum FruitType
{
    Cherry = 1,      // 체리
    Strawberry = 2,  // 딸기
    Grape = 3,       // 포도
    Mandarin = 4,    // 한라봉
    Persimmon = 5,   // 감
    Apple = 6,       // 사과
    Pear = 7,        // 배
    Peach = 8,       // 복숭아
    Pineapple = 9,   // 파인애플
    Melon = 10,      // 멜론
    Watermelon = 11  // 수박
}

/// <summary>
/// 과일의 속성 데이터
/// </summary>
[System.Serializable]
public class FruitData
{
    public FruitType type;
    public string name;
    public float radius;           // 반지름 (기본 크기 기준)
    public int score;              // 머지 시 획득 점수
    public bool canDrop;           // 직접 떨어뜨릴 수 있는지 여부
    public Color color;            // 과일의 색상

    public FruitData(FruitType type, string name, float radius, int score, bool canDrop, Color color)
    {
        this.type = type;
        this.name = name;
        this.radius = radius;
        this.score = score;
        this.canDrop = canDrop;
        this.color = color;
    }
}

/// <summary>
/// 과일 데이터를 관리하는 static 클래스
/// </summary>
public static class FruitDatabase
{
    private static FruitData[] fruitDatas;

    static FruitDatabase()
    {
        // 과일 데이터 초기화 (GDD.md 기준)
        float baseRadius = 1.0f;
        float sizeMultiplier = 1.25f;

        fruitDatas = new FruitData[11]
        {
            new FruitData(FruitType.Cherry, "체리", baseRadius * Mathf.Pow(sizeMultiplier, 0), 10, true, new Color(0.914f, 0.118f, 0.388f)), // #E91E63
            new FruitData(FruitType.Strawberry, "딸기", baseRadius * Mathf.Pow(sizeMultiplier, 1), 20, true, new Color(1f, 0.420f, 0.420f)), // #FF6B6B
            new FruitData(FruitType.Grape, "포도", baseRadius * Mathf.Pow(sizeMultiplier, 2), 30, true, new Color(0.612f, 0.149f, 0.690f)), // #9C27B0
            new FruitData(FruitType.Mandarin, "한라봉", baseRadius * Mathf.Pow(sizeMultiplier, 3), 40, true, new Color(1f, 0.596f, 0f)), // #FF9800
            new FruitData(FruitType.Persimmon, "감", baseRadius * Mathf.Pow(sizeMultiplier, 4), 50, true, new Color(1f, 0.435f, 0f)), // #FF6F00
            new FruitData(FruitType.Apple, "사과", baseRadius * Mathf.Pow(sizeMultiplier, 5), 60, false, new Color(0.302f, 0.686f, 0.314f)), // #4CAF50
            new FruitData(FruitType.Pear, "배", baseRadius * Mathf.Pow(sizeMultiplier, 6), 70, false, new Color(0.545f, 0.765f, 0.290f)), // #8BC34A
            new FruitData(FruitType.Peach, "복숭아", baseRadius * Mathf.Pow(sizeMultiplier, 7), 80, false, new Color(1f, 0.670f, 0.569f)), // #FFAB91
            new FruitData(FruitType.Pineapple, "파인애플", baseRadius * Mathf.Pow(sizeMultiplier, 8), 90, false, new Color(1f, 0.843f, 0f)), // #FFD700
            new FruitData(FruitType.Melon, "멜론", baseRadius * Mathf.Pow(sizeMultiplier, 9), 100, false, new Color(0f, 0.792f, 0.325f)), // #00C853
            new FruitData(FruitType.Watermelon, "수박", baseRadius * Mathf.Pow(sizeMultiplier, 10), 110, false, new Color(0.898f, 0.224f, 0.207f)) // #E53935
        };
    }

    /// <summary>
    /// 과일 타입으로 과일 데이터를 얻는다
    /// </summary>
    public static FruitData GetFruitData(FruitType type)
    {
        if (fruitDatas == null) return null;
        return fruitDatas[(int)type - 1];
    }

    /// <summary>
    /// 다음 단계의 과일 타입을 반환한다
    /// </summary>
    public static FruitType GetNextFruitType(FruitType type)
    {
        if (type >= FruitType.Watermelon)
            return FruitType.Watermelon;
        return (FruitType)((int)type + 1);
    }
}
