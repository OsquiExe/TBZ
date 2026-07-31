using System.Collections;
using UnityEngine;

public class ShockwaveEffect : MonoBehaviour
{
    public int level = 1;
    private float timer = 0f;

    [Header("Settings de la Onda")]
    [SerializeField] private float baseRadius = 15f;
    [SerializeField] private float baseDamage = 10f;
    [SerializeField] private float baseCooldown = 15f;
    [SerializeField] private float minCooldown = 7f;

    private void Update()
    {
        timer += Time.deltaTime;


        float currentCooldown = Mathf.Max(minCooldown, baseCooldown - (level * 1.5f));

        if (timer >= currentCooldown)
        {
            TriggerShockwave();
            timer = 0f;
        }
    }

    private void TriggerShockwave()
    {
        float currentRadius = baseRadius + (level * 2f);
        float currentDamage = baseDamage + (level * 5f);
        float currentCooldown = Mathf.Max(minCooldown, baseCooldown - (level * 1.5f));

        Debug.Log($" [PowerUp] ¡Onda Expansiva Nivel {level}! Radio: {currentRadius}, Daño: {currentDamage}, CD: {currentCooldown}s");

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

        LineRenderer line = waveObj.AddComponent<LineRenderer>();
        line.positionCount = 50;
        line.loop = true;
        line.useWorldSpace = false;
        line.startWidth = 0.6f;
        line.endWidth = 0.6f;

        line.material = new Material(Shader.Find("Sprites/Default"));
        line.sortingOrder = 50;

        float duration = 1.2f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            float easeOut = Mathf.Sin(t * Mathf.PI * 0.5f);
            float currentVisualRadius = Mathf.Lerp(0f, maxRadius, easeOut);

            for (int i = 0; i < 50; i++)
            {
                float angle = (i / 50f) * Mathf.PI * 2f;
                float x = Mathf.Cos(angle) * currentVisualRadius;
                float y = Mathf.Sin(angle) * currentVisualRadius;
                line.SetPosition(i, new Vector3(x, y, 0));
            }

            float alpha = Mathf.Lerp(1f, 0f, t);
            line.startColor = new Color(0f, 1f, 1f, alpha);
            line.endColor = new Color(0f, 1f, 1f, alpha);

            yield return null;
        }

        Destroy(waveObj);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, baseRadius);
    }
}