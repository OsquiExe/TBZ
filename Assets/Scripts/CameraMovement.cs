using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [Header ("Player")]
    [SerializeField]public Transform player;
    [SerializeField]public Rigidbody2D rb;

    [Header ("Camera settings")]
    public float smoothness = 0.15f;
    public Vector3 offset = new Vector3(0, 0, -10);
    private Vector3 velocity = Vector3.zero;

    void LateUpdate()
    {
        if ( player == null) return;

        
        Vector2 playerVelocity = rb. velocity;

      
        Vector3 targetPosition = new Vector3 ( player. position.x, player. position.y, offset.z);

        
        if ( playerVelocity. sqrMagnitude > 0.01f)
        {
            targetPosition += new Vector3(playerVelocity.x, playerVelocity.y, 0) * Time.deltaTime;
        }
        
        transform.position = Vector3.SmoothDamp( transform.position, targetPosition + offset, ref velocity, smoothness);
    } 
}
