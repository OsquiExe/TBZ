using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New PowerUp", menuName = "PowerUp")]
public class PowerUpSO : ScriptableObject
{
    public Sprite powerUpImage;
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
