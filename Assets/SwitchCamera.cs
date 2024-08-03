using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class SwitchCamera : MonoBehaviour
{
    public GameObject[] cameras;
    public GameObject pivot;
    public static Transform activeCameraTransform;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            SwitchCam(0);
        }
        else if (Input.GetKeyDown(KeyCode.F2))
        {
            SwitchCam(1);
        }

        if (cameras.Length > 1 && cameras[1].activeSelf && !cameras[0].activeSelf)
        {
            if (pivot != null)
            {
                pivot.SetActive(true);
            }
        }
        else
        {
            if (pivot != null)
            {
                pivot.SetActive(false);
            }
        }
    }

    void SwitchCam(int index)
    {
        for (int i = 0; i < cameras.Length; i++)
        {
            bool isActive = (i == index);
            cameras[i].SetActive(isActive);

            AudioListener audioListener = cameras[i].GetComponent<AudioListener>();
            if (audioListener != null)
            {
                audioListener.enabled = isActive;
            }

            if (isActive)
            {
                activeCameraTransform = cameras[i].transform;
            }
        }
    }

    public static Transform GetActiveCameraTransform()
    {
        return activeCameraTransform;
    }
}
