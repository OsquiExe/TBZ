using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] public float damage = 1f;
    [SerializeField] public float knockBackForce = 5f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Enemy enemy = collision.GetComponent<Enemy>();
        
        //Condicional para que el enemigo reciba daño.
        if(enemy != null)
        {
            Vector2 direction = (collision.transform.position - transform.position).normalized;

            Vector2 force = direction * knockBackForce;

            enemy.TakeDamage(damage, force);
        }
    }
}
