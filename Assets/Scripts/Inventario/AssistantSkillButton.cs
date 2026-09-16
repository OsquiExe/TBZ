using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AssistantSkillButton : MonoBehaviour
{
    [Header("Interfaz")]
    public TextMeshProUGUI cooldownText;

    private Button skillButton;
    private EquipmentItemSO currentAssistant;
    private Transform playerTransform;

    private bool isCooldown = false;
    private float forceFieldRadius = 3.5f;
    private bool isEquipped = false;

    private Vector2 lastPlayerDirection = Vector2.down; // Para la Metralleta

    private void Awake()
    {
        skillButton = GetComponent<Button>();
        skillButton.onClick.AddListener(ActivateSkill);

        if (cooldownText != null) cooldownText.text = "";
    }

    private void Start()
    {
        if (!isEquipped) gameObject.SetActive(false);
    }

    private void Update()
    {
        // Guardamos constantemente hacia dónde mira el jugador para la metralleta
        if (playerTransform != null)
        {
            PlayerController pc = playerTransform.GetComponent<PlayerController>();
            if (pc != null && pc.input != Vector2.zero)
            {
                lastPlayerDirection = pc.input.normalized;
            }
        }
    }

    public void Setup(EquipmentItemSO assistant, Transform player)
    {
        currentAssistant = assistant;
        playerTransform = player;
        isEquipped = true;
        gameObject.SetActive(true);
    }

    private void ActivateSkill()
    {
        if (isCooldown || currentAssistant == null || playerTransform == null) return;
        StartCoroutine(SkillRoutine());
    }

    private IEnumerator SkillRoutine()
    {
        isCooldown = true;
        skillButton.interactable = false;
        if (cooldownText != null) cooldownText.text = "ON";

        GameObject vfx = null;
        if (currentAssistant.buffVFXPrefab != null)
        {
            vfx = Instantiate(currentAssistant.buffVFXPrefab, playerTransform);
            vfx.transform.localPosition = Vector3.zero;
        }

        float timer = 0f;
        float damageTickTimer = 0f;

        while (timer < currentAssistant.buffDuration)
        {
            timer += Time.deltaTime;
            damageTickTimer += Time.deltaTime;

            // LÓGICA 1: CAMPO DE FUERZA
            if (currentAssistant.buffType == AssistantBuffType.CampoDeFuerza && damageTickTimer >= 0.5f)
            {
                damageTickTimer = 0f;
                Collider2D[] hitColliders = Physics2D.OverlapCircleAll(playerTransform.position, forceFieldRadius);

                foreach (var hit in hitColliders)
                {
                    Enemy enemy = hit.GetComponent<Enemy>();
                    if (enemy != null && !enemy.isDead) enemy.TakeDamage(10f, Vector2.zero);

                    JefeController jefe = hit.GetComponent<JefeController>();
                    if (jefe != null) jefe.RecibirDano(10f);
                }
            }

            // LÓGICA 2: METRALLETA (Dispara cada 0.15 segundos)
            else if (currentAssistant.buffType == AssistantBuffType.Metralleta && damageTickTimer >= 0.15f)
            {
                damageTickTimer = 0f;
                if (currentAssistant.projectilePrefab != null)
                {
                    GameObject bullet = Instantiate(currentAssistant.projectilePrefab, playerTransform.position, Quaternion.identity);

                    // Rotamos la bala para que mire hacia donde viaja
                    float angle = Mathf.Atan2(lastPlayerDirection.y, lastPlayerDirection.x) * Mathf.Rad2Deg;
                    bullet.transform.rotation = Quaternion.Euler(0, 0, angle);

                    AssistantBullet bulletScript = bullet.GetComponent<AssistantBullet>();
                    if (bulletScript == null) bulletScript = bullet.AddComponent<AssistantBullet>();

                    // Daño base de la bala + Daño extra del jugador
                    float finalDmg = 3f;
                    PlayerController pc = playerTransform.GetComponent<PlayerController>();
                    if (pc != null) finalDmg += pc.bonusDamage;

                    // Dirección, Velocidad y Daño
                    bulletScript.Setup(lastPlayerDirection, 15f, finalDmg);
                }
            }

            yield return null;
        }

        if (vfx != null) Destroy(vfx);

        // Cuenta regresiva
        float cdTimer = currentAssistant.buffCooldown;
        while (cdTimer > 0f)
        {
            cdTimer -= Time.deltaTime;
            if (cooldownText != null) cooldownText.text = Mathf.CeilToInt(cdTimer).ToString();
            yield return null;
        }

        if (cooldownText != null) cooldownText.text = "";
        isCooldown = false;
        skillButton.interactable = true;
    }
}