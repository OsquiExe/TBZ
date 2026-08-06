using UnityEngine;

public enum EquipmentType
{
    Casco,
    Armadura,
    Botas,
    Alas,
    ProtesisCibernetica,
    ProtesisFisica
}

[CreateAssetMenu(fileName = "NewEquipment", menuName = "TechLab/Equipamiento")]
public class EquipmentItemSO : ScriptableObject
{
    [Header("Datos de la Tienda")] 
    public int price = 100; 

    [Header("Datos Visuales")]
    public string itemName = "Nuevo Ítem";
    public Sprite itemIcon;
    public EquipmentType equipmentType;

    [Header("Estadísticas")]
    public float healthBonus = 0f;
    public float damageBonus = 0f;
    public float speedBonus = 0f;
}