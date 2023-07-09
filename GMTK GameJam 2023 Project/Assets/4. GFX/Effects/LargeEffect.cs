using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LargeEffect : MonoBehaviour
{
    Animator animator;
    

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

    public void StopAnimation()
    {
        gameObject.SetActive(false);
    }
}
