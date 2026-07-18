using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PowerUps {
    public class LaserOld : PowerUpClass {
        [SerializeField] float[] rotation = { -45, 45, 135, 225 };
        [Space(10)]

        [SerializeField] public float damage = 1f;
        [SerializeField] public float knockBackForce = 5f;

        [Space(10)]
        [SerializeField] LineRenderer lineRenderer;
        [SerializeField] float maxRayBounces = 10;

        protected override void OnUpdate() {
            var points = CalculateRays();

            lineRenderer.positionCount = points.Length;
            lineRenderer.SetPositions(points);
        }

        protected override void OnCooldownStarts() {
            lineRenderer.enabled = false;
        }

        protected override void OnCooldownEnds() {
            transform.localEulerAngles = new Vector3(0, 0, rotation[Random.Range(0, rotation.Length)]);
            lineRenderer.enabled = true;
        }

        Vector3[] CalculateRays() {
            List<Vector3> points = new(){ transform.position };

            Vector3 previousOrigin = transform.position;
            Vector3 previousDirection = transform.up;
            for(int i = 0; i < maxRayBounces; i++) {

                RaycastHit2D hit = Physics2D.Raycast(previousOrigin, previousDirection, Mathf.Infinity);
                if(hit.transform != null) {
                    Debug.DrawLine(previousOrigin, hit.point);
                    Debug.DrawRay(hit.point, (Vector3)hit.point - hit.transform.position, Color.red);

                    previousDirection = (Vector3)hit.point - hit.transform.position;
                    previousOrigin = (Vector3)hit.point + (previousDirection * 0.1f);

                    points.Add(hit.point);

                    if(hit.transform.tag == "Enemy") {
                        Enemy enemy = hit.transform.GetComponent<Enemy>();


                        if(enemy != null) {
                            Vector2 direction = (hit.transform.position - transform.position).normalized;

                            Vector2 force = direction * knockBackForce;

                            enemy.TakeDamage(damage, force);
                            break;
                        }
                    }

                } else break;
            }

            return points.ToArray();
        }
    }

}