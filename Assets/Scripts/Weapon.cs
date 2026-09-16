using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] public float damage = 5f;
    [SerializeField] public float knockBackForce = 4f;

    private PlayerController playerRef;

    private void Start()
    {
        playerRef = FindObjectOfType<PlayerController>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        float finalDamage = damage;

        // Sumar daño del efecto de clase (si existe)
        DamageIncreaseEffect dmgEffect = GetComponentInParent<DamageIncreaseEffect>();
        if (dmgEffect != null)
        {
            finalDamage += dmgEffect.GetExtraDamage();
        }

        // Sumar daño base de los ítems de The Gang
        if (playerRef != null)
        {
            finalDamage += playerRef.bonusDamage;
        }

        Enemy enemy = collision.GetComponent<Enemy>();
        if (enemy != null && !enemy.isDead)
        {
            Vector2 direction = (collision.transform.position - transform.position).normalized;
            Vector2 force = direction * knockBackForce;

            enemy.TakeDamage(finalDamage, force);

            if (CameraMove.Instance != null)
            {
                CameraMove.Instance.ShakeCamera(0.1f, 0.05f);
            }
        }

        JefeController jefe = collision.GetComponent<JefeController>();
        if (jefe != null)
        {
            jefe.RecibirDano(finalDamage);

            if (CameraMove.Instance != null)
            {
                CameraMove.Instance.ShakeCamera(0.2f, 0.08f);
            }
        }
    }
}