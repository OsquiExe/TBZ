using System.Collections;
using UnityEngine;
using TMPro; // Necesario para crear el texto 3D por código

public class CoinPickup : MonoBehaviour
{
    [Header("Configuración de Monedas")]
    [SerializeField] private int minCoins = 10;
    [SerializeField] private int maxCoins = 50;

    private bool isCollected = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Si ya la agarramos este frame, ignoramos
        if (isCollected) return;

        // Verificamos que sea el jugador
        if (collision.CompareTag("Player"))
        {
            isCollected = true;

            // Calculamos la plata
            int coinsToGive = Random.Range(minCoins, maxCoins + 1);

            // Sumamos la plata al sistema
            if (CurrencyManager.Instance != null)
            {
                CurrencyManager.Instance.AddCoins(coinsToGive);
            }
            else
            {
                // Fallback por si probás suelto
                int current = PlayerPrefs.GetInt("DibiCoinsTotal", 0);
                PlayerPrefs.SetInt("DibiCoinsTotal", current + coinsToGive);
                PlayerPrefs.Save();
            }

            // 1. Apagamos el dibujo de la moneda para que parezca que la agarramos
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            if (sr != null) sr.enabled = false;

            // 2. Apagamos el colisionador para no agarrarla 2 veces
            Collider2D col = GetComponent<Collider2D>();
            if (col != null) col.enabled = false;

            // 3. Disparamos el efecto visual (esta corutina se encarga de destruir la moneda al final)
            StartCoroutine(ShowFloatingTextAndDie(coinsToGive));
        }
    }

    private IEnumerator ShowFloatingTextAndDie(int amount)
    {

        GameObject textObj = new GameObject("CoinFeedback");
 
        textObj.transform.position = transform.position + Vector3.up * 0.5f;

        TextMeshPro tmpro = textObj.AddComponent<TextMeshPro>();
        tmpro.text = "+" + amount;
        tmpro.color = Color.yellow; // Color oro
        tmpro.fontSize = 6;
        tmpro.fontStyle = FontStyles.Bold;
        tmpro.alignment = TextAlignmentOptions.Center;
        tmpro.sortingOrder = 50; 

  
        float duration = 1f; 
        float elapsed = 0f;

        Vector3 startPos = textObj.transform.position;
        Vector3 endPos = startPos + Vector3.up * 1.5f; 

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

      
            textObj.transform.position = Vector3.Lerp(startPos, endPos, Mathf.Sin(t * Mathf.PI * 0.5f));

       
            Color c = tmpro.color;
            c.a = Mathf.Lerp(1f, 0f, t);
            tmpro.color = c;

            yield return null;
        }


        Destroy(textObj); 
        Destroy(gameObject); 
    }
}