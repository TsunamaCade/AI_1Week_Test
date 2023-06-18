using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TeammateAI : MonoBehaviour
{
    [SerializeField] private NavMeshAgent nma;

    [SerializeField] private Transform groupLeader;

    [SerializeField] private bool takingDamage;

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

        if (takingDamage)
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

    void Follow()
    {
        //Set to follow the leader when they move out of range
        direction = groupLeader;
        nma.SetDestination(followLocation.position);
    }

    void WaitForCommand()
    {
        //Player is able to direct the AI using a waypoint system, and can attack certain enemies, take cover, or simply relocate based on the player's crosshair
    }

    void Attack()
    {
        //Attack enemies
    }

    void Retreat()
    {
        //If the AI took too much damage, it would find a suitable location to flee to to recover health
    }

    void Guard()
    {
        //Look towards certain direction, updates in live gameplay
        direction = point;
    }

    void MoveSpot()
    {
        //If AI comes into contact of Player or Teammate crosshair
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
        //Face set direction to nigate Navmesh being weird about included code for facing directions
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
