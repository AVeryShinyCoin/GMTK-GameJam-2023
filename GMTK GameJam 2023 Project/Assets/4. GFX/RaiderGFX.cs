using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaiderGFX : MonoBehaviour
{
    [SerializeField] RuntimeAnimatorController[] tankAnimators;
    [SerializeField] RuntimeAnimatorController[] ddAnimators;
    [SerializeField] RuntimeAnimatorController[] healerAnimators;
    Raider parentScript;
    Animator animator;
    string currentAnimation;
    SpriteRenderer sr;

    private void Awake()
    {
        parentScript = GetComponentInParent<Raider>();
        animator = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
    }
    private void Start()
    {
        int role = parentScript.Role;
        if (role == 0)
        {
            GetComponent<Animator>().runtimeAnimatorController = tankAnimators[Random.Range(0, tankAnimators.Length)];
        }
        if (role == 1)
        {
            GetComponent<Animator>().runtimeAnimatorController = ddAnimators[Random.Range(0, ddAnimators.Length)];
        }
        if (role == 2)
        {
            GetComponent<Animator>().runtimeAnimatorController = healerAnimators[Random.Range(0, healerAnimators.Length)];
        }
    }

    void SetCurrentAnimation(string newAnimation)
    {
        if (currentAnimation == newAnimation) return;
        currentAnimation = newAnimation;
        animator.Play(newAnimation);
    }

    private void Update()
    {
        if (parentScript.moving)
        {
            SetCurrentAnimation("Run");
            if (transform.position.x > parentScript.tarDestination.x)
            {
                sr.flipX = true;
            } else
            {
                sr.flipX = false;
            }
        }

        float ratio = ((float)parentScript.HitPoints / (float)parentScript.MaxHitPoints);
        GetComponent<SpriteRenderer>().color = new Color(0.5f + ratio/2, ratio, ratio, 1f);
    }

    public void ArrivedAtDestination()
    {
        SetCurrentAnimation("Idle");
        if (transform.position.x > BossMechanics.Instance.transform.position.x)
        {
            sr.flipX = true;
        }
        else
        {
            sr.flipX = false;
        }
    }

    public void UseAbility()
    {
        SetCurrentAnimation("Ability");
    }

    public void AbilityComplete()
    {
        SetCurrentAnimation("Idle");
    }

    public void Die()
    {
        SetCurrentAnimation("Death");
    }
}
