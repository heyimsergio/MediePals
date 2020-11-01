using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{

    [SerializeField] Transform jugador;

    [SerializeField] Vector3 offset;
    
    [SerializeField] float smooth = 0.125f;

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, jugador.position + offset, smooth);
        //transform.position = Vector3.SmoothDamp();
    }
}