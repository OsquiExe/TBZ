using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] public float damage = 5f;
    [SerializeField] public float knockBackForce = 4f;
    DamageIncreaseEffect dmgEffect;

    private void Awake() {
        DamageIncreaseEffect dmgEffect = GetComponentInParent<DamageIncreaseEffect>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(!collision.CompareTag("Enemy")) return;

        float finalDamage = damage;
         

        if (dmgEffect != null)
        {
            finalDamage += dmgEffect.GetExtraDamage();
        }

        Enemy enemy = collision.GetComponent<Enemy>();
        if (enemy != null && !enemy.isDead)
        {
            Vector2 direction = (collision.transform.position - transform.position).normalized;
            Vector2 force = direction * knockBackForce;

            Debug.Log($"⚔️ El arma golpeó al enemigo normal. Daño: {finalDamage} - {gameObject.name}");
            enemy.TakeDamage(finalDamage, force);

            Debug.Log("A");
            if (CameraMove.Instance != null)
            {
                CameraMove.Instance.ShakeCamera(0.1f, 0.05f);
            }

            Debug.Log("B");
        }


        JefeController jefe = collision.GetComponent<JefeController>();
        if (jefe != null)
        {
            Debug.Log($"💥 ¡El arma golpeó al JEFE! Daño: {finalDamage}");


            jefe.RecibirDano(finalDamage);

            if (CameraMove.Instance != null)
            {

                CameraMove.Instance.ShakeCamera(0.2f, 0.08f);
            }
        }
    }
}