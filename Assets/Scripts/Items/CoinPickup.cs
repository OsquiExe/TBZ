using System.Collections;
using UnityEngine;
using TMPro; 

public class CoinPickup : MonoBehaviour
{
    [Header("Configuración de Monedas")]
    [SerializeField] private int minCoins = 10;
    [SerializeField] private int maxCoins = 50;

    private bool isCollected = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
  
        if (isCollected) return;


        if (collision.CompareTag("Player"))
        {
            isCollected = true;

          
            int coinsToGive = Random.Range(minCoins, maxCoins + 1);

        
            if (CurrencyManager.Instance != null)
            {
                CurrencyManager.Instance.AddCoins(coinsToGive);
            }
            else
            {
          
                int current = PlayerPrefs.GetInt("DibiCoinsTotal", 0);
                PlayerPrefs.SetInt("DibiCoinsTotal", current + coinsToGive);
                PlayerPrefs.Save();
            }


            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            if (sr != null) sr.enabled = false;

      
            Collider2D col = GetComponent<Collider2D>();
            if (col != null) col.enabled = false;


            StartCoroutine(ShowFloatingTextAndDie(coinsToGive));
        }
    }

    private IEnumerator ShowFloatingTextAndDie(int amount)
    {

        GameObject textObj = new GameObject("CoinFeedback");
 
        textObj.transform.position = transform.position + Vector3.up * 0.5f;

        TextMeshPro tmpro = textObj.AddComponent<TextMeshPro>();
        tmpro.text = "+" + amount;
        tmpro.color = Color.yellow;
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