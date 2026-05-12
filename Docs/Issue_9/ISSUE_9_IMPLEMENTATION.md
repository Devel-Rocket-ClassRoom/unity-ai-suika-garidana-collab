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

`sprite.bounds.extents`를 이용한 자동 보정도 시도했지만,
스프라이트마다 투명 여백·PPU 설정이 달라 시각적으로 일치하지 않았다.

---

## 최종 수정 내용 — `Assets/Scripts/Fruit.cs`

### 핵심 원리: 고정 사이즈 방식

스프라이트 크기에 맞추려는 시도를 포기하고 `fruitData.radius`로 크기와 콜라이더를 **항상 고정**한다.
스프라이트는 시각 표시 전용으로만 사용한다.

```
localScale   = radius × 2
collider.radius = 0.5 (로컬)

월드 콜라이더 반경 = 0.5 × (radius × 2) = radius ✓
```

### 수정 후 코드

```csharp
// 스프라이트는 표시용으로만 설정
if (spriteRenderer != null)
{
    if (fruitData.sprite != null)
    {
        spriteRenderer.sprite = fruitData.sprite;
        spriteRenderer.color = Color.white;
    }
    else
    {
        spriteRenderer.color = fruitData.color;
    }
}

// localScale = diameter(radius*2), 콜라이더 로컬 반경 0.5
// → 월드 콜라이더 반경 = 0.5 * (radius*2) = radius
transform.localScale = Vector3.one * fruitData.radius * 2f;
if (circleCollider != null)
{
    circleCollider.radius = 0.5f;
    circleCollider.isTrigger = false;
}
```

---

## 완료 조건 체크

- [x] 모든 단계(Cherry~Watermelon)에서 콜라이더 월드 반경 = `fruitData.radius` (수식으로 보장)
- [x] 스프라이트 유무와 무관하게 동일한 물리 크기 적용
- [x] `sprite.bounds` 의존성 완전 제거 — 스프라이트 PPU/여백에 영향받지 않음
- [x] Issue #10 새 radius 값(baseRadius=0.25, multiplier=1.32)과 연동하여 정확히 동작
