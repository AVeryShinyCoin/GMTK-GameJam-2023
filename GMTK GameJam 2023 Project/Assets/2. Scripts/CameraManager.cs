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
    float size;
    float targetSize;
    float sizeDifference;

    public bool OnDesk;

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
        OnDesk = false;
        target = new Vector3(21f, 0f, -10f);
        origin = transform.position;
        difference = target - origin;
        moving = true;
        progress = 0;

        targetSize = 5.1f;
        size = Camera.main.orthographicSize;
        sizeDifference = targetSize - size;
    }

    public void PanToRaidScreen()
    {
        OnDesk = false;
        target = new Vector3(0f, 0f, -10f);
        origin = transform.position;
        difference = target - origin;
        moving = true;
        progress = 0;

        targetSize = 5.1f;
        size = Camera.main.orthographicSize;
        sizeDifference = targetSize - size;
    }

    public void PanToDeskScreen()
    {
        OnDesk = true;
        target = new Vector3(4.58f, -2.2f, -10f);
        origin = transform.position;
        difference = target - origin;
        moving = true;
        progress = 0;

        targetSize = 11.5f;
        size = Camera.main.orthographicSize;
        sizeDifference = targetSize - size;
    }

    private void FixedUpdate()
    {
        if (moving)
        {
            progress += Time.deltaTime;
            float ratio = progress / time;
            mainCamera.transform.position = (origin + difference * ratio);

            if (targetSize != size)
            {
                Camera.main.orthographicSize = (size + sizeDifference * ratio);
            }


            if (progress >= time)
            {
                moving = false;
                mainCamera.transform.position = target;
            }
        }
    }
}
