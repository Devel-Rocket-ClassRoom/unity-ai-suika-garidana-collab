# 🍉 이슈 #4 - 물리 충돌 및 멈춤 구현 빠른 시작

이슈 #4 (물리 충돌 및 멈춤 구현)를 빠르게 적용하는 방법입니다.

## 🚀 1단계: 자동 씬 설정

Unity 에디터에서 다음 순서대로 진행하세요:

### 1.1 SampleScene 열기
- `Assets/Scenes/SampleScene.unity` 열기

### 1.2 자동 설정 실행
1. 메뉴에서 **Tools** → **Suika Game** → **Setup Scene for Issue #4** 클릭
2. 콘솔에서 다음 메시지 확인:
   ```
   ✅ 이슈 #4 설정이 완료되었습니다!
   ```

### 1.3 테스트
- **Play 버튼** 클릭
- 마우스를 클릭하면 과일이 생성되고 떨어집니다
- 바닥과 벽에 충돌합니다
- 같은 종류 과일이 머지됩니다

---

## ✅ 이슈 #4 완료 조건 확인

다음을 테스트하여 완료 조건을 확인하세요:

### ✔️ 바닥(Floor) Collider 생성
```
과일을 떨어뜨렸을 때 바닥(Y = -6.2)에서 멈춤
```

### ✔️ 좌/우 벽(Wall) Collider 생성
```
과일이 왼쪽 벽(X = -5.2)과 오른쪽 벽(X = 5.2)에 충돌함
```

### ✔️ 과일 간 충돌 처리
```
같은 종류 과일끼리만 머지됨
다른 종류 과일끼리는 겹쳐서 떨어짐 (머지 안 됨)
```

### ✔️ 물리 튜닝
```
과일이 자연스럽게 멈춤
과일이 바닥에 닿으면 약간 튕겨남
벽에 충돌하면 천천히 떨어짐
```

---

## 🎮 테스트 시나리오

### 기본 물리 테스트
1. Play 버튼 클릭
2. 게임 화면의 중간에 마우스 클릭
3. 과일이 떨어져서 **자연스럽게 멈추는지** 확인
4. Stop 버튼 클릭

### 벽 충돌 테스트
1. Play 버튼 클릭
2. 게임 화면의 **왼쪽 끝**에 마우스 클릭
3. 과일이 왼쪽 벽에 충돌하는지 확인
4. 게임 화면의 **오른쪽 끝**에 마우스 클릭
5. 과일이 오른쪽 벽에 충돌하는지 확인
6. Stop 버튼 클릭

### 머지 테스트
1. Play 버튼 클릭
2. 같은 위치에서 **같은 종류의 과일 2개** 생성
3. 과일이 만나면 **다음 단계 과일로 합쳐지는지** 확인
4. 점수가 증가하는지 콘솔에서 확인
5. Stop 버튼 클릭

### 중력 테스트
1. Play 버튼 클릭
2. 컨테이너의 **가장 위쪽**에 과일 생성
3. 과일이 **중력에 의해 떨어지는지** 확인
4. 바닥에 닿으면 멈추는지 확인
5. Stop 버튼 클릭

---

## 📁 프로젝트 구조

```
Assets/
├── Scripts/
│   ├── Fruit.cs                    ← 물리 설정 개선
│   ├── FruitCollisionHandler.cs    ← 반발력 설정 추가
│   ├── FruitSpawner.cs
│   ├── FruitType.cs
│   └── GameManager.cs
├── Editor/
│   └── SceneSetup.cs               ← Floor/Wall 생성 추가
├── Scenes/
│   └── SampleScene.unity
└── FruitPrefab.prefab
```

---

## 📊 씬 구조 (설정 후)

```
Scene Hierarchy
├── Main Camera
├── Container                    (게임 영역 / 배경)
├── GameOverLine                 (게임 오버 판정선)
├── Floor                        (바닥 Collider) ← 이슈 #4 추가
├── LeftWall                     (왼쪽 벽 Collider) ← 이슈 #4 추가
├── RightWall                    (오른쪽 벽 Collider) ← 이슈 #4 추가
├── GameManager
├── FruitSpawner
└── [생성된 과일들]
```

---

## 🐛 문제 해결

### Q: "Setup Scene for Issue #4" 메뉴가 보이지 않습니다
**A:** 
- Unity 에디터가 완전히 로드될 때까지 기다리세요
- `Assets/Scripts/` 폴더의 모든 스크립트가 컴파일되었는지 확인하세요
- 콘솔에 컴파일 오류가 있으면 수정하세요

### Q: 과일이 바닥을 뚫고 나갑니다
**A:**
- Floor의 Collider가 활성화되었는지 확인하세요
- Floor의 Collider가 **Is Trigger = False**인지 확인하세요
- Floor의 Y 위치가 정확히 **-6.2**인지 확인하세요

### Q: 과일이 벽을 뚫고 나갑니다
**A:**
- LeftWall의 X 위치가 **-5.2**인지 확인하세요
- RightWall의 X 위치가 **5.2**인지 확인하세요
- 벽의 Collider가 활성화되었는지 확인하세요

### Q: 과일이 계속 떨어지고 멈추지 않습니다
**A:**
- Rigidbody2D의 **Gravity Scale**이 1로 설정되었는지 확인하세요
- PhysicsMaterial2D의 **Friction**이 0.3 이상인지 확인하세요
- Rigidbody2D의 **Angular Drag**가 너무 높지 않은지 확인하세요

### Q: 과일이 벽에 너무 세게 튕겨납니다
**A:**
- PhysicsMaterial2D의 **Bounciness**를 낮추세요 (0.2 ~ 0.3 권장)
- Rigidbody2D의 **Drag**를 약간 높이세요 (0.1 ~ 0.2)

---

## 📚 참고 문서

- `ISSUE_4_SETUP_GUIDE.md` - 상세한 수동 설정 가이드
- `Docs/GDD.md` - 게임 설계 문서
- `Docs/Mission.md` - 개발 미션 목표
- `Docs/Issue_3/ISSUE_3_SETUP_GUIDE.md` - 이슈 #3 설정 (선행 조건)

---

**마지막 업데이트**: 2026-05-12  
**관련 이슈**: #4 물리 충돌 및 멈춤 구현
