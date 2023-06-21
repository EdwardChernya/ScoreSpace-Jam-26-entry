using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIController : MonoBehaviour
{
    public GameObject player;
    public GameObject colObject;
    private Vector3 playerPosition;
    private NavMeshAgent navMeshAgent;

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {

        if (colObject.GetComponent<AIColider>().spotted)
        {
            playerPosition = player.transform.position;

            navMeshAgent.destination = playerPosition;
        }
    }
}
