using System;
using System.Collections;
using UnityEngine;

public class Item : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    CircleCollider2D circleCollider2D;

    AudioSource audioSource;
    [SerializeField] AudioClip effectAudioClip;
    [SerializeField] AudioClip sideEffectAudioClip;

    public bool used = false;
    public Action OnUsedItem;


    public bool doNotEatTooMuch = true;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        circleCollider2D = GetComponent<CircleCollider2D>();
    }

    public virtual void Effect()
    {
        Debug.Log($"Item {gameObject.name} 효과 발동 아싸~");
        VisualComponentDisabled();
        used = true;

        PlayAudio(effectAudioClip);
    }

    public virtual void SideEffect()
    {
        Debug.Log($"Item {gameObject.name} 부작용 낄낄");
        VisualComponentDisabled();
        used = true;
        PlayAudio(sideEffectAudioClip);
    }

    private void VisualComponentDisabled()
    {
        spriteRenderer.enabled = false;
        circleCollider2D.enabled = false;
    }


    void PlayAudio(AudioClip clip)
    {
        if (clip == null)
            return;

        audioSource.clip = clip;
        audioSource.Play();
    }

}