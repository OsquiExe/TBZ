using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PowerUp : MonoBehaviour
{
    [SerializeField] private SpriteRenderer powerUpImageRenderer;
    [SerializeField] private TextMeshPro powerUpTextRenderer;
    private PowerUpSO powerUpInfo;

    public void Setup(PowerUpSO powerUp)
    {
        powerUpInfo = powerUp;

        if (powerUpImageRenderer != null)
        {
            powerUpImageRenderer.sprite = powerUp.powerUpImage;

        
            powerUpImageRenderer.sortingLayerName = "Default";
            powerUpImageRenderer.sortingOrder = 50; 
        }

        if (powerUpTextRenderer != null)
        {
            powerUpTextRenderer.text = powerUp.powerUpText;
            powerUpTextRenderer.sortingOrder = 51; 
        }
    }

   
    public void OnCardClicked()
    {
        if (powerUpInfo == null) return;

        if (PowerUpManager.Instance != null)
        {
            PowerUpManager.Instance.SelectPowerUp(powerUpInfo);
        }
    }


    private void OnMouseDown()
    {
        OnCardClicked();
    }
}