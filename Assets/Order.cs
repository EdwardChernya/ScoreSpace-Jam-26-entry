using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.UI;

public class Order : MonoBehaviour
{
    public OrdersManager orderManager;
    public Transform player;
    public OrderDifficulty orderDifficulty;

    private Throw throwController;
    private bool orderStarted = false;
    private bool orderDelivered = false;
    private bool orderExpired = false;
    private float orderTimerAmount;
    private float orderTimeLeft;
    private bool playerInRange = false;

    private List<OrderItem> orderItemsRequired = new List<OrderItem>();
    private List<OrderItem> orderItemsDelivered = new List<OrderItem>();
    
    void Start()
    {
        this.throwController = player.transform.Find("Penguin").transform.GetComponent<Throw>();
    }

    void Update()
    {
        
    }

    private bool CheckAllItemsDelivered<T>(T[] first, T[] second)
    {
        return Enumerable.SequenceEqual(first, second);
    }

    void DeliverOrder()
    {
        if (orderStarted && playerInRange && !orderExpired && !orderDelivered && Input.GetKeyDown(KeyCode.E)) 
        {
            if (throwController.currentlyHoldingObject)
            {
                try
                {
                    OrderItem item = throwController.currentHeldObject.GetComponent<OrderItem>();

                    for (int i = 0;i < orderItemsRequired.Count; i++)
                    {
                        if (orderItemsRequired[i] == item)
                        {
                            // Deliver 1 item that is correct
                            orderItemsDelivered.Add(item);

                            GameObject.Destroy(item.gameObject); // Remove the delivered object

                            bool allItemsDelivered = orderItemsDelivered.SequenceEqual(orderItemsRequired); // Check if all items are delivered

                            if (allItemsDelivered)
                            {
                                orderManager.DeliverOrder(this, orderTimeLeft, orderDifficulty);
                                this.orderDelivered = true;


                            }

                        }
                    }

                }
                catch (System.Exception ex)
                {
                    return;
                }
            }



            this.orderManager.orders.Remove(this);
            
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            this.playerInRange = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            this.playerInRange = false;
        }
    }


}
