using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] public Animator anim;
    [SerializeField] public float moveSpeed = 5f;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] public float x;
    [SerializeField] public float y;
    [SerializeField] public Vector2 input;
    [SerializeField] public bool moving;
    [SerializeField] private Vector2 lastMoveDirection;
    [SerializeField] public int currentHealth, maxHealth = 10, currentExperience, maxExperience, currentLevel;

    [SerializeField] public Transform Aim;
    [SerializeField] public bool isWalking = false;

    [Header("Dash Settings")]
    [SerializeField] private float dashSpeed = 15f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 1f;
    [SerializeField] private float doubleClickTime = 0.3f; 
    [SerializeField] private float dashDamage = 2f;
    [SerializeField] private float dashKnockbackForce = 10f;

    private bool isDashing = false;
    private float dashCooldownTimer = 0f;
    private float lastClickTime = 0f;
    private Vector2 dashDirection;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        GetInput();
        HandleDashInput();
        Animate();
    }

    private void FixedUpdate()
    {
        
        if (isDashing)
        {
            rb.velocity = dashDirection * dashSpeed;
        }
        else
        {
            rb.velocity = input * moveSpeed;
        }

    
        if (isWalking && !isDashing)
        {
            Vector3 vector3 = Vector3.left * input.x + Vector3.down * input.y;
            Aim.rotation = Quaternion.LookRotation(Vector3.forward, vector3);
        }
    }

    private void GetInput()
    {
        if (isDashing) return; 

        x = Input.GetAxisRaw("Horizontal");
        y = Input.GetAxisRaw("Vertical");

        if ((x == 0 && y == 0) && (input.x != 0 || input.y != 0))
        {
            isWalking = false;
            lastMoveDirection = input;
            Vector3 vector3 = Vector3.left * lastMoveDirection.x + Vector3.down * lastMoveDirection.y;
            Aim.rotation = Quaternion.LookRotation(Vector3.forward, vector3);
        }
        else if (x != 0 || y != 0)
        {
            isWalking = true;
        }

        input = new Vector2(x, y).normalized;
        if (input != Vector2.zero)
        {
            lastMoveDirection = input;
        }
    }

    private void HandleDashInput()
    {
        if (dashCooldownTimer > 0)
        {
            dashCooldownTimer -= Time.deltaTime;
        }

     
        if (Input.GetMouseButtonDown(0))
        {
            float timeSinceLastClick = Time.time - lastClickTime;

            if (timeSinceLastClick <= doubleClickTime && dashCooldownTimer <= 0f && !isDashing)
            {
                StartCoroutine(DashRoutine());
            }

            lastClickTime = Time.time;
        }
    }

    private IEnumerator DashRoutine()
    {
        isDashing = true;
        dashCooldownTimer = dashCooldown;

       
        dashDirection = lastMoveDirection == Vector2.zero ? Vector2.down : lastMoveDirection.normalized;

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
               
                enemy.TakeDamage(dashDamage, dashDirection * dashKnockbackForce);
            }
        }
    }

    private void Animate()
    {
        if (input.magnitude > 0.1f || input.magnitude < -0.1f)
        {
            moving = true;
        }
        else
        {
            moving = false;
        }
        if (moving && !isDashing)
        {
            anim.SetFloat("x", x);
            anim.SetFloat("y", y);
        }

        anim.SetBool("Moving", moving);
    }

    private void OnEnable()
    {
        if (ExperienceManager.Instance != null)
        {
            ExperienceManager.Instance.OnExperienceChange += HandleExperienceChange;
        }
    }

    private void OnDisable()
    {
        if (ExperienceManager.Instance != null)
        {
            ExperienceManager.Instance.OnExperienceChange -= HandleExperienceChange;
        }
    }

    private void HandleExperienceChange(int newExperience)
    {
        currentExperience += newExperience;
        if (currentExperience >= maxExperience)
        {
            LevelUp();
        }
    }

    private void LevelUp()
    {
        currentLevel++;
        currentExperience = 0;
        maxExperience += 100;
    }
}