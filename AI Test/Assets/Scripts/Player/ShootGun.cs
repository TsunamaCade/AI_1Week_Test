using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootGun : MonoBehaviour
{
    [SerializeField] private Transform gun;
    RaycastHit hit;

    [SerializeField] private bool canShoot;

    [SerializeField] private GameObject[] nonHostiles;

    void Update()
    {
        nonHostiles = GameObject.FindGameObjectsWithTag("NonHostile");
        RaycastHit hit;
        if(Physics.Raycast(gun.position, transform.forward, out hit, Mathf.Infinity))
        {
            if(hit.transform != null)
            {
                //Run away when looked at
                if(hit.transform.CompareTag("NonHostile"))
                {
                    hit.transform.GetComponent<NonHostile>().StopAndMoveToClosestExit();
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

                //Move AI into other position
                if(hit.transform.CompareTag("Ally"))
                {
                    hit.transform.GetComponent<TeammateAI>().inWay = true;
                }

                if(Input.GetButtonDown("Fire1") && canShoot)
                {
                    canShoot = false;
                    
                    //Run away when gunfire
                    foreach(GameObject ai in nonHostiles)
                    {
                        ai.GetComponent<NonHostile>().StopAndMoveToClosestExit();
                    }
                    
                    //Take health away from target
                    if(hit.transform.GetComponent<TakeDamage>() != null)
                    {
                        hit.transform.GetComponent<TakeDamage>().health -= 1f;
                    }

                    StartCoroutine(FireAgain());
                }
                else
                {
                    return;
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