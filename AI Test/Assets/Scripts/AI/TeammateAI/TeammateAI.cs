using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TeammateAI : MonoBehaviour
{
    [SerializeField] private NavMeshAgent nma;

    [SerializeField] private Transform groupLeader;
    [SerializeField] private Transform groupLeaderWeapon;
    [SerializeField] private GameObject weapon;

    [SerializeField] private bool becomeHostile;
    [SerializeField] private float wanderRange;

    [SerializeField] private bool takingDamage;

    [SerializeField] private Transform followLocation;
    
    [SerializeField] private float rotationSpeed;

    [SerializeField] private Transform direction;

    [SerializeField] private Transform point;

    private Quaternion targetRotation;

    private bool noDanger = true;

    [SerializeField] private bool canRetreat;

    [SerializeField] private float attackRange;
    [SerializeField] private float attackStopRange;
    [SerializeField] private float attackCooldown;
    private bool isAttacking;
    private float attackTimer;

    private bool attackIsRunning = false;

    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private Transform targetEnemy;
    [SerializeField] private float retreatDistance;


    void Start()
    {
        direction = null;
        nma.SetDestination(NewWanderPoint());
    }

    void Update()
    {
        //If weapons out, become hostile, ready up
        if(becomeHostile)
        {
            if(noDanger)
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
            }

            if (targetEnemy != null)
            {
                noDanger = false;
                float distanceToEnemy = Vector3.Distance(transform.position, targetEnemy.position);
                if(distanceToEnemy <= retreatDistance && canRetreat)
                {
                    Retreat();
                }
                else if(isAttacking)
                {
                    Vector3 dir = targetEnemy.position - transform.position;
                    transform.rotation = Quaternion.LookRotation(Vector3.RotateTowards(transform.forward, dir, rotationSpeed * Time.deltaTime, 0f));
                    if(!attackIsRunning)
                    {
                        StartCoroutine(Attack());
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    nma.SetDestination(targetEnemy.position);
                }
            }
            else
            {
                FindEnemy();
            }
        }

        //If weapons hidden, blend in with the crowd
        else if(!becomeHostile)
        {
            if (!nma.pathPending && nma.remainingDistance <= nma.stoppingDistance)
            {
                nma.SetDestination(NewWanderPoint());
            }
        }

        if(groupLeaderWeapon.gameObject.activeSelf)
        {
            becomeHostile = true;
            weapon.SetActive(true);
        }
    }

    void FindEnemy()
    {
        Debug.Log("FindEnemy");
        //Detect enemies within a certain range
        Collider[] colliders = Physics.OverlapSphere(transform.position, attackRange, enemyLayer);

        if(colliders.Length > 0)
        {
            //Find the closest enemy
            float closestDistance = Mathf.Infinity;
            Transform closestEnemy = null;

            foreach(Collider collider in colliders)
            {
                float distance = Vector3.Distance(transform.position, collider.transform.position);
                if(distance < closestDistance)
                {
                    closestDistance = distance;
                    closestEnemy = collider.transform;
                }
            }

            if (closestEnemy != null)
            {
                targetEnemy = closestEnemy;
                isAttacking = true;
            }
            else
            {
                noDanger = true;
            }
        }
    }


    IEnumerator Attack()
    {
        attackIsRunning = true;
        Debug.Log("Attack");
        if(targetEnemy != null && targetEnemy.gameObject.activeSelf)
        {
            //Check if the enemy is within attack range
            float distance = Vector3.Distance(transform.position, targetEnemy.position);
            if(distance <= attackStopRange)
            {
                RaycastHit hit; 
                if(Physics.Raycast(transform.position, transform.forward, out hit, Mathf.Infinity))
                {
                    if(hit.transform.CompareTag("Enemy"))
                    {
                        hit.transform.GetComponent<TakeDamage>().health -= 1f;
                    }
                }
            }
            else
            {
                //Move towards the enemy if it is out of range
                nma.SetDestination(targetEnemy.position);
            }
        }
        else
        {
            //No target enemy or the enemy is no longer active, go back to guarding
            targetEnemy = null;
            isAttacking = false;
        }
        yield return new WaitForSeconds(0.5f);
        attackIsRunning = false;
    }

    private Vector3 NewWanderPoint()
    {

        //Find another point to move towards within range of navigation
        Vector3 finalPosition = Vector3.zero;
        Vector3 randomPosition = Random.insideUnitSphere * wanderRange;
        randomPosition += transform.position;
        if(NavMesh.SamplePosition(randomPosition, out NavMeshHit hit, wanderRange, 1))
        {
            finalPosition = hit.position;
        }
        return finalPosition;
    }

    void Follow()
    {
        //Set to follow the leader when they move out of range
        direction = groupLeader;
        nma.SetDestination(followLocation.position);
    }

    void Retreat()
    {
        canRetreat = false;
        Debug.Log("Retreat");
        //If too much damage is taken or the enemy is too close, find a suitable location to retreat to to recover health
        Vector3 retreatDirection = transform.position - targetEnemy.position;
        retreatDirection.y = 0f;
        retreatDirection.Normalize();

        Vector3 retreatDestination = transform.position + retreatDirection * retreatDistance;

        nma.SetDestination(retreatDestination);
        StartCoroutine(RetreatAgain());
    }

    IEnumerator RetreatAgain()
    {
        yield return new WaitForSeconds(10f);
        canRetreat = true;
    }

    void Guard()
    {
        //Look towards certain direction, updates in live gameplay
        direction = point;
    }

    void FaceDirection()
    {
        //Face set direction to nigate Navmeshes being weird about code for facing directions
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