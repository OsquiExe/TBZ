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
  
  [Header ("Vida")]
  [SerializeField] private float health, maxHealth = 5f;

  [Header("Knockback")]
  [SerializeField] public Vector2 knockBackForce;

  [Header("Experience")]
  [SerializeField] private int expAmount = 100; 

  private void Awake()
  {
      enemyRb = GetComponent<Rigidbody2D>();
  }

  private void Start()
  {
      target = GameObject.FindGameObjectWithTag("Player"). transform; 
      health = maxHealth;
  }

  private void Update()
  {
      if(target)
      {
          Vector3 direction = (target.position - transform.position). normalized; 
          moveDirection = direction;
      }
  }

  private void FixedUpdate()
  {
      enemyRb.velocity = new Vector2(moveDirection.x, moveDirection.y) * enemySpeed; 
  }

  public void TakeDamage(float damage,Vector2 force)
  {
      health -= damage;
      if ( health <= 0)
      {
        ApplyKnockback(force);
        Destroy(gameObject);
        ExperienceManager.Instance.AddExperience(expAmount);
      }
  }

  public void ApplyKnockback(Vector2 force)
  {
       enemyRb.velocity = Vector2.zero;
       enemyRb.AddForce(force, ForceMode2D.Impulse);
  } 
}
