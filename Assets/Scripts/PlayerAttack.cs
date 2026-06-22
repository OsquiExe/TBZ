using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
   [SerializeField] public GameObject Punch;
   [SerializeField] public Animator animator; 
   [SerializeField] bool isAttacking = false;
   [SerializeField] float attackDuration = 0.3f;
   [SerializeField] float attackTimer = 0f;
   [SerializeField] public int damage = 1;
   [SerializeField] public float knockBackForce = 5f;
   [SerializeField] private Vector2 lastDirection = Vector2.down; 

   private void OnTriggerEnter2D(Collider2D collision)
   {
       Enemy enemy = collision.GetComponent<Enemy>();
       if (enemy != null)
       {
            Vector2 direction = (collision.transform.position - transform.position).normalized;
            enemy.TakeDamage(damage, direction * knockBackForce);
       }
   }

   public void Update()
   {
      CheckAttackTimer();

      if (Input.GetKeyDown(KeyCode.E)|| Input.GetMouseButtonDown(0))
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
      }
      else if(isAttacking)
      {
          Punch.SetActive(false);
          isAttacking = false;
      }
      animator.SetFloat("attackX", lastDirection.x);
      animator.SetFloat("attackY", lastDirection.y);

      animator.SetTrigger("Attack");
   }

   public void CheckAttackTimer()
   {
      if (isAttacking)
      {
          attackTimer += Time.deltaTime;
          if(attackTimer >= attackDuration)
          {
              attackTimer = 0;
              isAttacking = false;
              Punch.SetActive(false);
          }
      }
   }

   public void Movement()
   {
       float horizontal = Input.GetAxisRaw("Horizontal");
       float vertical = Input.GetAxisRaw("Vertical");

       Vector2 moveDirection = new Vector2(horizontal, vertical);

       if(moveDirection != Vector2.zero)
       {
           lastDirection = moveDirection.normalized;
       }
   }
}
