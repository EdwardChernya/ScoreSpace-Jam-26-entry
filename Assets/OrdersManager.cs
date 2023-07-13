using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public enum OrderType
{
    OnlyDeliver, // 0
    DeliverAndMaybeCook,
    DeliverAndCookEvenSplit,
    MostlyCook,
    OnlyCook // 4
}

public enum OrderDifficulty
{
    Easy,
    Medium,
    Hard,
    VeryHard
}

public class OrdersManager : MonoBehaviour
{
    // In order to split up the levels properly, we define 5 different difficulties
    private int orderTypeCount = Enum.GetValues(typeof(OrderType)).Length;
    private OrderType currentGameOrderType = OrderType.OnlyDeliver;
    public List<Order> orders;
    private int maxActiveOrders = 4;
    private float countdownBetweenOrder = 5f;

    private GameObject coyoteNpcPrefab;

    private void Awake()
    {
        this.orders = new List<Order>();
    }

    void Start()
    {
       
    }

    private IEnumerator CooldownBetweenOrders()
    {
        // Wait for the specified duration
        yield return new WaitForSeconds(countdownBetweenOrder);
    }

    public void DeliverOrder(Order order, float timeLeftFromOrder, OrderDifficulty orderType)
    {
        // Do stuff here for orders and score
    }

    void Update()
    {
        SwitchOrderType();
    }

    void SpawnOrder()
    {
        switch (currentGameOrderType)
        {
            //case OrderType.OnlyDeliver:

        }
    }

    void SwitchOrderType() // Temporary to switch between order types
    {
        if (currentGameOrderType == 0 && Input.GetKeyDown(KeyCode.LeftArrow)) 
        {
            currentGameOrderType = (OrderType)orderTypeCount;
            return;
        }
        else if ((int)currentGameOrderType == orderTypeCount && Input.GetKeyDown(KeyCode.RightArrow))
        {
            currentGameOrderType = (OrderType)orderTypeCount;
            return;
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            int counter = (int)currentGameOrderType--;
            currentGameOrderType = (OrderType)counter;
            return;
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            int counter = (int)currentGameOrderType++;
            currentGameOrderType = (OrderType)counter;
        }
    }
}
