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
            if (powerUp.powerUpImageSquare != null)
            {
                powerUpImageRenderer.sprite = powerUp.powerUpImageSquare;

    
                powerUpImageRenderer.sortingLayerName = "MenuSuperior";
                powerUpImageRenderer.sortingOrder = 50;
            }
            else
            {
                Debug.LogWarning($" El PowerUp '{powerUp.name}' NO tiene asignada la imagen CUADRADA en el Inspector.");
            }
        }

        if (powerUpTextRenderer != null)
        {
            powerUpTextRenderer.text = powerUp.powerUpText;

       
            powerUpTextRenderer.sortingLayerID = SortingLayer.NameToID("MenuSuperior");
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