using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NonHostile : MonoBehaviour
{
    [SerializeField] private Transform[] fleeLocation;
    [SerializeField] private NavMeshAgent nma;
    [SerializeField] private float wanderRange;

    [SerializeField] private bool isWandering = true;

    [SerializeField] private bool isPathing = true;
    [SerializeField] private Transform[] pathPoints;
    private int currentWaypointIndex = 0;

    public bool isFleeing;


    public bool isLookedAt = false;
    private bool hasBeenLookedAt;
    private bool isFleeingFromCower = false;

    void Start()
    {
        nma.speed = 5f;

        if (isWandering)
        {
            nma.SetDestination(NewWanderPoint());
        }
    }

    void Update()
    {
        if(isPathing)
        {
            if (!nma.pathPending && nma.remainingDistance <= nma.stoppingDistance)
            {
                currentWaypointIndex++;
                if (currentWaypointIndex >= pathPoints.Length)
                {
                    currentWaypointIndex = 0;
                }
                nma.SetDestination(pathPoints[currentWaypointIndex].position);
            }
        }

        if (isWandering)
        {
            if (!nma.pathPending && nma.remainingDistance <= nma.stoppingDistance)
            {
                nma.SetDestination(NewWanderPoint());
            }
        }

        if(isLookedAt)
        {
            Cower();
        }
        else if(!isLookedAt && !isFleeingFromCower && hasBeenLookedAt)
        {
            isFleeingFromCower = true;
            StartCoroutine(FleeAfterCower());
        }
    }

    private Vector3 NewWanderPoint()
    {

        //Find another point to move towards within range of navigation
        Vector3 finalPosition = Vector3.zero;
        Vector3 randomPosition = Random.insideUnitSphere * wanderRange;
        randomPosition += transform.position;
        if (NavMesh.SamplePosition(randomPosition, out NavMeshHit hit, wanderRange, 1))
        {
            finalPosition = hit.position;
        }
        return finalPosition;
    }

    private void MoveToClosestExit()
    {
        //Move towards exit
        Vector3 closestExit = FindClosestExit();
        nma.SetDestination(closestExit);
        nma.speed = 10f;
    }

    private Vector3 FindClosestExit()
    {
        //Find closest exit in array
        float closestDistance = Mathf.Infinity;
        Vector3 closestExit = Vector3.zero;

        foreach (Transform point in fleeLocation)
        {
            float distance = (transform.position - point.position).sqrMagnitude;

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestExit = point.position;
            }
        }
        return closestExit;
    }
    
    public void StopAndMoveToClosestExit()
    {
        //Stop everything it's doing, and find a suitable exit
        isFleeing = true;
        isWandering = false;
        isPathing = false;
        nma.ResetPath();
        MoveToClosestExit();
    }

    void Cower()
    {
        //Stop moving because player is pointing weapon at them
        isWandering = false;
        isPathing = false;
        isFleeing = false;
        isFleeingFromCower = false;
        nma.ResetPath();
        hasBeenLookedAt = true;
    }

    IEnumerator FleeAfterCower()
    {
        //Wait until player isn't looking so they may flee
        yield return new WaitForSeconds(Random.Range(3f, 5f));
        StopAndMoveToClosestExit();
    }
}