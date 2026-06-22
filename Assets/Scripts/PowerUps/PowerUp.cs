using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PowerUp : MonoBehaviour
{
    [SerializeField] SpriteRenderer powerUpImageRenderer;
    [SerializeField] TextMeshPro powerUpTextRenderer; 
    private PowerUpSO powerUpInfo;

    public void Setup(PowerUpSO powerUp)
    {
        powerUpInfo = powerUp;
        powerUpImageRenderer.sprite = powerUp.powerUpImage;
        powerUpTextRenderer.text = powerUp.powerUpText;
    }

    void OnMouseDown()
    {
        Debug.Log("you clicked a PowerUp");
        PowerUpManager.Instance.SelectPowerUp(powerUpInfo);
    }
}
