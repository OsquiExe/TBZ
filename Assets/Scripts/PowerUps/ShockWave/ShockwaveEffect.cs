using System.Collections;
using UnityEngine;

public class ShockwaveEffect : MonoBehaviour
{
    public int level = 1;
    private float timer = 0f;

    [Header("Settings de la Onda")]
    [SerializeField] private float baseRadius = 15f;
    [SerializeField] private float baseCooldown = 15f;
    [SerializeField] private float minCooldown = 7f;

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
        float currentCooldown = Mathf.Max(minCooldown, baseCooldown - (level * 1.5f));

        if (timer >= currentCooldown)
        {
            TriggerShockwave();
            timer = 0f;
            if (AudioManager != null) AudioManager.PlaySFX(AudioManager.shockwaveSFX);
        }
    }

    private void TriggerShockwave()
    {
        float currentRadius = baseRadius + (level * 2f);
        float currentDamage = 2f * level;

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, currentRadius);
        foreach (var hit in hitEnemies)
        {
            Enemy enemy = hit.GetComponent<Enemy>();
            if (enemy != null && !enemy.isDead)
            {
                Vector2 knockbackDir = (enemy.transform.position - transform.position).normalized;
                enemy.TakeDamage(currentDamage, knockbackDir * 8f);
            }
        }

        if (CameraMove.Instance != null)
        {
            CameraMove.Instance.ShakeCamera(0.3f, 0.15f);
        }

        StartCoroutine(CreateVisualShockwave(currentRadius));
    }

    private IEnumerator CreateVisualShockwave(float maxRadius)
    {
        GameObject waveObj = new GameObject("ShockwaveVFX");
        waveObj.transform.position = transform.position;

        SpriteRenderer sr = waveObj.AddComponent<SpriteRenderer>();
        sr.sortingOrder = 50;
        waveObj.transform.localScale = new Vector3(maxRadius * 1.5f, maxRadius * 1.5f, 1f);

        Sprite[] spritesActivos = (level >= 4 && spritesMaximo != null && spritesMaximo.Length > 0) ? spritesMaximo : spritesNormal;

        if (spritesActivos != null && spritesActivos.Length > 0)
        {
            sr.sprite = spritesActivos[0];
            float timePerFrame = 0.5f / spritesActivos.Length;

            for (int i = 0; i < spritesActivos.Length; i++)
            {
                sr.sprite = spritesActivos[i];
                yield return new WaitForSeconds(timePerFrame);
            }
        }
        else
        {
            yield return new WaitForSeconds(0.5f);
        }

        Destroy(waveObj);
    }
}