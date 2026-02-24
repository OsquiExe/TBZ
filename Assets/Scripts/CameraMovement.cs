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

        //Detectar la velocidad del jugador
        Vector2 playerVelocity = rb. velocity;

        //La cámara se mueve a la posición del jugador, ajustando la velocidad
        Vector3 targetPosition = new Vector3 ( player. position.x, player. position.y, offset.z);

        //Si el jugador se está moviendo la cámara se adapta a su velocidad
        if ( playerVelocity. sqrMagnitude > 0.01f)
        {
            targetPosition += new Vector3(playerVelocity.x, playerVelocity.y, 0) * Time.deltaTime;
        }
        //Suavizado del movimiento
        transform.position = Vector3.SmoothDamp( transform.position, targetPosition + offset, ref velocity, smoothness);
    } 
}
