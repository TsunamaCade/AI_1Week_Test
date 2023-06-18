using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    public CharacterController cc;
    private Vector3 move;
    [SerializeField] private float speed;

    [SerializeField] private Transform gun;

    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        move = (transform.right * x + transform.forward * z);

        cc.Move(move * speed * Time.deltaTime);

        if(Input.GetButtonDown("Equipt"))
        {
            Debug.Log("Equipt");
            gun.gameObject.SetActive(true);
        }
    }
}
