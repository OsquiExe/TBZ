using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    [Header("Mobile Controls")]
    [SerializeField] public VirtualJoystick joystick;

    [Header("Player Settings")]
    [SerializeField] public Animator anim;
    [SerializeField] public float moveSpeed = 5f;
    public Rigidbody2D rb;
    [SerializeField] public float x;
    [SerializeField] public float y;
    [SerializeField] public Vector2 input;
    [SerializeField] public bool moving;
    [SerializeField] private Vector2 lastMoveDirection;

    [Header("Status & Health")]
    public bool isInvulnerable = false;
    [SerializeField] private float invulnerabilityDuration = 2f;
    private float invulnTimer = 0f;
    private bool isAttackingInvulnerable = false;

    [SerializeField] public float currentHealth;
    [SerializeField] public float maxHealth = 100f;
    [SerializeField] private Image healthBarFill;

    [Header("Inactividad Castigo")]
    [SerializeField] private float timeBeforeDrain = 5f;
    [SerializeField] private float healthDrainRate = 5f;
    private float idleTimer = 0f;

    [Header("Experience")]
    [SerializeField] public int currentExperience;
    [SerializeField] public int maxExperience;
    [SerializeField] public int currentLevel;

    [Header("Aim & Movement")]
    [SerializeField] public Transform Aim;
    [SerializeField] public bool isWalking = false;

    public PlayerAttack playerAttack;

    public delegate Transform GetTransform();
    public static GetTransform GetPlayerTransform;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerAttack = GetComponent<PlayerAttack>();

        GetPlayerTransform = OnGetPlayerTransform;

        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    private void Update()
    {
        GetInput();
        Animate();
        HandleHealthDrain();
        HandleInvulnerability();
    }

    private void FixedUpdate()
    {
        if (playerAttack != null && !playerAttack.isDashing)
        {
            rb.velocity = input * moveSpeed;
        }

        if (isWalking && (playerAttack == null || !playerAttack.isDashing))
        {
            Vector3 vector3 = Vector3.left * input.x + Vector3.down * input.y;
            Aim.rotation = Quaternion.LookRotation(Vector3.forward, vector3);
        }
    }

    private void HandleInvulnerability()
    {
        if (invulnTimer > 0) invulnTimer -= Time.deltaTime;

        bool dashingStatus = playerAttack != null && playerAttack.isDashing;
        isInvulnerable = (invulnTimer > 0) || dashingStatus || isAttackingInvulnerable;
    }

    public void SetAttackInvulnerability(bool state)
    {
        isAttackingInvulnerable = state;
    }

    public void TakeDamage(float damage)
    {
        if (isInvulnerable || currentHealth <= 0) return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthUI();

        if (CameraMove.Instance != null) CameraMove.Instance.ShakeCamera(0.25f, 0.15f);

        if (currentHealth <= 0) Die();
        else invulnTimer = invulnerabilityDuration;
    }

    public void Heal(float amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthUI();
    }

    private void HandleHealthDrain()
    {
        if (currentHealth <= 0) return;

        bool isDashing = playerAttack != null && playerAttack.isDashing;

        if (moving || isDashing) idleTimer = 0f;
        else idleTimer += Time.deltaTime;

        if (idleTimer >= timeBeforeDrain)
        {
            currentHealth -= healthDrainRate * Time.deltaTime;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
            UpdateHealthUI();
            if (currentHealth <= 0) Die();
        }
    }

    private void UpdateHealthUI()
    {
        if (healthBarFill != null) healthBarFill.fillAmount = currentHealth / maxHealth;
    }

    private void Die()
    {
        if (GameManager.Instance != null) GameManager.Instance.TriggerDefeat();
    }

    private void GetInput()
    {
        if (playerAttack != null && playerAttack.isDashing) return;

        x = joystick != null ? joystick.GetHorizontal() : 0f;
        y = joystick != null ? joystick.GetVertical() : 0f;

        if (x == 0 && y == 0)
        {
            x = Input.GetAxisRaw("Horizontal");
            y = Input.GetAxisRaw("Vertical");
        }

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
        if (input != Vector2.zero) lastMoveDirection = input;
    }

    private void Animate()
    {
        moving = (input.magnitude > 0.1f || input.magnitude < -0.1f);

        bool dashingStatus = playerAttack != null && playerAttack.isDashing;
        bool attackingStatus = playerAttack != null && playerAttack.isAttacking;

        if (dashingStatus || attackingStatus)
        {
        
            return;
        }

        if (moving)
        {
            anim.SetFloat("x", x);
            anim.SetFloat("y", y);
        }

        anim.SetBool("Moving", moving);
    }

    private void OnEnable()
    {
        if (ExperienceManager.Instance != null) ExperienceManager.Instance.OnExperienceChange += HandleExperienceChange;
    }

    private void OnDisable()
    {
        if (ExperienceManager.Instance != null) ExperienceManager.Instance.OnExperienceChange -= HandleExperienceChange;
    }

    private void HandleExperienceChange(int newExperience)
    {
        currentExperience += newExperience;
        if (currentExperience >= maxExperience) LevelUp();
    }

    private void LevelUp()
    {
        currentLevel++;
        currentExperience = 0;
        maxExperience += 100;
    }

    Transform OnGetPlayerTransform() { return transform; }
}