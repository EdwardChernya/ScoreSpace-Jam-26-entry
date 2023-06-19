using UnityEngine;

public class TrolleyManager : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject camera;
    [SerializeField] private GameObject playerHoldPoint;
    private CameraMovement cameraMovement;
    private GameObject trolleyInRange;
    private Transform trolleyOrientation;
    private Transform currentTrolley;


    private bool trolleyIsInRange;
    private bool playerOnTrolley = false;


    void Start()
    {
        camera = GameObject.Find("PlayerCam");
        cameraMovement = camera.GetComponent<CameraMovement>();
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.E) && trolleyIsInRange)
        {

            playerHoldPoint = trolleyInRange.transform.Find("trooley model").Find("playerMount").gameObject;

            player.transform.position = playerHoldPoint.transform.position;
            player.transform.SetParent(playerHoldPoint.transform);
            player.transform.rotation = Quaternion.identity;

            trolleyOrientation = trolleyInRange.transform.Find("trolleyOrientation");

            cameraMovement.trolley = trolleyInRange.transform;
            cameraMovement.trolleyOrientation = trolleyOrientation;
            cameraMovement.trolleyObject = trolleyInRange.transform;
            cameraMovement.onTrolley = true;


            player.GetComponent<PlayerMovement>().enabled = false;
            //player.GetComponent<Rigidbody>().isKinematic = true;

            trolleyInRange.GetComponent<RolleycartController>().enabled = true;
            this.playerOnTrolley = true;
            this.currentTrolley = trolleyInRange.transform;
        }

        if (playerOnTrolley)
        {
            //player.transform.position = playerHoldPoint.transform.position;
            player.transform.rotation = currentTrolley.transform.rotation;
        }


    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Trolley")
        {
            this.trolleyInRange = other.gameObject;
            this.trolleyIsInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Trolley")
        {
            this.trolleyInRange = null;
            this.trolleyIsInRange = false;
        }
    }




}
