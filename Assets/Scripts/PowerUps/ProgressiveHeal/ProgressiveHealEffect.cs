using UnityEngine;

public class ProgressiveHealEffect : MonoBehaviour
{
    public int level = 1;
    private float timer = 0f;

    [Header("Settings de Curación")]
    [SerializeField] private float baseHealInterval = 5f;
    [SerializeField] private float minHealInterval = 2f; 

    private PlayerController player;

    private void Start()
    {
        player = GetComponent<PlayerController>();
    }

    private void Update()
    {
        if (player == null) return;

        timer += Time.deltaTime;


        float currentInterval = Mathf.Max(minHealInterval, baseHealInterval - (level * 0.5f));

        if (timer >= currentInterval)
        {
          
            float healAmount = 5f * level;
            player.Heal(healAmount);

            Debug.Log($"[PowerUp] Cura Progresiva Nivel {level} activada. +{healAmount} HP cada {currentInterval}s.");

            timer = 0f;
        }
    }
}