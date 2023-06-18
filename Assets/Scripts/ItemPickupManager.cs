using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickupManager : MonoBehaviour
{
    [SerializeField] private GameObject itemPickUpHolder;
    [SerializeField] GameObject gameManagerObject;
    private GameStateManager gameStateManager;
    private GameObject shelfInRange;
    private bool shelfInRangeBool;


    void Start()
    {
        this.shelfInRangeBool = false;
        this.gameStateManager = gameManagerObject.GetComponent<GameStateManager>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && shelfInRangeBool)
        {
            Debug.Log("Consuming item");

            gameStateManager.ConsumeItem();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Active")
        {
            // Handle consuming here
            shelfInRange = other.GetComponent<GameObject>();
            Debug.Log("Item in range");
            this.shelfInRangeBool = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Active")
        {
            this.shelfInRangeBool = false;
            shelfInRange = null;
        }
    }


}
