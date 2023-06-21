using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootGun : MonoBehaviour
{
    [SerializeField] private Transform gun;
    RaycastHit hit;

    [SerializeField] private bool canShoot;

    [SerializeField] private GameObject[] nonHostiles;

    [SerializeField] private GameObject hitObject;

    void Update()
    {
        nonHostiles = GameObject.FindGameObjectsWithTag("NonHostile");
        RaycastHit hit;
        if(Physics.Raycast(gun.position, transform.forward, out hit, Mathf.Infinity))
        {
            if(hit.transform != null)
            {
                //Stop and cower when player is looking at them
                if(hit.transform.CompareTag("NonHostile"))
                {
                    hitObject = hit.transform.gameObject;
                    hit.transform.GetComponent<NonHostile>().isLookedAt = true;
                }
                else
                {
                    if((hitObject != null) && (hitObject.transform.GetComponent<NonHostile>() != null))
                    {
                        hitObject.transform.GetComponent<NonHostile>().isLookedAt = false;
                    }
                }

                //Dodge when in LOS
                if(hit.transform.CompareTag("Enemy"))
                {
                    hit.transform.GetComponent<Hostile>().isInView = true;
                    if(!hit.transform.CompareTag("Enemy"))
                    {
                        hit.transform.GetComponent<Hostile>().isInView = false;
                    }
                }

                if(Input.GetButtonDown("Fire1") && canShoot)
                {
                    Debug.Log("Shoot");
                    canShoot = false;

                    //Run away when gunfire
                    foreach(GameObject ai in nonHostiles)
                    {
                        if(!ai.GetComponent<NonHostile>().isFleeing && !ai.GetComponent<NonHostile>().isLookedAt)
                        {
                            ai.GetComponent<NonHostile>().StopAndMoveToClosestExit();
                        }
                        else
                        {
                            return;
                        }
                    }
                    
                    //Take health away from target
                    if(hit.transform.GetComponent<TakeDamage>() != null)
                    {
                        Debug.Log("Take Health");
                        hit.transform.GetComponent<TakeDamage>().health -= 1f;
                    }

                    StartCoroutine(FireAgain());
                }
            }
        }
    }

    IEnumerator FireAgain()
    {
        yield return new WaitForSeconds(2f);
        canShoot = true;
    }
}