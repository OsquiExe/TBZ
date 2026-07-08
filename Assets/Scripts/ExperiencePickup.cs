using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExperiencePickup : MonoBehaviour
{
    private int expAmount = 0;
    private Transform playerTransform;
    private bool isBeingAttracted = false;

    [Header("Configuración del Imán")]
    [SerializeField] private float magnetRadius = 3f;
    [SerializeField] private float initialMoveSpeed = 2f; 
    [SerializeField] private float acceleration = 5f; 

    private float currentSpeed;

    public void Setup(int amount)
    {
        expAmount = amount;
        currentSpeed = initialMoveSpeed;


        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
    }

    private void Update()
    {
        if (playerTransform == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        if (!isBeingAttracted && distanceToPlayer <= magnetRadius)
        {
            isBeingAttracted = true;
        }

 
        if (isBeingAttracted)
        {
            currentSpeed += acceleration * Time.deltaTime; 
            transform.position = Vector2.MoveTowards(transform.position, playerTransform.position, currentSpeed * Time.deltaTime);
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (ExperienceManager.Instance != null)
            {
           
                ExperienceManager.Instance.AddExperience(expAmount);
            }

           
            Destroy(gameObject);
        }
    }
}