using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class BombPickup : MonoBehaviour
{
    [Header("Configuración de la Bomba")]
    [SerializeField] private float explosionRadius = 20f; 
    [SerializeField] private float massiveDamage = 99999f; 

    private bool isCollected = false;
    [SerializeField] UnityEvent onExplode;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isCollected) return;

        if (collision.CompareTag("Player"))
        {
            isCollected = true;

            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            if (sr != null) sr.enabled = false;

            Collider2D col = GetComponent<Collider2D>();
            if (col != null) col.enabled = false;

            Detonate();
        }
    }

    private void Detonate()
    {
        onExplode?.Invoke();

        if (CameraMove.Instance != null)
        {
            CameraMove.Instance.ShakeCamera(0.6f, 0.4f);
        }


        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        int killCount = 0;

        foreach (var hit in hitEnemies)
        {
            Enemy enemy = hit.GetComponent<Enemy>();
            if (enemy != null && !enemy.isDead)
            {
            
                Vector2 knockbackDir = (enemy.transform.position - transform.position).normalized;

                
                enemy.TakeDamage(massiveDamage, knockbackDir * 15f);
                killCount++;
            }
        }

        Debug.Log($"☢️ [Bomba] ¡Bomba detonada! {killCount} enemigos eliminados de un golpe.");


        StartCoroutine(VisualExplosionAndDie());
    }


    private IEnumerator VisualExplosionAndDie()
    {
        GameObject explosionObj = new GameObject("NukeVFX");
        explosionObj.transform.position = transform.position;

        LineRenderer line = explosionObj.AddComponent<LineRenderer>();
        line.positionCount = 50;
        line.loop = true;
        line.useWorldSpace = false;
        line.startWidth = 2f;
        line.endWidth = 2f;
        line.material = new Material(Shader.Find("Sprites/Default"));
        line.sortingOrder = 100; 

        float duration = 0.5f; 
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

 
            float currentRadius = Mathf.Lerp(0f, explosionRadius, 1f - Mathf.Pow(1f - t, 4f));

            for (int i = 0; i < 50; i++)
            {
                float angle = (i / 50f) * Mathf.PI * 2f;
                float x = Mathf.Cos(angle) * currentRadius;
                float y = Mathf.Sin(angle) * currentRadius;
                line.SetPosition(i, new Vector3(x, y, 0));
            }

            float alpha = Mathf.Lerp(1f, 0f, t);
            line.startColor = new Color(1f, 1f, 1f, alpha);
            line.endColor = new Color(1f, 1f, 1f, alpha);

            yield return null;
        }

        Destroy(explosionObj);
        Destroy(gameObject); 
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}