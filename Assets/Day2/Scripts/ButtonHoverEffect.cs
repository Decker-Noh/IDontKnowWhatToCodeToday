using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public class ButtonHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("버튼 설정")]

    [SerializeField] private AudioSource audioSource; // 호버 사운드

    [Header("애니메이션 설정")]
    [SerializeField] private Vector3 hoverScale = new Vector3(1.1f, 1.1f, 1.1f); // 호버 시 크기
    [SerializeField] private float animationDuration = 0.2f; // 애니메이션 시간


    [SerializeField] private AudioClip hoverSound;
    private Vector3 originalScale;


    private void Awake()
    {
        // 원래 크기를 저장
        originalScale = transform.localScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("asdasdfasdf");
        // 버튼이 호버되었을 때 크기와 위치 변경

        transform.DOScale(hoverScale, animationDuration).SetEase(Ease.OutBack).SetUpdate(true);

        // 호버사운드 재생
        if (audioSource != null)
        {
            audioSource.clip = hoverSound;
            audioSource.Play();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // 버튼에서 마우스가 벗어났을 때 원래 크기로 돌아오기
        transform.DOScale(originalScale, animationDuration).SetEase(Ease.OutBack).SetUpdate(true);
    }
}