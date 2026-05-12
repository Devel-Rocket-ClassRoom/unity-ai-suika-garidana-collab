# Issue #5 — 같은 종류 과일 머지 기능 구현

## 완료 조건 (원본 이슈 기준)

- [x] `OnCollisionEnter2D`로 같은 종류 과일 충돌 감지
- [x] 충돌한 두 과일 제거
- [x] 다음 단계 과일 생성 (두 과일 중간 위치)
- [x] 수박(최종 단계) 머지 방지

---

## 변경된 파일

### `Assets/Scripts/FruitCollisionHandler.cs`

#### 문제
`fruitPrefab` 필드가 `[SerializeField]`로 선언되어 있어 Inspector에서 직접 할당해야 했음.
프리팹 인스턴스마다 자동 연결이 안 되어 `Instantiate(fruitPrefab, ...)` 호출 시 `null` 참조 에러 발생.

#### 수정 내용

| 항목 | 변경 전 | 변경 후 |
|---|---|---|
| `fruitPrefab` 참조 방식 | `[SerializeField] private GameObject fruitPrefab` | `GameManager.Instance.GetFruitPrefab()` 으로 런타임 조회 |
| 수박 머지 | 수박끼리 충돌하면 `GetNextFruitType`이 수박을 다시 반환 → 무한 루프 위험 | `OnCollisionEnter2D` 진입 시 `FruitType.Watermelon` 이면 즉시 return |
| 머지 순서 | 삭제 → 생성 | `MarkAsMerged` → 점수 추가 → `Destroy` → `Instantiate` 순서로 정렬 (삭제 후 참조 오류 방지) |

### `Assets/Scripts/GameManager.cs`

#### 추가 내용
- `[SerializeField] private GameObject fruitPrefab` 필드 추가
- `public GameObject GetFruitPrefab()` 메서드 추가
  → `FruitCollisionHandler`가 씬 내 단일 프리팹 레퍼런스를 공유

### `Assets/Editor/SceneSetup.cs`

#### 수정 내용
- `CreateGameManager()`: `fruitPrefab` 필드를 `Assets/FruitPrefab.prefab`으로 자동 연결
- `CreateUI()`: 메서드 추가 — Canvas, 점수 텍스트, 다음 과일 이미지 자동 생성
- `SetupScene()`: `CreateUI()` 호출 추가
- 새 메뉴 항목 `Tools > Suika Game > Setup Scene for Issue #5 & #6` 추가

---

## 동작 흐름

```
과일 A (Cherry) ─ OnCollisionEnter2D ─▶ 과일 B (Cherry)?
                                          │ 같은 종류 && mergeDelay 경과
                                          ▼
                              MarkAsMerged(A, B)
                              AddScore(딸기 점수)
                              Destroy(A), Destroy(B)
                              Instantiate(FruitPrefab) → Initialize(Strawberry)
```

---

## 씬 설정 방법

Unity 에디터에서:
```
Tools > Suika Game > Setup Scene for Issue #5 & #6
```

또는 수동 설정:
1. `GameManager` 게임 오브젝트 선택
2. Inspector → `Fruit Prefab` 필드에 `Assets/FruitPrefab.prefab` 드래그
