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

    private void Update()
    {
        if (ignoreCollisionWithPlayer)
        {
            StartCoroutine(CooldownPickUpSameObject());
        }
    }

    private IEnumerator CooldownPickUpSameObject()
    {
        IgnorePlayerCollision();
        yield return new WaitForSeconds(0.5f);
        gameObject.tag = "Pickup";
        StopIgnoringCollision();
        ignoreCollisionWithPlayer = false;
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
