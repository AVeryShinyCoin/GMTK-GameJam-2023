using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LargeEffect : MonoBehaviour
{
    Animator animator;
    [SerializeField] string sound;
    

    private void Awake()
    {
        animator = GetComponent<Animator>();
        gameObject.SetActive(false);
    }

    public void PlayAnimation()
    {
        gameObject.SetActive(true);
        animator.Play("Ability");
    }

    public void PlaySound()
    {
        if (sound == null) return;

        SoundManager.Instance.PlayUniqueSound(sound);
    }

    public void StopAnimation()
    {
        gameObject.SetActive(false);
    }
}
