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
    [SerializeField] private float knockbackDuration = 0.3f;
    [SerializeField] private float secondaryDamage = 2f;
    [SerializeField] private float damageToPlayer = 10f;

    [Header("Experience")]
    [SerializeField] private int expAmount = 100;

    public bool isDead = false;
    private bool isKnockedBack = false;
    private bool canDamageOtherEnemy = false;

    private void Awake()
    {
        enemyRb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        health = maxHealth;
    }

    private void Update()
    {
        if (isDead) return;

        if (target && !isKnockedBack)
        {
            Vector3 direction = (target.position - transform.position).normalized;
            moveDirection = direction;
        }
    }

    private void FixedUpdate()
    {
        if (isDead || isKnockedBack) return;

        enemyRb.velocity = new Vector2(moveDirection.x, moveDirection.y) * enemySpeed;
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

        if (ExperienceManager.Instance != null)
            ExperienceManager.Instance.AddExperience(expAmount);

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
                    Debug.Log($"💥 El enemigo golpeó al jugador por {damageToPlayer} de daño.");
                    player.TakeDamage(damageToPlayer);
                }
            }
        }
    }
}