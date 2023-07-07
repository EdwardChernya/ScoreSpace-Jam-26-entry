using System.Collections;
using UnityEngine;

public class SliderMinigame : MonoBehaviour
{
    private bool playerInRange;
    public bool playerPlayingMinigame;

    private GameObject sliderUi;
    private GameObject slider;
    private Transform leftmostPosition;
    private Transform rightmostPosition;
    private SliderGreenZoneSystem greenZoneChecker;
    private GameObject greenZone;
    private GameObject player;
    private PlayerMovement controller;

    private int correctSliderCompletions;

    private float sliderErrorPoint = 0.01f;
    private float sliderSpeed = 0.001f;
    private bool slideLeft;
    private bool resetSlider = false;
    private bool gameCompletedSuccesfully = false;

    private GameObject playerCam;
    private GameObject aimCam;
    private GameObject freezerCam;
    private Vector3 freezerCameraPosition;
    

    private void Start()
    {
        this.player = GameObject.Find("Player");
        this.playerCam = GameObject.Find("PlayerCam");
        this.freezerCam = GameObject.Find("SliderPuzzleCamera");
        this.aimCam = GameObject.Find("AimCamera");
        this.controller = player.GetComponent<PlayerMovement>();
        this.sliderUi = transform.GetChild(0).gameObject;
        this.sliderUi = transform.GetChild(0).gameObject;
        this.greenZone = sliderUi.transform.GetChild(0).gameObject;
        this.freezerCameraPosition = transform.GetChild(1).gameObject.transform.position;
        this.correctSliderCompletions = 0;
        this.slider = sliderUi.transform.GetChild(1).gameObject;
        this.leftmostPosition = sliderUi.transform.GetChild(2);
        this.rightmostPosition = sliderUi.transform.GetChild(3);
        this.slider.transform.position = leftmostPosition.position;
        this.slideLeft = false;
        this.greenZoneChecker = greenZone.GetComponent<SliderGreenZoneSystem>();
    }

    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E) && !playerPlayingMinigame && !gameCompletedSuccesfully)
        {
            this.aimCam.SetActive(false);
            this.playerCam.SetActive(false);
            this.freezerCam.transform.position = freezerCameraPosition;
            this.freezerCam.SetActive(true);
            this.sliderUi.SetActive(true);
            RespawnGreenZone();
            playerPlayingMinigame = true;
            this.controller.playerPlayingMinigame = true;
        }

        if (playerPlayingMinigame && !gameCompletedSuccesfully && !resetSlider)
        {
            MoveSlider();
            OnCorrectPress();
        }
        
        if (gameCompletedSuccesfully)
        {
            this.controller.playerPlayingMinigame = false;
            playerPlayingMinigame = false;
            playerInRange = false;

            this.playerCam.SetActive(true);
            this.freezerCam.SetActive(false);
            this.sliderUi.SetActive(false);

            gameCompletedSuccesfully = false;
        }

    }

    private void RespawnGreenZone()
    {
        Vector3 randomSpawnPosition = new Vector3(0.13f, Random.Range(-0.33f, 0.39f), 0f);
        this.greenZone.transform.localPosition= randomSpawnPosition;
    }

    private IEnumerator ResetSliderCd()
    {
        this.resetSlider = true;
        this.slider.transform.position = this.leftmostPosition.position;
        RespawnGreenZone();

        yield return new WaitForSeconds(1f);
        this.resetSlider = false;
    }

    private void OnCorrectPress()
    {
        if (playerPlayingMinigame && Input.GetKeyDown(KeyCode.Space) && greenZoneChecker.sliderInGreenZone && !resetSlider)
        {
            
            if (correctSliderCompletions == 2)
            {
                this.gameCompletedSuccesfully = true;
                correctSliderCompletions = 0;
                return;
            }

            correctSliderCompletions++;
            this.greenZone.transform.localScale = new Vector3(1f, this.greenZone.transform.localScale.y - correctSliderCompletions * 0.05f, 1f);
            this.sliderSpeed += 0.002f;

            StartCoroutine(ResetSliderCd());
        }
    }

    private void GameFail()
    {

    }

    private void MoveSlider()
    {
        if (!slideLeft)
        {
            if (Vector3.Distance(slider.transform.position, rightmostPosition.position) > sliderErrorPoint)
            {
                this.slider.transform.position = Vector3.MoveTowards(slider.transform.position, rightmostPosition.position, 0.001f);
                return;
            }
            else
            {
                slideLeft = true;
            }
        }

        if (slideLeft)
        {
            if (Vector3.Distance(slider.transform.position, leftmostPosition.position) > sliderErrorPoint)
            {
                this.slider.transform.position = Vector3.MoveTowards(slider.transform.position, leftmostPosition.position, 0.001f);
                return;
            }
            else
            {
                slideLeft = false;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            this.playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            this.playerInRange = false;
        }
    }



}
