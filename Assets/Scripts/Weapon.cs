using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] public float damage = 5f;
    [SerializeField] public float knockBackForce = 4f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Enemy enemy = collision.GetComponent<Enemy>();

        if (enemy != null && !enemy.isDead)
        {
            Vector2 direction = (collision.transform.position - transform.position).normalized;
            Vector2 force = direction * knockBackForce;

          
            float finalDamage = damage;

           
            DamageIncreaseEffect dmgEffect = GetComponentInParent<DamageIncreaseEffect>();

           
            if (dmgEffect != null)
            {
                finalDamage += dmgEffect.GetExtraDamage();
            }

     
            Debug.Log($"⚔️ El arma golpeó al enemigo. Aplicando {finalDamage} de daño total (Base: {damage} + PowerUp).");

   
            enemy.TakeDamage(finalDamage, force);

            if (CameraMove.Instance != null)
            {
                CameraMove.Instance.ShakeCamera(0.1f, 0.05f);
            }
        }
    }
}