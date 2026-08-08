using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeBullet : MonoBehaviour
{
    float velocityMag;
    float originalRotation;

    [SerializeField] float timeBetweenChanges = 2;
    [SerializeField] float zigzagAngle = 45;
    float frequencyTime;
    int positive = 1;

    [Space(10)]
    [SerializeField] Rigidbody2D rb;

    private void Start() {
        rb = GetComponent<Rigidbody2D>();
        positive = 1;
    }

    private void FixedUpdate() {
        velocityMag = rb.velocity.magnitude;
        rb.velocity = transform.up * velocityMag;

        originalRotation = rb.rotation - (zigzagAngle * positive);

        if(Time.time - frequencyTime > timeBetweenChanges) {
            frequencyTime = Time.time;

            positive *= -1;
            rb.rotation = originalRotation + (zigzagAngle * positive);
        }
    }
}
