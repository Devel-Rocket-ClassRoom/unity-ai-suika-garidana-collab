# Issue #8 — 과일 드롭 시 체리만 생성되는 문제

## 원인 분석

`FruitSpawner.DropFruit()`에서 드롭 후 다음 과일을 결정할 때 항상 `initialFruitType`(= Cherry)을 고정으로 넘기고 있었다.

```csharp
// 수정 전 (FruitSpawner.cs:79)
SetNextFruitType(initialFruitType); // 항상 Cherry
```

`SetNextFruitType()`에도 랜덤 선택 로직 없이 전달받은 타입을 그대로 세팅하는 구조였다.

---

## 수정 내용 — `Assets/Scripts/FruitSpawner.cs`

### 추가: 드롭 가능 과일 목록 + 가중치 테이블

```csharp
private static readonly FruitType[] droppableFruits =
{
    FruitType.Cherry,      // 30%
    FruitType.Strawberry,  // 25%
    FruitType.Grape,       // 20%
    FruitType.Mandarin,    // 15%
    FruitType.Persimmon,   // 10%
};
private static readonly float[] dropWeights = { 30f, 25f, 20f, 15f, 10f };
```

낮은 단계 과일이 더 자주 등장하는 수박게임 원작의 밸런스를 반영했다.

### 추가: `SelectRandomDroppableFruit()`

누적 가중치 방식으로 랜덤 선택:

```
전체 가중치 합 = 100
roll = Random(0, 100)
roll < 30          → Cherry
30 ≤ roll < 55     → Strawberry
55 ≤ roll < 75     → Grape
75 ≤ roll < 90     → Mandarin
90 ≤ roll < 100    → Persimmon
```

### 변경: 랜덤 선택 호출 위치 3곳

| 위치 | 수정 전 | 수정 후 |
|---|---|---|
| `Start()` | `nextFruitType = initialFruitType` | `SetNextFruitType(SelectRandomDroppableFruit())` |
| `DropFruit()` | `SetNextFruitType(initialFruitType)` | `SetNextFruitType(SelectRandomDroppableFruit())` |
| `HandleKeyInput()` Space | `SetNextFruitType(initialFruitType)` | `SetNextFruitType(SelectRandomDroppableFruit())` |

### 삭제: `initialFruitType` 필드 및 `SetNextFruitType()` canDrop 폴백 로직

`droppableFruits` 배열이 드롭 가능 과일만 담고 있으므로 불필요한 방어 로직 제거.

---

## 완료 조건 체크

- [x] 드롭할 때마다 Cherry~Persimmon 중 랜덤으로 다음 과일이 결정됨
- [x] 가중치 적용 — Cherry(30%) > Strawberry(25%) > ... > Persimmon(10%)
- [x] 게임 시작 시 첫 번째 과일도 랜덤으로 결정됨
- [x] UI 다음 과일 미리보기가 랜덤 선택 결과를 반영함
