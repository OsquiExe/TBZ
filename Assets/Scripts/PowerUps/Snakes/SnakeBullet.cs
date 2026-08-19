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
    [SerializeField] SpriteRenderer sprite;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] TrailRenderer tail;

    private void Start() {
        rb = GetComponent<Rigidbody2D>();
        positive = 1;

        tail.Clear();
    }

    private void OnEnable() {
        tail.Clear();
    }

    private void FixedUpdate() {
        velocityMag = rb.velocity.magnitude;
        rb.velocity = transform.up * velocityMag;

        originalRotation = transform.eulerAngles.z - (zigzagAngle * positive);

        if(Time.time - frequencyTime > timeBetweenChanges) {
            frequencyTime = Time.time;

            positive *= -1;
            transform.eulerAngles = Vector3.forward * (originalRotation + (zigzagAngle * positive));
            sprite.transform.localEulerAngles = Vector3.forward * (-(zigzagAngle * positive) + 90f);

            if(sprite.transform.parent != null)
            sprite.transform.localPosition = Vector3.zero;
        }
    }
}
