using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Hostile : MonoBehaviour
{
    [SerializeField] private NavMeshAgent nma;


    [SerializeField] private bool canDodge;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float dodgeForce;
    [SerializeField] private float dodgeDuration;

    [SerializeField] private float wanderRange;

    private bool isDodging = false;

    [SerializeField] private Transform player;
    [SerializeField] private Transform playerGun;

    [SerializeField] private GameObject weapon;
    [SerializeField] private float bulletSpread;
    [SerializeField] private float bulletRange;
    

    public bool isInView;

    void Update()
    {
        if(canDodge == true && isInView)
        {
            StartCoroutine(Dodge());
        }
        else if(playerGun.gameObject.activeSelf == true)
        {
            Attack();
        }
        else
        {
            Wander();
        }
    }

    IEnumerator Dodge()
    {
        isInView = false;
        isDodging = true;
        Vector3 directionToPlayer = player.position - transform.position;
        directionToPlayer.Normalize();

        //Find the angle between AI's forward vector and direction to player
        Vector3 crossProduct = Vector3.Cross(transform.forward, directionToPlayer);

        Vector3 dodgeDirection;
        if (crossProduct.y > 0)
        {
            // Dodge to the right
            dodgeDirection = -transform.right;
        }
        else
        {
            // Dodge to the left
            dodgeDirection = transform.right;
        }

        rb.AddForce(dodgeDirection * dodgeForce, ForceMode.VelocityChange);

        yield return new WaitForSeconds(dodgeDuration);

        rb.velocity = Vector3.zero;
        isDodging = false;
        canDodge = false;
        isInView = false;
        yield return new WaitForSeconds(5f);
        isInView = false;
        canDodge = true;
    }

    void Attack()
    {
        //Move in range of player and fire gun
        nma.SetDestination(player.position);
        weapon.SetActive(true);
        if(nma.remainingDistance <= nma.stoppingDistance)
        { 
            Vector3 dir = player.position - transform.position;
            Quaternion lookRotation = Quaternion.LookRotation(dir);
            Vector3 rotation = lookRotation.eulerAngles;
            transform.rotation = Quaternion.Euler(0f, rotation.y, 0f);
            Shoot();
        }
    }

    void Shoot()
    {
        //If bullet hits, friendly takes damage
        RaycastHit hit;
        Vector3 dir = transform.forward + new Vector3(Random.Range(-bulletSpread, bulletSpread), Random.Range(-bulletSpread, bulletSpread), Random.Range(-bulletSpread, bulletSpread));
        if(Physics.Raycast(transform.position, dir, out hit, bulletRange))
        {
            if(hit.transform.CompareTag("Ally"))
            {
                hit.transform.GetComponent<TakeDamage>().health -= 1f;
            }
        }
    }

    void Wander()
    {
        weapon.SetActive(false);
        if (!nma.pathPending && nma.remainingDistance <= nma.stoppingDistance)
        {
            nma.SetDestination(NewWanderPoint());
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
}
