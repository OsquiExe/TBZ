using UnityEngine;

public class AssistantBullet : MonoBehaviour
{
    private Vector2 direction;
    private float speed;
    private float damage;

    public void Setup(Vector2 dir, float spd, float dmg)
    {
        direction = dir;
        speed = spd;
        damage = dmg;

        // La bala se destruye sola a los 2 segundos para no llenar la memoria
        Destroy(gameObject, 2f);
    }

    private void Update()
    {
        // Movimiento de la bala
        transform.Translate(direction * speed * Time.deltaTime, Space.World);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        bool hitSomething = false;

        Enemy enemy = collision.GetComponent<Enemy>();
        if (enemy != null && !enemy.isDead)
        {
            enemy.TakeDamage(damage, direction * 2f);
            hitSomething = true;
        }

        JefeController jefe = collision.GetComponent<JefeController>();
        if (jefe != null)
        {
            jefe.RecibirDano(damage);
            hitSomething = true;
        }

        // Si tocó un enemigo o jefe, la bala desaparece
        if (hitSomething)
        {
            Destroy(gameObject);
        }
    }
}