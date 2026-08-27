using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    [Header("Configuración de Cura")]
    [SerializeField] private float healAmount = 25f;

    [Header("VFX")]
    [SerializeField] private Sprite[] healSprites; 

    AudioManager AudioManager;

    private void Awake()
    {
        AudioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerController player = collision.GetComponent<PlayerController>();

            if (player != null)
            {
                player.Heal(healAmount);
                AudioManager.PlaySFX(AudioManager.healSFX);


                if (healSprites != null && healSprites.Length > 0)
                {
                    GameObject vfx = new GameObject("HealVFX");
                    vfx.transform.position = player.transform.position;
                    vfx.AddComponent<SpriteRenderer>().sortingOrder = 50;
                    vfx.AddComponent<VFXPlayer>().PlayAndDestroy(healSprites, 0.08f, player.transform);
                }

                Destroy(gameObject);
            }
        }
    }
}