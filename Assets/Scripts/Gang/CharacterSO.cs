using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacter", menuName = "TheGang/Personaje")]
public class CharacterSO : ScriptableObject
{
    [Header("Datos de la Tienda")] 
    public int price = 1000;

    public string characterName;
    public Sprite characterIcon;
    public GameObject characterPrefab;

    [Header("Stats Base del Personaje")]
    public float baseHealth = 100f;
    public float baseDamage = 10f;
    public float baseSpeed = 5f;
}