# Issue #9 — 과일 콜라이더 크기가 스프라이트와 맞지 않음

## 원인 분석

`Fruit.Initialize()`에서 `circleCollider.radius = fruitData.radius`를 설정한 후
`transform.localScale = Vector3.one * fruitData.radius * 2`로 오브젝트를 스케일했다.

Unity의 `CircleCollider2D.radius`는 **로컬 공간** 기준이므로, 스케일이 적용된 뒤
실제 월드 반경은 다음과 같이 계산된다.

```
월드 콜라이더 반경 = circleCollider.radius × localScale.x
                   = fruitData.radius × (fruitData.radius × 2)
                   = 2 × fruitData.radius²
```

| 단계 | radius | 스프라이트 월드 반경 | 콜라이더 월드 반경 | 오차 |
|---|---|---|---|---|
| 체리 | 0.50 | 0.50 | 0.50 | 0% (우연히 정확) |
| 딸기 | 0.625 | 0.625 | 0.78 | +25% |
| 수박 | 4.66 | 4.66 | 43.4 | +831% |

체리는 우연히 맞지만 단계가 높아질수록 콜라이더가 스프라이트보다 훨씬 커진다.

또한, 스프라이트의 PPU(Pixels Per Unit) 설정에 따라 로컬 크기가 달라지므로
수식으로만 스케일을 계산하면 어떤 스프라이트에서도 정확하게 맞지 않는다.

---

## 수정 내용 — `Assets/Scripts/Fruit.cs`

### 핵심 원리

스프라이트의 실제 로컬 반경(`sprite.bounds.extents.x` = 픽셀 절반 / PPU)을 읽어
원하는 월드 반경(`fruitData.radius`)이 되도록 스케일을 **역산**한다.

```
spriteLocalHalfWidth = sprite.bounds.extents.x  (PPU 보정 후 로컬 반경)
scale                = fruitData.radius / spriteLocalHalfWidth

월드 콜라이더 반경 = spriteLocalHalfWidth × scale = fruitData.radius ✓
```

### 스프라이트 있을 때

```csharp
float spriteLocalHalfWidth = fruitData.sprite.bounds.extents.x;
float scale = fruitData.radius / spriteLocalHalfWidth;
transform.localScale = Vector3.one * scale;
circleCollider.radius = spriteLocalHalfWidth; // 로컬 반경
// 월드 반경 = spriteLocalHalfWidth * scale = fruitData.radius ✓
```

### 스프라이트 없을 때 (폴백) — `ApplyDefaultSizeAndCollider()`

스프라이트가 null이거나 bounds를 읽지 못하면 기본 동작 유지:
- `localScale = Vector3.one * radius * 2` (직경 = 단위 원 기준)
- `circleCollider.radius = 0.5` → 월드 반경 = `0.5 × radius×2 = radius` ✓

---

## 완료 조건 체크

- [x] 스프라이트 bounds를 기반으로 콜라이더 반경을 자동 보정
- [x] 모든 단계(Cherry~Watermelon)에서 콜라이더 월드 반경 = `fruitData.radius`
- [x] 스프라이트 PPU 설정이 달라도 자동 보정되므로 에셋 교체에 강함
- [x] 스프라이트 없는 경우 기존 폴백 로직으로 안전하게 처리
