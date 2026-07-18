using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserBullet : MonoBehaviour
{
    [SerializeField] Rigidbody2D rb;
    [SerializeField] float speedMult = 1;

    private void OnEnable() {
        rb.AddForce(transform.right * speedMult, ForceMode2D.Impulse);
    }

    private void OnDisable() {
        rb.velocity = Vector2.zero;
    }


    private void OnTriggerEnter2D(Collider2D collision) {
        Debug.Log("a");

        Vector3 surfaceNormal = -(collision.transform.position - transform.position).normalized;

        Debug.DrawRay(rb.position, surfaceNormal);

        rb.velocity = surfaceNormal * speedMult;
        transform.right = surfaceNormal;
    }
}
