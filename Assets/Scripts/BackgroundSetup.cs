using UnityEngine;

/// <summary>
/// 게임 배경을 설정하는 초기화 스크립트
/// </summary>
public class BackgroundSetup : MonoBehaviour
{
    [SerializeField] private Color backgroundColor = new Color(0.878f, 0.98f, 1f, 1f); // #E0F6FF (밝은 하늘색)

    private void Awake()
    {
        // 카메라 배경색 설정
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            mainCamera.backgroundColor = backgroundColor;
        }
    }
}
