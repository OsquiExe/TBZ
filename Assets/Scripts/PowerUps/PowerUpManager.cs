using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowerUpManager : MonoBehaviour
{
    [SerializeField] public GameObject uiPowerUpSelect;
    [SerializeField] public GameObject powerUpPrefab;
    [SerializeField] Transform powerUpPosition1;
    [SerializeField] Transform powerUpPosition2;
    [SerializeField] Transform powerUpPosition3;
    [SerializeField] List<PowerUpSO> deck;

    GameObject powerUp1, powerUp2, powerUp3;

    List<PowerUpSO> alreadySelectedPowerUps = new List<PowerUpSO>();

    void Start()
    {
        RandomizeNewPowerUps();
    }

    void RandomizeNewPowerUps()
    {
        if(powerUp1 != null) Destroy (powerUp1);
        if(powerUp2 != null) Destroy (powerUp2);
        if(powerUp3 != null) Destroy (powerUp3);

        List<PowerUpSO> randomizePowerUps = new List<PowerUpSO>();

        List<PowerUpSO> availablePowerUp = new List<PowerUpSO>(deck);
        availablePowerUp.RemoveAll(powerUp => 
            powerUp.isUnique && alreadySelectedPowerUps.Contains(powerUp)
        //|| powerUp.unlockLevel > GameManager.Instance.currentLevel()
        );

        if(availablePowerUp.Count < 3)
        {
            Debug.Log("Not enough available power ups");
            return;
        }
        while(randomizePowerUps.Count < 3)
        {
            PowerUpSO randomPowerUp = availablePowerUp[Random.Range(0, availablePowerUp.Count)];
            if(!randomizePowerUps.Contains(randomPowerUp))
            {
                randomizePowerUps.Add(randomPowerUp);
            }
        }
        powerUp1 = InstantiatePowerUp(randomizePowerUps[0], powerUpPosition1);
        powerUp2 = InstantiatePowerUp(randomizePowerUps[1], powerUpPosition2);
        powerUp3 = InstantiatePowerUp(randomizePowerUps[2], powerUpPosition3);
    }
    GameObject InstantiatePowerUp(PowerUpSO powerUpSO, Transform position)
    {
        GameObject powerUpGo = Instantiate(powerUpPrefab, position.position, Quaternion.identity, position);
        PowerUp powerUp = powerUpGo.GetComponent<PowerUp>();
        powerUp.Setup(powerUpSO);
        return powerUpGo;
    }
}
