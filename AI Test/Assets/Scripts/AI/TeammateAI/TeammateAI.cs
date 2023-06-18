using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TeammateAI : MonoBehaviour
{
    [SerializeField] private NavMeshAgent nma;
    [SerializeField] private Transform groupLeader;
    [SerializeField] private bool isPlayerDirecting;
    [SerializeField] private bool hasPlayerDirected;
    [SerializeField] private bool takingDamage;
    [SerializeField] private GameObject weapon;
    [SerializeField] private Transform retreatLocation;
    [SerializeField] private Transform followLocation;
    [SerializeField] private Transform altFollowLocation;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private Transform direction;
    [SerializeField] private Transform point;
    private Quaternion targetRotation;
    public bool inWay;
    [SerializeField] private LayerMask friendlyMask;


    void Start()
    {
        direction = null;
    }

    void Update()
    {
        if (targetRotation != transform.rotation)
        {
            FaceDirection();
        }

        if(groupLeader.transform.GetComponent<ShootGun>() == null)
        {
            return;
        }
        else
        {
            EquiptWeapon();
        }


        if (isPlayerDirecting)
        {
            WaitForCommand();
        }
        else if (takingDamage)
        {
            Retreat();
        }
        else if((groupLeader.position - transform.position).magnitude > 10f)
        {
            Follow();
        }
        else if((groupLeader.position - transform.position).magnitude < 10f)
        {
            Guard();
        }

        if(inWay)
        {
            MoveSpot();
        }

        RaycastHit hit;
        if(Physics.Raycast(transform.position, transform.forward, out hit, Mathf.Infinity, friendlyMask))
        {
            if(hit.transform.GetComponent<TeammateAI>() != null)
            {
                hit.transform.GetComponent<TeammateAI>().inWay = true;
            }
        }
    }

    void EquiptWeapon()
    {
        weapon.SetActive(true);
    }

    void Follow()
    {
        direction = groupLeader;
        nma.SetDestination(followLocation.position);
    }

    void WaitForCommand()
    {
        
    }

    void Attack()
    {
        
    }

    void Retreat()
    {
        
    }

    void Guard()
    {
        direction = point;
    }

    void MoveSpot()
    {
        if(nma.destination == (followLocation.position))
        {
            nma.SetDestination(altFollowLocation.position);
        }
        else if(nma.destination == (altFollowLocation.position))
        {
            nma.SetDestination(followLocation.position);
        }
        if(nma.remainingDistance <= nma.stoppingDistance)
        {
            inWay = false;
        }
    }

    void FaceDirection()
    {
        if(direction != null)
        {
            Vector3 dir = direction.position - transform.position;
            dir.y = 0;
            Quaternion rot = Quaternion.LookRotation(dir);

            targetRotation = rot;

            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
}
