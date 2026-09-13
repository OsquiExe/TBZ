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

    [Header("Feedback Visual de Daño")]
    [SerializeField] private float flashDuration = 0.1f; 
    private SpriteRenderer[] allRenderers;
    private Material[] originalMaterials; 
    private Material whiteFlashMaterial; 

    [Header("Vida")]
    [SerializeField] private float health;
    [SerializeField] private float maxHealth = 5f;

    [Header("Knockback & Colisiones")]
    [SerializeField] public Vector2 knockBackForce;
    [SerializeField] private float knockbackDuration = 0.2f;
    [SerializeField] private float secondaryDamage = 2f;
    [SerializeField] private float damageToPlayer = 5f;

    [Header("Experience Drop Settings")]
    [SerializeField] private int expAmount = 100;
    [SerializeField] private GameObject xpPickupPrefab;

    [Header("Dibi-Coins Drop Settings")]
    [SerializeField] private GameObject coinPrefab;
    [Range(0f, 100f)]
    [SerializeField] private float coinDropChance = 20f;

    [Header("IA Evasión de Obstáculos")]
    [SerializeField] private float detectionRadius = 1.5f;
    [SerializeField] private LayerMask obstacleLayer;

    public bool isDead = false;
    private bool isKnockedBack = false;
    private bool canDamageOtherEnemy = false;
    bool takingDmg = false;

    private void Awake()
    {
        enemyRb = GetComponent<Rigidbody2D>();

        allRenderers = GetComponentsInChildren<SpriteRenderer>();
        originalMaterials = new Material[allRenderers.Length];

        for (int i = 0; i < allRenderers.Length; i++)
        {
            originalMaterials[i] = allRenderers[i].material;
        }


        whiteFlashMaterial = new Material(Shader.Find("GUI/Text Shader"));
        whiteFlashMaterial.color = Color.white;
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

    public void SetupEnemyStats(float healthMultiplier, float damageMultiplier, float speedMultiplier)
    {
        maxHealth *= healthMultiplier;
        health = maxHealth;
        damageToPlayer *= damageMultiplier;
        enemySpeed *= speedMultiplier;
    }

    private void Update()
    {
        if (isDead) return;

        if (target && !isKnockedBack)
        {
            Vector2 directionToPlayer = (target.position - transform.position).normalized;
            Vector2 avoidance = Vector2.zero;

            Collider2D obstacleHit = Physics2D.OverlapCircle(transform.position, detectionRadius, obstacleLayer);

            if (obstacleHit != null)
            {
                Vector2 closestPoint = obstacleHit.ClosestPoint(transform.position);
                Vector2 pushAway = ((Vector2)transform.position - closestPoint);
                float distance = pushAway.magnitude;

                if (distance < 0.01f)
                {
                    pushAway = -directionToPlayer;
                    distance = 0.1f;
                }

                Vector2 normal = pushAway.normalized;
                Vector2 tangent1 = new Vector2(-normal.y, normal.x);
                Vector2 tangent2 = new Vector2(normal.y, -normal.x);
                Vector2 bestTangent = (Vector2.Dot(tangent1, directionToPlayer) > Vector2.Dot(tangent2, directionToPlayer)) ? tangent1 : tangent2;

                float avoidanceWeight = Mathf.Clamp01(1f - (distance / detectionRadius));
                avoidance = (bestTangent * 2f + normal * 3f) * avoidanceWeight;
            }

            Vector2 separation = Vector2.zero;
            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, 0.6f);

            foreach (var col in hitColliders)
            {
                if (col.gameObject != gameObject && col.CompareTag("Enemy"))
                {
                    Vector2 diff = transform.position - col.transform.position;
                    separation += diff.normalized / Mathf.Max(diff.magnitude, 0.1f);
                }
            }

            anim.SetFloat("rot", (Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg) + 90, 0.1f, Time.deltaTime);
            moveDirection = (directionToPlayer + avoidance + (separation * 0.4f)).normalized;
        }
    }

    private void FixedUpdate()
    {
        if (isDead || isKnockedBack) return;
        enemyRb.MovePosition(enemyRb.position + moveDirection * enemySpeed * Time.fixedDeltaTime);
    }

    public void TakeDamage(float damage, Vector2 force)
    {
        if (isDead || takingDmg) return;


        if (gameObject.activeInHierarchy)
        {
            StartCoroutine(DamageFlashRoutine());
        }

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


    private IEnumerator DamageFlashRoutine()
    {
        takingDmg = true;
        for (int i = 0; i < allRenderers.Length; i++)
        {
            if (allRenderers[i] != null) allRenderers[i].material = whiteFlashMaterial;
        }


        yield return new WaitForSeconds(flashDuration);

  
        for (int i = 0; i < allRenderers.Length; i++)
        {
            if (allRenderers[i] != null) allRenderers[i].material = originalMaterials[i];
        }
        takingDmg = false;
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
        anim.SetTrigger("death");

        if (xpPickupPrefab != null)
        {
            GameObject droppedXp = Instantiate(xpPickupPrefab, transform.position, Quaternion.identity);
            ExperiencePickup pickupScript = droppedXp.GetComponent<ExperiencePickup>();
            if (pickupScript != null)
            {
                pickupScript.Setup(expAmount);
            }
        }

        if (WaveManager.Instance != null && WaveManager.Instance.currentWave % 4 == 0)
        {
            if (coinPrefab != null)
            {
                float randomRoll = Random.Range(0f, 100f);
                if (randomRoll <= coinDropChance)
                {
                    Vector3 coinPos = transform.position + new Vector3(Random.Range(-0.3f, 0.3f), Random.Range(-0.3f, 0.3f), 0);
                    Instantiate(coinPrefab, coinPos, Quaternion.identity);
                }
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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}