using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraComponentController : MonoBehaviour
{
    private GameObject playerCamera;
    private GameObject aimCamera;
    private GameObject minigameCamera;

    private bool camerasActivated = false;

    void Start()
    {
        this.playerCamera = GameObject.Find("PlayerCam");
        this.aimCamera = GameObject.Find("AimCamera");
        this.minigameCamera = GameObject.Find("SliderPuzzleCamera");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void LateUpdate()
    {
        if (!camerasActivated)
        {
            this.playerCamera.SetActive(true);
            this.aimCamera.SetActive(false);
            this.minigameCamera.SetActive(false);
            camerasActivated = true;
        }
    }
}
