using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] public GameObject Punch;
    [SerializeField] public Animator animator;
    [SerializeField] public bool isAttacking = false;

    [Header("Attack Settings")]
    [SerializeField] float attackDuration = 0.25f;
    [SerializeField] float autoAttackCooldown = 0.5f;

    [Header("Auto Attack Detection")]
    [SerializeField] private float enemyDetectionRadius = 2.5f;

    private float autoAttackTimer = 0f;
    private float activeAttackTimer = 0f;

    [Header("Dash Settings")]
    public bool isDashing = false;
    [SerializeField] private float dashSpeed = 40f;
    [SerializeField] private float dashDuration = 0.35f;
    [SerializeField] private float dashCooldown = 0.5f;
    [SerializeField] private float dashDamage = 2f;
    [SerializeField] private float dashKnockbackForce = 10f;
    [SerializeField] private float minimumSwipeDistance = 50f;

    [SerializeField] private LayerMask playerLayer;
    private Camera mainCamera;
    private bool touchStartedOnPlayer = false;

    private float dashCooldownTimer = 0f;
    private Vector2 dashDirection;
    [SerializeField] private Vector2 lastDirection = Vector2.down;

    private Vector2 touchStartPos;
    private Vector2 touchEndPos;

    private PlayerController playerController;

    AudioManager AudioManager;

    private void Awake()
    {
        AudioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

    private void Start()
    {
        playerController = GetComponent<PlayerController>();
        autoAttackTimer = autoAttackCooldown;

        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("PlayerAttack: No se encontró la cámara principal tagged como 'MainCamera'.");
        }
    }

    public void Update()
    {
        SyncMovementDirection();
        HandleAutoAttack();
        CheckAttackTimer();
        HandleDashInput();
    }

    public void SyncMovementDirection()
    {
        if (playerController != null && playerController.input != Vector2.zero && !isDashing)
        {
            lastDirection = playerController.input.normalized;
        }
    }

    private void HandleDashInput()
    {
        if (PowerUpManager.Instance != null && PowerUpManager.Instance.uiPowerUpSelect != null && PowerUpManager.Instance.uiPowerUpSelect.activeInHierarchy) return;

        if (dashCooldownTimer > 0) dashCooldownTimer -= Time.deltaTime;

        if (Input.touchCount > 0)
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                Touch touch = Input.GetTouch(i);

                if (touch.phase == TouchPhase.Began)
                {
                    CheckTouchBegan(touch.position);
                }
                else if (touch.phase == TouchPhase.Ended)
                {
                    touchEndPos = touch.position;
                    ProcessSwipeGesture();
                }
            }
        }
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                CheckTouchBegan(Input.mousePosition);
            }
            else if (Input.GetMouseButtonUp(0))
            {
                touchEndPos = Input.mousePosition;
                ProcessSwipeGesture();
            }
        }
    }

    private void CheckTouchBegan(Vector2 screenPosition)
    {
        if (mainCamera == null) return;

        touchStartPos = screenPosition;
        touchStartedOnPlayer = false;

        Vector3 worldPos = mainCamera.ScreenToWorldPoint(screenPosition);
        Vector2 worldPos2D = new Vector2(worldPos.x, worldPos.y);

        Collider2D[] playerColliders = Physics2D.OverlapCircleAll(worldPos2D, 1.5f, playerLayer);

        foreach (var col in playerColliders)
        {
            if (col.gameObject == gameObject)
            {
                touchStartedOnPlayer = true;
                break;
            }
        }
    }

    private void ProcessSwipeGesture()
    {
        if (!touchStartedOnPlayer) return;

        touchStartedOnPlayer = false;

        float distance = Vector2.Distance(touchStartPos, touchEndPos);

        if (distance >= minimumSwipeDistance)
        {
            if (dashCooldownTimer <= 0f && !isDashing)
            {
                Vector2 swipeDir = (touchEndPos - touchStartPos).normalized;
                StartCoroutine(DashRoutine(swipeDir));
                AudioManager.PlaySFX(AudioManager.dashSFX);
            }
        }
    }

    private IEnumerator DashRoutine(Vector2 dir)
    {
        isDashing = true;
        dashCooldownTimer = dashCooldown;
        dashDirection = dir;
        lastDirection = dir;

        OnAttack();

        float elapsed = 0f;

        while (elapsed < dashDuration)
        {
            elapsed += Time.fixedDeltaTime;
            float t = elapsed / dashDuration;

            float speedMultiplier = Mathf.Pow(1f - t, 2f);
            float currentSpeed = dashSpeed * speedMultiplier;

            if (playerController != null && playerController.rb != null)
            {
                playerController.rb.velocity = dashDirection * currentSpeed;
            }

            yield return new WaitForFixedUpdate();
        }

        if (playerController != null && playerController.rb != null)
        {
            playerController.rb.velocity = Vector2.zero;
        }

        isDashing = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDashing)
        {
            float totalDamage = dashDamage;
            DamageIncreaseEffect dmgEffect = GetComponent<DamageIncreaseEffect>();

            if (dmgEffect != null)
            {
                totalDamage += dmgEffect.GetExtraDamage();
            }

            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(totalDamage, dashDirection * dashKnockbackForce);
                AudioManager.PlaySFX(AudioManager.punchSFX);
            }

            JefeController jefe = collision.gameObject.GetComponent<JefeController>();
            if (jefe != null)
            {
                jefe.RecibirDano(totalDamage);
                AudioManager.PlaySFX(AudioManager.punchSFX);
            }
        }
    }

    private void HandleAutoAttack()
    {
        if (!isAttacking)
        {
            if (autoAttackTimer > 0f) autoAttackTimer -= Time.deltaTime;

            if (autoAttackTimer <= 0f && IsEnemyNearby())
            {
                OnAttack();
            }
        }
    }


    private bool IsEnemyNearby()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, enemyDetectionRadius);

        foreach (var hit in hitColliders)
        {
            bool esObjetivoValido = false;

            Enemy enemy = hit.GetComponent<Enemy>();
            if (enemy != null && !enemy.isDead) esObjetivoValido = true;

            JefeController jefe = hit.GetComponent<JefeController>();
            if (jefe != null) esObjetivoValido = true;

            if (esObjetivoValido)
            {
   
                Vector2 direccionAlEnemigo = (hit.transform.position - transform.position).normalized;


                float anguloFrente = Vector2.Dot(lastDirection, direccionAlEnemigo);


                if (anguloFrente > 0.25f)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public void OnAttack()
    {
        Punch.SetActive(true);
        isAttacking = true;
        activeAttackTimer = 0f;
        autoAttackTimer = autoAttackCooldown;

        if (playerController != null)
        {
            playerController.SetAttackInvulnerability(true);
        }

        animator.ResetTrigger("Attack");

        animator.SetFloat("attackx", lastDirection.x);
        animator.SetFloat("attacky", lastDirection.y);

        animator.SetBool("Moving", true);
        animator.SetFloat("x", lastDirection.x);
        animator.SetFloat("y", lastDirection.y);

        animator.SetTrigger("Attack");

        animator.Play("FightTree", 0, 0f);
    }

    public void CheckAttackTimer()
    {
        if (isAttacking)
        {
            activeAttackTimer += Time.deltaTime;
            if (activeAttackTimer >= attackDuration)
            {
                activeAttackTimer = 0f;
                isAttacking = false;
                Punch.SetActive(false);

                if (playerController != null)
                {
                    playerController.SetAttackInvulnerability(false);
                }
            }
        }
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, enemyDetectionRadius);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, (Vector2)transform.position + (lastDirection * enemyDetectionRadius));
    }
}