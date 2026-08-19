using System.Collections;
using UnityEngine;
using TMPro;

public class MissileEffect : MonoBehaviour
{
    public int level = 1;
    private float timer = 0f;

    [Header("Settings de la Bomba")]
    [SerializeField] private float explosionRadius = 3f;
    [SerializeField] private float fuseTime = 1f;
    [SerializeField] private float baseCooldown = 10f;
    [SerializeField] private float minCooldown = 4f;

    AudioManager AudioManager;

    private void Awake()
    {
        AudioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

    private void Update()
    {
        timer += Time.deltaTime;

        float currentCooldown = Mathf.Max(minCooldown, baseCooldown - (level * 1.2f));

        if (timer >= currentCooldown)
        {
            SpawnBombs();
            timer = 0f;
        }
    }

    private void SpawnBombs()
    {
        int bombsToSpawn = level;


        float currentDamage = 2f * level;
        float currentCooldown = Mathf.Max(minCooldown, baseCooldown - (level * 1.2f));

        Debug.Log($"💣 [PowerUp] ¡Misiles Nivel {level}! Tirando {bombsToSpawn} misiles. Daño: {currentDamage}. CD: {currentCooldown}s");

        for (int i = 0; i < bombsToSpawn; i++)
        {
            Vector2 randomOffset = Random.insideUnitCircle.normalized * Random.Range(1.5f, 3f);
            Vector2 spawnPosition = (Vector2)transform.position + randomOffset;

            StartCoroutine(BombRoutine(spawnPosition, currentDamage));

            AudioManager.PlaySFX(AudioManager.bombDecoysSFX);
        }
    }

    private IEnumerator BombRoutine(Vector2 position, float damage)
    {
        GameObject bombObj = new GameObject("MissileBomb");
        bombObj.transform.position = position;

        TextMeshPro tmpro = bombObj.AddComponent<TextMeshPro>();
        tmpro.text = "🐹";
        tmpro.alignment = TextAlignmentOptions.Center;
        tmpro.fontSize = 8;
        tmpro.sortingOrder = 40;

        float elapsed = 0f;
        Vector3 originalScale = bombObj.transform.localScale;

        while (elapsed < fuseTime)
        {
            elapsed += Time.deltaTime;

            float scalePingPong = 1f + Mathf.PingPong(elapsed * 4f, 0.3f);
            bombObj.transform.localScale = originalScale * scalePingPong;

            tmpro.color = Color.Lerp(Color.white, Color.red, elapsed / fuseTime);

            yield return null;
        }

        Destroy(bombObj);

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(position, explosionRadius);
        foreach (var hit in hitEnemies)
        {
            Enemy enemy = hit.GetComponent<Enemy>();
            if (enemy != null && !enemy.isDead)
            {
                Vector2 knockbackDir = (enemy.transform.position - (Vector3)position).normalized;
                enemy.TakeDamage(damage, knockbackDir * 10f);
            }
        }

        if (CameraMove.Instance != null)
        {
            CameraMove.Instance.ShakeCamera(0.2f, 0.1f);
        }

        StartCoroutine(CreateExplosionVFX(position));
    }

    private IEnumerator CreateExplosionVFX(Vector2 pos)
    {
        GameObject explosionObj = new GameObject("ExplosionVFX");
        explosionObj.transform.position = pos;

        LineRenderer line = explosionObj.AddComponent<LineRenderer>();
        line.positionCount = 30;
        line.loop = true;
        line.useWorldSpace = false;
        line.startWidth = 0.8f;
        line.endWidth = 0.8f;
        line.material = new Material(Shader.Find("Sprites/Default"));
        line.sortingOrder = 50;

        float duration = 0.3f;
        float elapsed = 0f;
        Color orange = new Color(1f, 0.4f, 0f);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            float easeOut = 1f - Mathf.Pow(1f - t, 3f);
            float currentRadius = Mathf.Lerp(0f, explosionRadius, easeOut);

            for (int i = 0; i < 30; i++)
            {
                float angle = (i / 30f) * Mathf.PI * 2f;
                float x = Mathf.Cos(angle) * currentRadius;
                float y = Mathf.Sin(angle) * currentRadius;
                line.SetPosition(i, new Vector3(x, y, 0));
            }

            float alpha = Mathf.Lerp(1f, 0f, t);
            line.startColor = new Color(orange.r, orange.g, orange.b, alpha);
            line.endColor = new Color(orange.r, orange.g, orange.b, alpha);

            yield return null;
        }

        Destroy(explosionObj);
    }
}