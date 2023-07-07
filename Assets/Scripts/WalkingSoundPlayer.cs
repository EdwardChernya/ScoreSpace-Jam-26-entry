using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkingSoundPlayer : MonoBehaviour
{
    [SerializeField] public LayerMask whatIsGround;
    private AudioSource walkSoundSource;
    private GameObject player;
    private PlayerMovement playerController;

    void Start()
    {
        walkSoundSource = GetComponent<AudioSource>();
        player = GameObject.Find("Player");
        playerController = player.GetComponent<PlayerMovement>();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Ground")
        {
            if (playerController.verticalInput != 0 || playerController.horizontalInput != 0)
            {
                Debug.Log("Triggered");
                //walkSoundSource.Play(0);
            }
        }
    }
}
