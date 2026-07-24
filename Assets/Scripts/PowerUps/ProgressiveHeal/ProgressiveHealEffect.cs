using UnityEngine;

public class ProgressiveHealEffect : MonoBehaviour
{
    public int level = 1;
    private float timer = 0f; 
    private float healInterval = 5f; 

    private PlayerController player;

    private void Start()
    {
        player = GetComponent<PlayerController>();
    }

    private void Update()
    {
        if (player == null) return;

        timer += Time.deltaTime;

        if (timer >= healInterval)
        {
     
            float healAmount = 5f * level;
            player.Heal(healAmount);

            Debug.Log($"[PowerUp] Cura Progresiva Nivel {level} activada. +{healAmount} HP.");

            timer = 0f; 
        }
    }
}