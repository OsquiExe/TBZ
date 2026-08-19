using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New PowerUp", menuName = "PowerUp")]
public class PowerUpSO : ScriptableObject
{
    [Header("Visuals (Sprites)")]

    public Sprite powerUpImageSquare;
    public Sprite powerUpIconRound; 
    public GameObject prefab;

    public string powerUpText;
    public PowerUpEffect effectType;
    public float effectValue;
    public bool isUnique;
    public int unlockLevel;
}
public enum PowerUpEffect
{
    Boomerang,
    Laser,
    FlameThrower,
    Missiles,
    Decoys,
    Snakes,
    Spikes,
    ShockWave,
    MoreDamage,
    ProgressiveHeal
}
