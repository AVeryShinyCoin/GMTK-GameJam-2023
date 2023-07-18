using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

public class TableSlamAnimation : MonoBehaviour
{
    [SerializeField] string sound;
    [SerializeField] GameObject bottle;
    [SerializeField] GameObject cameraAnimator;


    private InputActions input = null;


    public void PlayAnimation()
    {
        GetComponent<Animator>().Play("Slam");

    }

   



    private void Update()
    {
        if (input.PlayerController.Space.WasPerformedThisFrame())
        {
            PlayAnimation();
        }
    }

    public void PlaySlam()
    {
        bottle.GetComponent<Animator>().Play("Jump");
        cameraAnimator.GetComponent<Animator>().Play("Shake");
    }

    public void PlaySound()
    {
        if (sound != null) SoundManager.Instance.PlayUniqueSound(sound);
    }


    private void Awake()
    {
        input = new InputActions();
    }

    private void OnEnable()
    {
        input.Enable();
    }

    private void OnDisable()
    {
        input.Disable();
    }
}
