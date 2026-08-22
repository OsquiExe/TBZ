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

    [Header("Animaciones de Efectos (Opcional)")]
    [Tooltip("Ej: Los sprites de OndaMax o la Explosión de los Señuelos")]
    public Sprite[] spritesPrincipales;

    [Tooltip("Ej: Los sprites del Señuelo esperando (Bichito verde)")]
    public Sprite[] spritesSecundarios;

    [Header("Estadísticas")]
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