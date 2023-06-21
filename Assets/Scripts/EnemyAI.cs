using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    private NavMeshAgent agent;
    private Transform player;
    [SerializeField] private LayerMask whatIsGround, whatIsPlayer;
    public Transform nextPlaceToGo;

    // Patroll
    public Vector3 walkPoint;
    private bool walkPointSet;
    public float walkPointRange;

    // Chase
    private bool playerIsInSight;

    // States and Animation
    private bool enemyChasing;
    private bool enemyPatroling;
    private bool chooseNextPoint;


    [SerializeField] private GameObject waypointHolder;
    [SerializeField] private Animator animator;
    private Transform[] waypoints;


    public void Awake()
    {
        chooseNextPoint = true;
        waypoints = waypointHolder.GetComponentsInChildren<Transform>();
        player = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();
    }

    public void Update()
    {
        if (!playerIsInSight)
        {
            Patrol();
        }
        else
        {
            Chase();
        }
    }

    private void Chase()
    {
        agent.SetDestination(player.position);
    }

    private IEnumerator WaitUntilNextCheckpoint()
    {
        // Wait for the specified duration
        chooseNextPoint = false;

        yield return new WaitForSeconds(2f);

        chooseNextPoint = true;
    }

    private void Patrol()
    {
        if (!walkPointSet)
        {
            SetWalkPoint();
        }
        else
        {

            if (chooseNextPoint)
            {
                agent.SetDestination(walkPoint);
            }

        }

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        if (distanceToWalkPoint.magnitude < 1f)
        {
            // Distance reached
            walkPointSet = false;

            animator.SetBool("isPatroling", false);
            animator.SetBool("isChasing", false);

            StartCoroutine(WaitUntilNextCheckpoint());

        }
    }
    private void SetWalkPoint()
    {
        // Assume ts.Length is 77
        int[] randomWaypoints = new int[4];
        int quarter = waypoints.Length / 4;
        int counter = 0;
        int oldQuarter = 0;

        Vector3 playerPosition = player.transform.position;

        while (counter < 4)
        {
            if (counter == 0)
            {
                int zeroShelf = Random.Range(0, quarter);
                oldQuarter = quarter;
                randomWaypoints[counter] = zeroShelf;
            }
            else
            {
                int selectedShelf = Random.Range(oldQuarter, (counter + 1) * quarter);
                randomWaypoints[counter] = selectedShelf;
                oldQuarter = selectedShelf + 1; // Increment oldQuarter to the next shelf
            }

            counter++;
        }

        List<Transform> finalWaypoints = new List<Transform>();
        for (int i = 0; i < randomWaypoints.Length; i++)
        {
            finalWaypoints.Add(waypoints[randomWaypoints[i]]);
        }

        finalWaypoints.Sort((a, b) =>
            Vector3.Distance(playerPosition, a.position).CompareTo(Vector3.Distance(playerPosition, b.position))
        );

        nextPlaceToGo = finalWaypoints[0];

        walkPoint = finalWaypoints[0].position;
        walkPointSet = true;

        animator.SetBool("isPatroling", true);

    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            playerIsInSight = true;
            enemyChasing = true;
            enemyPatroling = false;

            animator.SetBool("isPatroling", false);
            animator.SetBool("isChasing", true);

            gameObject.GetComponent<CapsuleCollider>().radius = 2f;
            gameObject.GetComponent<CapsuleCollider>().height = 2f;

        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            playerIsInSight = false;
            enemyPatroling = true;
            enemyChasing = false;


            animator.SetBool("isPatroling", true);
            animator.SetBool("isChasing", false);

            gameObject.GetComponent<CapsuleCollider>().radius = 0.4f;
            gameObject.GetComponent<CapsuleCollider>().height = 0.8f;
        }
    }
}
