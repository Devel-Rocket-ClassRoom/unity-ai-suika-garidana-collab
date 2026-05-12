# Issue #6 — 게임오버 라인 처리 구현

## 완료 조건 (원본 이슈 기준)

- [x] 게임오버 라인 Y값 정의 (`GameOverLine` 오브젝트, Y = 6.5)
- [x] 과일이 라인 위에 있는지 지속 감지
- [x] 라인 위에 머문 시간이 임계값(기본 1초) 초과 시 게임오버
- [x] 게임오버 상태에서 입력 차단 (`FruitSpawner.SetCanDrop(false)`)
- [x] 게임오버 UI 표시 (결과 화면 + 재시작 버튼)

---

## 변경된 파일

### `Assets/Scripts/GameManager.cs`

#### 문제
- 과일이 라인 위에 올라오는 즉시 `GameOver()` 호출 → 잠깐 튀어오른 과일에도 반응
- `GameOver()` 안에 `// TODO: 게임 오버 UI 구현` 주석만 있고 실제 UI 연동 없음

#### 수정 내용

| 항목 | 변경 전 | 변경 후 |
|---|---|---|
| 게임오버 조건 | 라인 위에 과일 감지 즉시 종료 | `aboveLineStartTime`으로 타이머 추적, `gameOverDelay`(기본 1초) 초과 시 종료 |
| 타이머 리셋 | 없음 | 과일이 라인 아래로 내려오면 `aboveLineStartTime = -1f` 로 리셋 |
| UI 연동 | `// TODO` | `UIManager.Instance.ShowGameOver(score)` 호출 |

#### 추가 필드
```csharp
[SerializeField] private float gameOverDelay = 1f;   // Inspector에서 조정 가능
private float aboveLineStartTime = -1f;               // -1 = 라인 위 과일 없음
```

#### `CheckGameOverCondition()` 로직
```
매 0.5초 체크:
  ┌─ 라인 위 과일 있음?
  │    ├─ aboveLineStartTime < 0  → 타이머 시작 (현재 Time.time 기록)
  │    └─ 경과 시간 >= gameOverDelay → GameOver() 호출
  └─ 라인 위 과일 없음 → aboveLineStartTime = -1 (타이머 리셋)
```

---

### `Assets/Scripts/UIManager.cs`

#### 추가 내용

| 새 필드 | 타입 | 역할 |
|---|---|---|
| `gameOverPanel` | `GameObject` | 게임오버 오버레이 패널 (평소 비활성) |
| `finalScoreText` | `Text` | 패널 안 최종 점수 표시 |

| 새 메서드 | 동작 |
|---|---|
| `ShowGameOver(int score)` | 패널 활성화 + 최종 점수 텍스트 업데이트 |
| `HideGameOver()` | 패널 비활성화 (재시작 시 사용) |

#### `UpdateNextFruit` 개선
스프라이트가 없는 경우에도 과일 색상(`data.color`)으로 이미지를 표시하도록 폴백 추가.

---

### `Assets/Editor/SceneSetup.cs`

#### `CreateUI()` 메서드 추가
다음 UI 요소를 자동 생성하고 `UIManager` 필드에 연결:

```
Canvas (ScreenSpaceOverlay)
├── ScoreText          — 좌상단, 폰트 28px, "SCORE: 0"
├── NextFruitImage     — 우상단, 80×80
└── GameOverPanel      — 전체 화면 반투명 블랙 (기본 비활성)
    ├── GameOverTitle  — "GAME OVER", 52px, 중앙 상단
    ├── FinalScoreText — "최종 점수: 0", 34px, 노란색
    └── RestartButton  — "다시 시작", 초록색, GameManager.RestartGame() 연결
```

---

## 씬 설정 방법

Unity 에디터에서:
```
Tools > Suika Game > Setup Scene for Issue #5 & #6
```

또는 수동 설정:
1. `GameManager` Inspector → `Container Top`에 `GameOverLine` 오브젝트 연결
2. `Game Over Delay` 값 조정 (기본 1초)
3. `UIManager` Inspector → `Game Over Panel`, `Final Score Text` 연결

---

## 게임오버 흐름

```
과일이 GameOverLine(Y=6.5) 위로 올라감
  → 0.5초 간격 체크 시작
  → 1초 경과 → GameOver() 호출
      ├── FruitSpawner.SetCanDrop(false)  ← 입력 차단
      └── UIManager.ShowGameOver(score)   ← 결과 화면 표시
              └── [다시 시작] 버튼 클릭
                      └── GameManager.RestartGame()  ← 씬 리로드
```
