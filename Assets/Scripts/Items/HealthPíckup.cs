using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    [Header("Configuración de Cura")]
    [SerializeField] private float healAmount = 25f; 

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        if (collision.CompareTag("Player"))
        {
            PlayerController player = collision.GetComponent<PlayerController>();

            if (player != null)
            {
                
                player.Heal(healAmount);

                Debug.Log($"🩹 El jugador se curó {healAmount} de vida.");

              
                Destroy(gameObject);
            }
        }
    }
}