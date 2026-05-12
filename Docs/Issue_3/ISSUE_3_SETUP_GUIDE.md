# 이슈 #3 - 과일 드롭 기능 구현 설정 가이드

이 가이드는 Unity 에디터에서 SampleScene을 설정하여 이슈 #3의 과일 드롭 기능을 완성하는 방법을 설명합니다.

## 생성된 스크립트 파일

다음 C# 스크립트 파일들이 `Assets/Scripts/` 폴더에 생성되었습니다:

- **FruitType.cs** - 과일 타입과 데이터 정의
- **Fruit.cs** - 개별 과일 게임 객체 관리
- **FruitSpawner.cs** - 마우스 클릭으로 과일 생성
- **FruitCollisionHandler.cs** - 과일 간 충돌 감지 및 머지
- **GameManager.cs** - 게임 전체 상태 관리

## Unity 씬 설정 단계

### 1단계: 기본 게임 객체 생성

#### 1.1 Fruit 프리팹 생성

1. `Assets/` 폴더에서 우클릭 → `Create` → `2D Object` → `Circle`
2. 게임 객체 이름을 `FruitPrefab`으로 변경
3. Inspector에서 다음을 설정:
   - **Scale**: X=2, Y=2, Z=1
   - **Color**: 흰색 (1, 1, 1, 1)

#### 1.2 Fruit 프리팹에 컴포넌트 추가

1. FruitPrefab을 선택한 상태에서 Inspector의 `Add Component` 클릭
2. 다음 컴포넌트 추가:
   - `Rigidbody2D`
   - `Circle Collider 2D`
   - `Fruit` (스크립트)
   - `FruitCollisionHandler` (스크립트)

#### 1.3 Rigidbody2D 설정

1. Rigidbody2D의 설정:
   - **Body Type**: Dynamic
   - **Gravity Scale**: 1
   - **Constraints**: Rotation Z 체크 (회전 방지)
   - **Collision Detection**: Continuous

#### 1.4 Circle Collider 2D 설정

1. Circle Collider 2D의 설정:
   - **Radius**: 0.5
   - **Is Trigger**: False (체크 해제)

#### 1.5 프리팹 저장

1. FruitPrefab을 `Assets/` 폴더로 드래그하여 프리팹으로 저장
2. Hierarchy에서 FruitPrefab을 삭제

---

### 2단계: 게임 컨테이너 생성

1. Hierarchy에서 우클릭 → `2D Object` → `Sprites` → `Square`
2. 게임 객체 이름을 `Container`로 변경
3. Inspector에서 다음을 설정:
   - **Position**: X=0, Y=0, Z=0
   - **Scale**: X=10, Y=12, Z=1
   - **Color**: 밝은 노란색 (#FFFACD)

#### 2.1 Container에 Box Collider 2D 추가

1. Container를 선택한 상태에서 `Add Component` 클릭
2. `Box Collider 2D` 추가
3. 설정:
   - **Is Trigger**: False
   - **Size**: X=10, Y=12
   - **Offset**: X=0, Y=0

---

### 3단계: 게임 오버 라인 생성

1. Hierarchy에서 우클릭 → `Create Empty`
2. 게임 객체 이름을 `GameOverLine`으로 변경
3. Inspector에서 Position 설정:
   - **Position**: X=0, Y=6.5, Z=0
   - **Scale**: X=1, Y=1, Z=1

---

### 4단계: GameManager 설정

1. Hierarchy에서 우클릭 → `Create Empty`
2. 게임 객체 이름을 `GameManager`로 변경
3. Inspector의 `Add Component` 클릭
4. `GameManager` (스크립트) 추가
5. Inspector의 GameManager 컴포넌트에서:
   - **Fruit Spawner**: (아직 비워둔다)
   - **Container Top**: `GameOverLine` 드래그
   - **Game Over Check Interval**: 0.5

---

### 5단계: FruitSpawner 생성

1. Hierarchy에서 우클릭 → `Create Empty`
2. 게임 객체 이름을 `FruitSpawner`로 변경
3. Inspector의 `Add Component` 클릭
4. `FruitSpawner` (스크립트) 추가
5. Inspector의 FruitSpawner 컴포넌트에서:
   - **Fruit Prefab**: `Assets/FruitPrefab` 드래그
   - **Drop Zone**: `Container` 드래그
   - **Drop Height**: 10
   - **Container Width**: 10
   - **Initial Fruit Type**: Cherry

---

### 6단계: GameManager에 FruitSpawner 연결

1. Hierarchy에서 `GameManager`를 선택
2. Inspector의 GameManager 컴포넌트에서:
   - **Fruit Spawner**: `FruitSpawner` 드래그

---

### 7단계: 메인 카메라 설정

1. Hierarchy에서 `Main Camera`를 선택
2. Inspector에서:
   - **Position**: X=0, Y=0, Z=-10
   - **Size**: 8
   - **Near Clip Plane**: 0.3
   - **Far Clip Plane**: 1000

---

## 테스트 방법

1. Unity 에디터에서 **Play 버튼** 클릭
2. 게임 화면에서 **마우스를 클릭**하여 과일 생성
3. 과일이 물리 연산에 따라 떨어지는지 확인
4. 과일들이 컨테이너 하단에서 튕겨나가는지 확인
5. 같은 종류의 과일 두 개가 만나면 머지되는지 확인
6. 과일이 게임오버라인(Y=6.5)을 넘으면 게임이 종료되는지 확인

---

## 이슈 #3 완료 조건 확인

### ✅ 마우스 클릭으로 과일 생성
- FruitSpawner 스크립트가 `Input.GetMouseButtonDown(0)` 감지
- 마우스 위치에 따라 과일 생성

### ✅ 드롭할 과일 위치 결정
- 마우스 X좌표를 감지하여 과일 생성 위치 결정
- 컨테이너 범위 내로 자동 제한
- 스페이스바로도 중앙에 떨어뜨릴 수 있음

### ✅ Rigidbody 활성화해 물리 적용
- Fruit 프리팹에 Rigidbody2D 컴포넌트 추가
- Dynamic 바디 타입으로 중력 적용
- 컨테이너 벽과 충돌하여 튕겨나감

---

## 문제 해결

### 과일이 생성되지 않는 경우
- FruitPrefab이 프리팹으로 저장되었는지 확인
- FruitSpawner의 Fruit Prefab이 정확히 연결되었는지 확인

### 과일이 떨어지지 않는 경우
- Rigidbody2D의 Body Type이 Dynamic인지 확인
- Gravity Scale이 1로 설정되었는지 확인

### 머지가 작동하지 않는 경우
- Circle Collider 2D의 Is Trigger가 False인지 확인
- 두 과일의 FruitType이 정확히 같은지 확인

---

## 다음 단계

이슈 #3이 완료되면 다음을 진행하세요:

1. **이슈 #1 (미션 1)**: 메인 메커닉 완성
   - 점수 시스템 추가
   - 게임 오버 UI 구현
   - 게임 재시작 기능

2. **이슈 #2 (미션 2)**: 애셋 생성 및 적용
   - Unity AI Generators로 과일 스프라이트 생성
   - 스프라이트를 게임에 적용

3. **이슈 #3 (미션 3)**: UI 및 마무리
   - 점수 표시 UI
   - 다음 과일 미리보기
   - 게임 오버 화면

---

**문서 작성일**: 2026-05-12  
**관련 이슈**: #3 과일 드롭 기능 구현
