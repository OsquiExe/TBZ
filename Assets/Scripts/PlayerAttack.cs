using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] public GameObject Punch;
    [SerializeField] public Animator animator;
    [SerializeField] public bool isAttacking = false;
    [SerializeField] float attackDuration = 0.3f;
    [SerializeField] float attackTimer = 0f;
    [SerializeField] public int damage = 1;
    [SerializeField] public float knockBackForce = 5f;
    [SerializeField] private Vector2 lastDirection = Vector2.down;

    private PlayerController playerController;

    private void Start()
    {
        playerController = GetComponent<PlayerController>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Enemy enemy = collision.GetComponent<Enemy>();
        if (enemy != null)
        {
            Vector2 direction = (collision.transform.position - transform.position).normalized;
            enemy.TakeDamage(damage, direction * knockBackForce);

            
            if (CameraMove.Instance != null)
            {
                CameraMove.Instance.ShakeCamera(0.1f, 0.05f);
            }
        }
    }

    public void Update()
    {
        Movement();
        CheckAttackTimer();

        if (Input.GetKeyDown(KeyCode.E) || Input.GetMouseButtonDown(0))
        {
            OnAttack();
        }
    }

    public void OnAttack()
    {
        if (!isAttacking)
        {
            Punch.SetActive(true);
            isAttacking = true;

            
            if (playerController != null)
            {
                playerController.SetAttackInvulnerability(true);
            }
        }

        animator.SetFloat("attackx", lastDirection.x);
        animator.SetFloat("attacky", lastDirection.y);
        animator.SetTrigger("Attack");
    }

    public void CheckAttackTimer()
    {
        if (isAttacking)
        {
            attackTimer += Time.deltaTime;
            if (attackTimer >= attackDuration)
            {
                attackTimer = 0;
                isAttacking = false;
                Punch.SetActive(false);

             
                if (playerController != null)
                {
                    playerController.SetAttackInvulnerability(false);
                }
            }
        }
    }

    public void Movement()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector2 moveDirection = new Vector2(horizontal, vertical);

        if (moveDirection != Vector2.zero)
        {
            lastDirection = moveDirection.normalized;
        }
    }
}