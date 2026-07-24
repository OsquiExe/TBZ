using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Enemy : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Animator anim;
    [SerializeField] public float enemySpeed = 5f;
    [SerializeField] private Rigidbody2D enemyRb;
    [SerializeField] private Transform target;
    [SerializeField] private Vector2 moveDirection;

    [Header("Vida")]
    [SerializeField] private float health, maxHealth = 5f;

    [Header("Knockback & Colisiones")]
    [SerializeField] public Vector2 knockBackForce;
    [SerializeField] private float knockbackDuration = 0.2f;
    [SerializeField] private float secondaryDamage = 2f;
    [SerializeField] private float damageToPlayer = 5f;

    [Header("Experience Drop Settings")]
    [SerializeField] private int expAmount = 100;
    [SerializeField] private GameObject xpPickupPrefab;

    public bool isDead = false;
    private bool isKnockedBack = false;
    private bool canDamageOtherEnemy = false;

    private void Awake()
    {
        enemyRb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            target = playerObj.transform;
        }
        health = maxHealth;
    }

    private void Update()
    {
        if (isDead) return;

        if (target && !isKnockedBack)
        {
            // Dirección principal hacia el jugador
            Vector2 directionToPlayer = (target.position - transform.position).normalized;

            // LÓGICA DE SEPARACIÓN (Evita que se fusionen en 1 solo mega-enemigo)
            Vector2 separation = Vector2.zero;
            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, 0.4f);

            foreach (var hit in hitColliders)
            {
                // Si toca a otro enemigo, empuja en la dirección opuesta
                if (hit.gameObject != gameObject && hit.CompareTag("Enemy"))
                {
                    Vector2 diff = transform.position - hit.transform.position;
                    // Mientras más cerca estén, más fuerte se repelen
                    separation += diff.normalized / Mathf.Max(diff.magnitude, 0.1f);
                }
            }

            // Mezclamos la persecución con la separación
            moveDirection = (directionToPlayer + (separation * 0.2f)).normalized;
        }
    }

    private void FixedUpdate()
    {
        if (isDead || isKnockedBack) return;

        // MovePosition es mucho más suave y respeta las colisiones para que no se superpongan
        enemyRb.MovePosition(enemyRb.position + moveDirection * enemySpeed * Time.fixedDeltaTime);
    }

    public void TakeDamage(float damage, Vector2 force)
    {
        if (isDead) return;

        health -= damage;
        if (health <= 0)
        {
            Die(force);
        }
        else
        {
            ApplyKnockback(force);
        }
    }

    public void ApplyKnockback(Vector2 force)
    {
        isKnockedBack = true;
        canDamageOtherEnemy = true;

        enemyRb.velocity = Vector2.zero;
        enemyRb.AddForce(force, ForceMode2D.Impulse);

        StartCoroutine(ResetKnockbackState());
    }

    private IEnumerator ResetKnockbackState()
    {
        yield return new WaitForSeconds(knockbackDuration);
        isKnockedBack = false;
        canDamageOtherEnemy = false;
    }

    private void Die(Vector2 force)
    {
        isDead = true;

        if (xpPickupPrefab != null)
        {
            GameObject droppedXp = Instantiate(xpPickupPrefab, transform.position, Quaternion.identity);
            ExperiencePickup pickupScript = droppedXp.GetComponent<ExperiencePickup>();
            if (pickupScript != null)
            {
                pickupScript.Setup(expAmount);
            }
        }

        if (WaveManager.Instance != null)
            WaveManager.Instance.OnEnemyKilled();

        enemyRb.velocity = Vector2.zero;
        enemyRb.AddForce(force * 1.5f, ForceMode2D.Impulse);

        Collider2D coll = GetComponent<Collider2D>();
        if (coll != null) coll.enabled = false;

        Destroy(gameObject, 0.4f);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isKnockedBack && canDamageOtherEnemy)
        {
            Enemy otherEnemy = collision.gameObject.GetComponent<Enemy>();

            if (otherEnemy != null && !otherEnemy.isDead && otherEnemy != this)
            {
                otherEnemy.TakeDamage(secondaryDamage, Vector2.zero);
                canDamageOtherEnemy = false;
            }
        }

        TryDamagePlayer(collision);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        TryDamagePlayer(collision);
    }

    private void TryDamagePlayer(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();

            if (player != null)
            {
                if (!player.isInvulnerable)
                {
                    player.TakeDamage(damageToPlayer);
                }
            }
        }
    }
}