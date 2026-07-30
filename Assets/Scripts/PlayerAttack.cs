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

    [Header("Dash Settings (Mitad Superior)")]
    public bool isDashing = false;
    [SerializeField] private float dashSpeed = 15f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 0.5f;
    [SerializeField] private float dashDamage = 2f;
    [SerializeField] private float dashKnockbackForce = 10f;
    [SerializeField] private float minimumSwipeDistance = 50f;

    private float dashCooldownTimer = 0f;
    private Vector2 dashDirection;
    [SerializeField] private Vector2 lastDirection = Vector2.down;

    private Vector2 touchStartPos;
    private Vector2 touchEndPos;

    private PlayerController playerController;

    private void Start()
    {
        playerController = GetComponent<PlayerController>();
        autoAttackTimer = autoAttackCooldown;
    }

    public void Update()
    {
        SyncMovementDirection();
        HandleAutoAttack();
        CheckAttackTimer();
        HandleDashInput();
    }

    private void FixedUpdate()
    {
        if (isDashing && playerController != null && playerController.rb != null)
        {
            playerController.rb.velocity = dashDirection * dashSpeed;
        }
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

                if (touch.position.y <= Screen.height / 2f) continue;
                if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(touch.fingerId)) continue;

                if (touch.phase == TouchPhase.Began)
                {
                    touchStartPos = touch.position;
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
                if (Input.mousePosition.y <= Screen.height / 2f) return;
                if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;
                touchStartPos = Input.mousePosition;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                if (Input.mousePosition.y <= Screen.height / 2f) return;
                if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;
                touchEndPos = Input.mousePosition;
                ProcessSwipeGesture();
            }
        }
    }

    private void ProcessSwipeGesture()
    {
        float distance = Vector2.Distance(touchStartPos, touchEndPos);

        if (distance >= minimumSwipeDistance)
        {
            if (dashCooldownTimer <= 0f && !isDashing)
            {
                Vector2 swipeDir = (touchEndPos - touchStartPos).normalized;
                StartCoroutine(DashRoutine(swipeDir));
            }
        }
    }

    private IEnumerator DashRoutine(Vector2 dir)
    {
        isDashing = true;
        dashCooldownTimer = dashCooldown;
        dashDirection = dir;
        lastDirection = dir;

        OnAttack(); // Acá disparamos el golpe obligatorio

        yield return new WaitForSeconds(dashDuration);

        isDashing = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDashing)
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            if (enemy != null)
            {
                float totalDamage = dashDamage;
                DamageIncreaseEffect dmgEffect = GetComponent<DamageIncreaseEffect>();

                if (dmgEffect != null)
                {
                    totalDamage += dmgEffect.GetExtraDamage();
                }

                enemy.TakeDamage(totalDamage, dashDirection * dashKnockbackForce);
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
            Enemy enemy = hit.GetComponent<Enemy>();
            if (enemy != null && !enemy.isDead)
            {
                return true;
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

        // 1. Limpiamos cualquier trigger viejo trabado
        animator.ResetTrigger("Attack");

        // 2. Le pasamos la dirección al FightTree
        animator.SetFloat("attackx", lastDirection.x);
        animator.SetFloat("attacky", lastDirection.y);

        // 3. Forzamos el Moving y la dirección normal para que no vuelva a Idle
        animator.SetBool("Moving", true);
        animator.SetFloat("x", lastDirection.x);
        animator.SetFloat("y", lastDirection.y);

        // 4. Disparamos el Trigger normal por seguridad
        animator.SetTrigger("Attack");

        // 5. ¡LA SOLUCIÓN! Forzamos la reproducción inmediata del árbol de pelea
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
    }
}