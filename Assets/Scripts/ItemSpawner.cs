using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class ItemSpawner : MonoBehaviour
{
    [SerializeField] private GameObject spawnShelfParent;
    [SerializeField] private GameObject player;
    [SerializeField] private Material highlightMaterial;
    private Material normalMaterial;

    public bool gameJustStarted;

    private string SHELF_ACTIVE = "Active";
    private string SHELF_INACTIVE = "Inactive";

    private Transform[] ts;
    public Transform currentActiveShelf;


    void Start()
    {
        ts = spawnShelfParent.GetComponentsInChildren<Transform>();
    }

    void Update()
    {
    }

    public void ChooseNextShelf()
    {
        // Assume ts.Length is 77
        int[] randomShelves = new int[4];
        int quarter = ts.Length / 4;
        int counter = 0;
        int oldQuarter = 0;

        Vector3 playerPosition = player.transform.position;

        while (counter < 4)
        {
            if (counter == 0)
            {
                int zeroShelf = Random.Range(0, quarter);
                oldQuarter = quarter;
                randomShelves[counter] = zeroShelf;
            }
            else
            {
                int selectedShelf = Random.Range(oldQuarter, (counter + 1) * quarter);
                randomShelves[counter] = selectedShelf;
                oldQuarter = selectedShelf + 1; // Increment oldQuarter to the next shelf
            }

            counter++;
        }

        List<Transform> finalShelves = new List<Transform>();
        for (int i = 0; i < randomShelves.Length; i++)
        {
            finalShelves.Add(ts[randomShelves[i]]);
        }

        finalShelves.Sort((a, b) =>
            Vector3.Distance(playerPosition, a.position).CompareTo(Vector3.Distance(playerPosition, b.position))
        );

        Renderer renderer;

        if (gameJustStarted)
        {
            currentActiveShelf = finalShelves[finalShelves.Count - 1];
            normalMaterial = currentActiveShelf.GetComponent<Renderer>().sharedMaterial;
            renderer = currentActiveShelf.GetComponent<Renderer>();
            renderer.sharedMaterial = highlightMaterial;
            currentActiveShelf.tag = SHELF_ACTIVE;

            gameJustStarted = false;
        }
        else
        {
            renderer = currentActiveShelf.GetComponent<Renderer>();
            renderer.sharedMaterial = normalMaterial;
            currentActiveShelf.tag = SHELF_INACTIVE;

            currentActiveShelf = finalShelves[finalShelves.Count - 1];

            renderer = currentActiveShelf.GetComponent<Renderer>();
            normalMaterial = renderer.sharedMaterial;
            renderer.sharedMaterial = highlightMaterial;
            currentActiveShelf.tag = SHELF_ACTIVE;
        }

    }
}
