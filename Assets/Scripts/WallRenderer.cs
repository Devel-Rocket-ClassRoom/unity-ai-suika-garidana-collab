using UnityEngine;

/// <summary>
/// 컬러 기반 벽과 바닥을 렌더링하기 위해 런타임에 간단한 스프라이트를 생성
/// </summary>
public class WallRenderer : MonoBehaviour
{
    private void Start()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null && spriteRenderer.sprite == null)
        {
            spriteRenderer.sprite = CreateSimpleSprite();
        }
    }

    private Sprite CreateSimpleSprite()
    {
        // 간단한 흰색 스프라이트 생성
        Texture2D texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
        texture.SetPixels(new Color[] { Color.white, Color.white, Color.white, Color.white });
        texture.Apply();

        return Sprite.Create(texture, new Rect(0, 0, 2, 2), new Vector2(0.5f, 0.5f));
    }
}
