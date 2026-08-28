using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    [Header("Configuración de Cura")]
    [SerializeField] private float healAmount = 25f;

    [Header("VFX")]
    [SerializeField] private Sprite[] healSprites; // Recuperamos la variable para tu animación

    AudioManager AudioManager;

    private void Awake()
    {
        // Buscamos el audio con cuidado por si no existe en la escena
        GameObject audioObj = GameObject.FindGameObjectWithTag("Audio");
        if (audioObj != null)
        {
            AudioManager = audioObj.GetComponent<AudioManager>();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerController player = collision.GetComponent<PlayerController>();

            if (player != null)
            {
                player.Heal(healAmount);
                Debug.Log($"🩹 El jugador se curó {healAmount} de vida.");

                // Escudo protector para el sonido
                if (AudioManager != null)
                {
                    AudioManager.PlaySFX(AudioManager.healSFX);
                }

                // Recuperamos la animación visual de las cruces verdes
                if (healSprites != null && healSprites.Length > 0)
                {
                    GameObject vfx = new GameObject("HealVFX");
                    vfx.transform.position = player.transform.position;
                    vfx.AddComponent<SpriteRenderer>().sortingOrder = 50;
                    vfx.AddComponent<VFXPlayer>().PlayAndDestroy(healSprites, 0.08f, player.transform);
                }

                // Destruimos el ítem siempre al final
                Destroy(gameObject);
            }
        }
    }
}