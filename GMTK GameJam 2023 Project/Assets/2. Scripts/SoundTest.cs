using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundTest : MonoBehaviour
{
    string name;
    private InputActions input = null;

    void Awake()
    {
        name = "BGMMusic";
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

    private void Update()
    {
        if (input.PlayerController.Q.WasPressedThisFrame())
        {
            SoundManager.Instance.PlaySound(name, 1f);
            Debug.Log("Starting music");
        }
        if (input.PlayerController.E.WasPressedThisFrame())
        {
            SoundManager.Instance.EndSound(name, 1f);
            Debug.Log("Stopping music");
        }

        if (input.PlayerController.NUM1.WasPressedThisFrame())
        {
            SoundManager.Instance.ChangeSoundVolume(name, 0.1f, 1.5f);
            Debug.Log("Low music");
        }

        if (input.PlayerController.NUM2.WasPressedThisFrame())
        {
            SoundManager.Instance.ChangeSoundVolume(name, 0.5f, 1.5f);
            Debug.Log("Mid music");
        }

        if (input.PlayerController.NUM3.WasPressedThisFrame())
        {
            SoundManager.Instance.ChangeSoundVolume(name, 1f, 1.5f);
            Debug.Log("Max music");
        }

    }


}
