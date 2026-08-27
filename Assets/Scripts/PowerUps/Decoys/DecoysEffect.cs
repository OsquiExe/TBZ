using System.Collections;
using UnityEngine;

public class DecoysEffect : MonoBehaviour
{
    public int level = 1;
    private float timer = 0f;

    [Header("Settings de la Bomba")]
    [SerializeField] private float explosionRadius = 3f;
    [SerializeField] private float fuseTime = 1f;
    [SerializeField] private float baseCooldown = 10f;
    [SerializeField] private float minCooldown = 4f;

    [HideInInspector] public Sprite[] spritesNormal;
    [HideInInspector] public Sprite[] spritesMaximo;

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

        for (int i = 0; i < bombsToSpawn; i++)
        {
            Vector2 randomOffset = Random.insideUnitCircle.normalized * Random.Range(1.5f, 3f);
            Vector2 spawnPosition = (Vector2)transform.position + randomOffset;
            StartCoroutine(BombRoutine(spawnPosition, currentDamage));

            if (AudioManager != null) AudioManager.PlaySFX(AudioManager.bombDecoysSFX);
        }
    }

    private IEnumerator BombRoutine(Vector2 position, float damage)
    {
        GameObject bombObj = new GameObject("DecoyBomb");
        bombObj.transform.position = position;

        SpriteRenderer sr = bombObj.AddComponent<SpriteRenderer>();
        sr.sortingOrder = 40;

        Sprite[] spritesActivos = (level >= 4 && spritesMaximo != null && spritesMaximo.Length > 0) ? spritesMaximo : spritesNormal;

        if (spritesActivos == null || spritesActivos.Length == 0)
        {
            Debug.LogWarning("Decoys: No se recibieron sprites.");
            yield break;
        }

        sr.sprite = spritesActivos[0];

        float elapsed = 0f;
        float frameRate = 0.1f;
        float nextFrameTime = frameRate;
        int frameIndex = 0;


        int idleFrames = Mathf.Min(4, spritesActivos.Length);
        Vector3 originalScale = new Vector3(0.7f, 0.7f, 1f);

        while (elapsed < fuseTime)
        {
            elapsed += Time.deltaTime;
            float scalePingPong = 1f + Mathf.PingPong(elapsed * 4f, 0.3f);
            bombObj.transform.localScale = originalScale * scalePingPong;

            if (elapsed >= nextFrameTime && idleFrames > 0)
            {
                frameIndex = (frameIndex + 1) % idleFrames;
                sr.sprite = spritesActivos[frameIndex];
                nextFrameTime += frameRate;
            }
            yield return null;
        }


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

        if (CameraMove.Instance != null) CameraMove.Instance.ShakeCamera(0.2f, 0.1f);


        bombObj.transform.localScale = new Vector3(explosionRadius * 0.8f, explosionRadius * 0.8f, 1f);

        if (spritesActivos.Length > idleFrames)
        {
            int explosionFramesCount = spritesActivos.Length - idleFrames;
            float expFrameRate = 0.3f / explosionFramesCount;

            for (int i = idleFrames; i < spritesActivos.Length; i++)
            {
                sr.sprite = spritesActivos[i];
                yield return new WaitForSeconds(expFrameRate);
            }
        }
        else
        {
            yield return new WaitForSeconds(0.3f);
        }

        Destroy(bombObj);
    }
}