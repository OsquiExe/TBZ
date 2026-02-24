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

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    
    private void Update()
    {
        GetInput();
        Animate();
    }

    private void FixedUpdate()
    {
        rb.velocity = input * moveSpeed;
        
        //determina la rotación del golpe si está volando. 
        if(isWalking)
        {
            Vector3 vector3 = Vector3.left * input.x + Vector3.down * input.y;
            Aim.rotation = Quaternion.LookRotation(Vector3.forward, vector3);
        }
    }

    private void GetInput()
    {
        //Información para leer los inputs de dirección del jugador.
        x = Input.GetAxisRaw("Horizontal");
        y = Input.GetAxisRaw("Vertical");

        if((x == 0 && y == 0) && (input.x != 0 || input.y != 0))
        {
            isWalking = false;
            lastMoveDirection = input;
            Vector3 vector3 = Vector3.left * lastMoveDirection.x + Vector3.down * lastMoveDirection.y;
            Aim.rotation = Quaternion.LookRotation(Vector3.forward, vector3);
        }
        else if(x != 0 || y != 0)
        {
            isWalking = true;
        }

        input = new Vector2(x,y);
        input.Normalize();
    }

    private void Animate()
    {
        if(input.magnitude > 0.1f || input.magnitude < -0.1f)
        {
            moving = true;
        }
        else
        {
            moving = false;
        }
        if(moving)
        {
            anim.SetFloat("x", x);
            anim.SetFloat("y", y);
        }

        anim.SetBool("Moving", moving);
    }

    private void OnEnable()
    {
        //Subscribe to the event
        ExperienceManager.Instance.OnExperienceChange += HandleExperienceChange;
    }

    private void OnDisable()
    {
        //Unsubscribe from the event
        ExperienceManager.Instance.OnExperienceChange -= HandleExperienceChange;
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
        currentLevel ++;
        currentExperience = 0;

        maxExperience += 100;
    }
}
