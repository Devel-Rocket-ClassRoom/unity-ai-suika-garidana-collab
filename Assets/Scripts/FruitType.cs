using UnityEngine;

/// <summary>
/// 과일의 종류를 나타내는 열거형
/// </summary>
public enum FruitType
{
    Cherry = 1, // 체리
    Strawberry = 2, // 딸기
    Grape = 3, // 포도
    Mandarin = 4, // 한라봉
    Persimmon = 5, // 감
    Apple = 6, // 사과
    Pear = 7, // 배
    Peach = 8, // 복숭아
    Pineapple = 9, // 파인애플
    Melon = 10, // 멜론
    Watermelon = 11, // 수박
}

/// <summary>
/// 과일의 속성 데이터
/// </summary>
[System.Serializable]
public class FruitData
{
    public FruitType type;
    public string name;
    public float radius; // 반지름 (기본 크기 기준)
    public float spriteLocalRadius; // 스프라이트 콘텐츠 실측 반경 (로컬 공간, PPU=100 기준)
    public int score; // 머지 시 획득 점수
    public bool canDrop; // 직접 떨어뜨릴 수 있는지 여부
    public Color color; // 과일의 색상
    public Sprite sprite; // 과일의 스프라이트

    public FruitData(
        FruitType type,
        string name,
        float radius,
        float spriteLocalRadius,
        int score,
        bool canDrop,
        Color color,
        Sprite sprite = null
    )
    {
        this.type = type;
        this.name = name;
        this.radius = radius;
        this.spriteLocalRadius = spriteLocalRadius;
        this.score = score;
        this.canDrop = canDrop;
        this.color = color;
        this.sprite = sprite;
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
        // baseRadius=0.25, multiplier=1.32 → 체리 지름 0.5, 수박 지름 ≈8.0 (컨테이너 너비 10의 80%)
        float baseRadius = 0.25f;
        float sizeMultiplier = 1.32f;

        // Resources/Fruits/ 에서 로드 — 에디터·빌드 모두 동작
        Sprite cherrySprite     = Resources.Load<Sprite>("Fruits/Cherry");
        Sprite strawberrySprite = Resources.Load<Sprite>("Fruits/Strawberry");
        Sprite grapeSprite      = Resources.Load<Sprite>("Fruits/Grape");
        Sprite mandarinSprite   = Resources.Load<Sprite>("Fruits/Mandarin");
        Sprite persimmonSprite  = Resources.Load<Sprite>("Fruits/Persimmon");
        Sprite appleSprite      = Resources.Load<Sprite>("Fruits/Apple");
        Sprite pearSprite       = Resources.Load<Sprite>("Fruits/Pear");
        Sprite peachSprite      = Resources.Load<Sprite>("Fruits/Peach");
        Sprite pineappleSprite  = Resources.Load<Sprite>("Fruits/Pineapple");
        Sprite melonSprite      = Resources.Load<Sprite>("Fruits/Melon");
        Sprite watermelonSprite = Resources.Load<Sprite>("Fruits/Watermelon");

        // spriteLocalRadius: 픽셀 분석으로 측정한 콘텐츠 min(w,h)/2 / PPU(100)
        // → sprite.bounds.extents(항상 5.12) 대신 실제 과일 이미지 크기를 반영
        fruitDatas = new FruitData[11]
        {
            new FruitData(
                FruitType.Cherry,
                "체리",
                baseRadius * Mathf.Pow(sizeMultiplier, 0),
                2.88f,
                10,
                true,
                new Color(0.914f, 0.118f, 0.388f),
                cherrySprite
            ),
            new FruitData(
                FruitType.Strawberry,
                "딸기",
                baseRadius * Mathf.Pow(sizeMultiplier, 1),
                3.14f,
                20,
                true,
                new Color(1f, 0.420f, 0.420f),
                strawberrySprite
            ),
            new FruitData(
                FruitType.Grape,
                "포도",
                baseRadius * Mathf.Pow(sizeMultiplier, 2),
                2.64f,
                30,
                true,
                new Color(0.612f, 0.149f, 0.690f),
                grapeSprite
            ),
            new FruitData(
                FruitType.Mandarin,
                "한라봉",
                baseRadius * Mathf.Pow(sizeMultiplier, 3),
                3.41f,
                40,
                true,
                new Color(1f, 0.596f, 0f),
                mandarinSprite
            ),
            new FruitData(
                FruitType.Persimmon,
                "감",
                baseRadius * Mathf.Pow(sizeMultiplier, 4),
                3.12f,
                50,
                true,
                new Color(1f, 0.435f, 0f),
                persimmonSprite
            ),
            new FruitData(
                FruitType.Apple,
                "사과",
                baseRadius * Mathf.Pow(sizeMultiplier, 5),
                3.24f,
                60,
                false,
                new Color(0.302f, 0.686f, 0.314f),
                appleSprite
            ),
            new FruitData(
                FruitType.Pear,
                "배",
                baseRadius * Mathf.Pow(sizeMultiplier, 6),
                2.75f,
                70,
                false,
                new Color(0.545f, 0.765f, 0.290f),
                pearSprite
            ),
            new FruitData(
                FruitType.Peach,
                "복숭아",
                baseRadius * Mathf.Pow(sizeMultiplier, 7),
                2.93f,
                80,
                false,
                new Color(1f, 0.670f, 0.569f),
                peachSprite
            ),
            new FruitData(
                FruitType.Pineapple,
                "파인애플",
                baseRadius * Mathf.Pow(sizeMultiplier, 8),
                2.97f,
                90,
                false,
                new Color(1f, 0.843f, 0f),
                pineappleSprite
            ),
            new FruitData(
                FruitType.Melon,
                "멜론",
                baseRadius * Mathf.Pow(sizeMultiplier, 9),
                3.82f,
                100,
                false,
                new Color(0f, 0.792f, 0.325f),
                melonSprite
            ),
            new FruitData(
                FruitType.Watermelon,
                "수박",
                baseRadius * Mathf.Pow(sizeMultiplier, 10),
                3.80f,
                110,
                false,
                new Color(0.898f, 0.224f, 0.207f),
                watermelonSprite
            ),
        };
    }

    public static FruitData GetFruitData(FruitType type)
    {
        if (fruitDatas == null)
            return null;
        return fruitDatas[(int)type - 1];
    }

    public static FruitType GetNextFruitType(FruitType type)
    {
        if (type >= FruitType.Watermelon)
            return FruitType.Watermelon;
        return (FruitType)((int)type + 1);
    }
}
