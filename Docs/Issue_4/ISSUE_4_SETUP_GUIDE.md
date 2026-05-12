# 이슈 #4 - 물리 충돌 및 멈춤 구현 설정 가이드

이 가이드는 Unity 에디터에서 SampleScene을 설정하여 이슈 #4의 물리 충돌 및 멈춤 기능을 완성하는 방법을 설명합니다.

## 수정된 스크립트 파일

다음 C# 스크립트 파일들이 수정되었습니다:

- **Fruit.cs** - Rigidbody2D 물리 설정 개선 및 PhysicsMaterial 설정
- **FruitCollisionHandler.cs** - 과일 간 충돌 처리 및 반발력 설정 추가
- **SceneSetup.cs** (Editor 스크립트) - 바닥 및 벽 자동 생성 기능 추가

---

## 이슈 #4 완료 조건

### ✅ 바닥(Floor) Collider 생성
- BoxCollider2D를 가진 Floor 게임 객체 생성
- Y 위치: -6.2 (컨테이너 하단)
- 크기: 10.2 × 0.4
- 반발력(Bounciness): 0.2, 마찰력(Friction): 0.5

### ✅ 좌/우 벽(Wall) Collider 생성
- 왼쪽 벽(LeftWall): X = -5.2
- 오른쪽 벽(RightWall): X = 5.2
- 크기: 0.4 × 12.4
- 반발력(Bounciness): 0.3, 마찰력(Friction): 0.3

### ✅ 과일 간 충돌 무시 설정
- Circle Collider2D의 isTrigger = False (물리 충돌 활성화)
- FruitCollisionHandler에서 같은 종류 과일만 머지 처리
- 머지 딜레이: 0.2초 (중복 머지 방지)

### ✅ 물리 튜닝
- Rigidbody2D 질량: 과일 크기(radius)에 따라 설정
- Drag: 0 (직선 운동 저항 제거)
- Angular Drag: 0.05 (회전 저항 최소화)
- Collision Detection: Continuous (충돌 감지 정확도)
- Rotation: FreezeRotation (회전 고정)

---

## Unity 씬 설정 단계

### 1단계: 자동 설정 실행 (권장)

Unity 에디터에서 다음 순서대로 진행하세요:

1. `Assets/Scenes/SampleScene.unity` 열기
2. 메뉴에서 **Tools** → **Suika Game** → **Setup Scene for Issue #4** 클릭
3. 콘솔에 다음 메시지 확인:
   ```
   ✅ 이슈 #4 설정이 완료되었습니다!
   ```

### 2단계: 테스트

1. **Play 버튼** 클릭
2. 마우스를 클릭하여 과일 생성
3. 과일이 떨어져서 다음을 확인:
   - 바닥에 닿으면 튕겨나감
   - 벽에 충돌하면 튕겨나감
   - 자연스럽게 멈춤
4. 같은 종류 과일 2개를 충돌시켜 머지 확인

---

## 수동 설정 방법

자동 설정이 작동하지 않는 경우, 다음 단계를 따르세요:

### Floor 생성

1. Hierarchy에서 우클릭 → `Create Empty`
2. 게임 객체 이름을 `Floor`로 변경
3. Inspector에서:
   - **Position**: X=0, Y=-6.2, Z=0
   - **Scale**: X=1, Y=1, Z=1
4. `Add Component` 클릭 → `Box Collider 2D` 추가
5. Box Collider 2D 설정:
   - **Size**: X=10.2, Y=0.4
   - **Offset**: X=0, Y=0
   - **Is Trigger**: False (체크 해제)

### LeftWall 생성

1. Hierarchy에서 우클릭 → `Create Empty`
2. 게임 객체 이름을 `LeftWall`로 변경
3. Inspector에서:
   - **Position**: X=-5.2, Y=0, Z=0
   - **Scale**: X=1, Y=1, Z=1
4. `Add Component` 클릭 → `Box Collider 2D` 추가
5. Box Collider 2D 설정:
   - **Size**: X=0.4, Y=12.4
   - **Offset**: X=0, Y=0
   - **Is Trigger**: False (체크 해제)

### RightWall 생성

1. Hierarchy에서 우클릭 → `Create Empty`
2. 게임 객체 이름을 `RightWall`로 변경
3. Inspector에서:
   - **Position**: X=5.2, Y=0, Z=0
   - **Scale**: X=1, Y=1, Z=1
4. `Add Component` 클릭 → `Box Collider 2D` 추가
5. Box Collider 2D 설정:
   - **Size**: X=0.4, Y=12.4
   - **Offset**: X=0, Y=0
   - **Is Trigger**: False (체크 해제)

### PhysicsMaterial2D 설정

Floor와 Walls에 반발력과 마찰력을 설정하려면:

1. Project 폴더에서 우클릭 → `Create` → `2D Physics Material`
2. `FloorPhysicsMaterial` 이름으로 생성
3. Inspector에서:
   - **Friction**: 0.5
   - **Bounciness**: 0.2
4. Floor의 Box Collider 2D에서 **Material** 필드에 드래그

5. 같은 방식으로 `WallPhysicsMaterial` 생성
   - **Friction**: 0.3
   - **Bounciness**: 0.3
6. LeftWall과 RightWall의 Box Collider 2D에 적용

---

## Physics 설정 설명

### Rigidbody2D 설정
- **Body Type**: Dynamic (중력 적용)
- **Gravity Scale**: 1 (일반적인 중력)
- **Mass**: 과일 크기에 따라 동적 설정
- **Drag**: 0 (공기 저항 없음)
- **Angular Drag**: 0.05 (회전 저항 최소화)
- **Collision Detection**: Continuous (정확한 충돌 감지)
- **Constraints**: Rotation Z 고정 (회전 방지)

### Circle Collider 2D 설정
- **Radius**: FruitData에 따라 설정
- **Is Trigger**: False (물리 충돌 활성화)

### PhysicsMaterial2D 설정
- **Friction**: 충돌 후 미끄럼 정도 조정
  - Floor: 0.5 (바닥에서 멈춤)
  - Walls: 0.3 (벽에서 천천히 떨어짐)
- **Bounciness**: 반발력 조정
  - Floor: 0.2 (낮은 반발)
  - Walls: 0.3 (약간의 반발)

---

## 문제 해결

### 과일이 떨어지지 않는 경우
- Floor의 Y 위치가 -6.2인지 확인
- Rigidbody2D의 Body Type이 Dynamic인지 확인
- Gravity Scale이 1로 설정되었는지 확인

### 과일이 바닥을 뚫고 나가는 경우
- Floor의 Collider가 활성화되었는지 확인
- Floor의 Collider가 Is Trigger = False인지 확인
- 과일과 Floor의 콜라이더 크기가 겹치는지 확인

### 과일이 벽을 뚫고 나가는 경우
- LeftWall과 RightWall의 X 위치 확인
- 벽의 크기가 충분한지 확인
- 벽의 Collider가 활성화되었는지 확인

### 과일이 충돌 후 계속 떨어지는 경우
- Rigidbody2D의 Angular Drag가 너무 낮지 않은지 확인
- PhysicsMaterial2D의 Friction 값이 충분한지 확인 (0.3 이상 권장)

---

## 다음 단계

이슈 #4가 완료되면 다음을 진행하세요:

1. **이슈 #5**: 같은 종류 과일 머지 기능 구현
2. **이슈 #6**: 게임오버 라인 처리 구현
3. **미션 1**: 점수 시스템, UI, 게임 오버 처리 완성

---

**문서 작성일**: 2026-05-12  
**관련 이슈**: #4 물리 충돌 및 멈춤 구현
