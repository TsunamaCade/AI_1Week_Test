using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakeDamage : MonoBehaviour
{
    public float health = 5;

    void Update()
    {
        if(health <= 0)
        {
            this.gameObject.SetActive(false);
        }
    }
}