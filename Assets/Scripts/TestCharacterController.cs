using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCharacterController : MonoBehaviour
{

    private CharacterController controller;

    public float speedMove = 5f;

    public float speedRotate = 25f;
    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 move = new Vector3(0, 0,Input.GetAxis("Vertical"));
        Vector3 rotation = new Vector3(0,Input.GetAxis("Horizontal"), 0);
        

        controller.transform.Translate(move * Time.deltaTime * speedMove,Space.Self);
        controller.transform.Rotate(rotation * Time.deltaTime * speedRotate);
    }
}
