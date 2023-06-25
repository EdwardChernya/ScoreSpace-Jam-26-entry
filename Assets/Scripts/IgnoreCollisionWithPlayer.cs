using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IgnoreCollisionWithPlayer : MonoBehaviour
{
    public  bool ignoreCollisionWithPlayer = false;
    private CapsuleCollider playerCollider;

    private void Start()
    {
        playerCollider = GameObject.Find("NormalCollider").GetComponent<CapsuleCollider>();
    }

    public void StopIgnoringCollision()
    {
        for (int i = 0; i < transform.GetComponentsInChildren<Collider>().Length; i++)
        {
            Physics.IgnoreCollision(playerCollider, transform.GetComponentsInChildren<Collider>()[i], false);
        }
    }

    public void IgnorePlayerCollision()
    {
        for (int i = 0; i < transform.GetComponentsInChildren<Collider>().Length; i++)
        {
            Physics.IgnoreCollision(playerCollider, transform.GetComponentsInChildren<Collider>()[i], true);
        }
    }

}
