using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossGFX : MonoBehaviour
{

    BossMechanics parentScript;
    Animator animator;

    private void Awake()
    {
        parentScript = GetComponentInParent<BossMechanics>();
        animator = GetComponent<Animator>();
    }
    private void Update()
    {
        if (parentScript.BossHP <= 0)
        {
            gameObject.GetComponent<SpriteRenderer>().color = new Color(0.5f , 0.0f, 0.0f, 1f);
            animator.speed = 0;
        }
        else
        {
            gameObject.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
            animator.speed = 1;
        }
        
    }
}
