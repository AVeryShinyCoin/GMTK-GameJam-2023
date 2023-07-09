using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance;
    GameObject mainCamera;

    Vector3 target;
    Vector3 origin;
    Vector3 difference;
    bool moving;
    float progress;
    float time;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        mainCamera = Camera.main.gameObject;
        time = 2.0f;
    }

    public void PanToEditScreen()
    {
        target = new Vector3(21f, 0f, -10f);
        origin = transform.position;
        difference = target - origin;
        moving = true;
        progress = 0;
    }

    public void PanToRaidScreen()
    {
        target = new Vector3(0f, 0f, -10f);
        origin = transform.position;
        difference = target - origin;
        moving = true;
        progress = 0;
    }

    private void FixedUpdate()
    {
        if (moving)
        {
            progress += Time.deltaTime;
            float ratio = progress / time;
            mainCamera.transform.position = (origin + difference * ratio);

            if (progress >= time)
            {
                moving = false;
                mainCamera.transform.position = target;
            }

        }
    }
}
