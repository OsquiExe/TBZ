using UnityEngine;

public enum EquipmentType
{
    Casco,
    Armadura,
    Botas,
    Alas,
    ProtesisCibernetica,
    ProtesisFisica,
    Asistente,
    Asistente2
}

public enum AssistantBuffType
{
    Ninguno,
    CampoDeFuerza,
    Metralleta
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

    [Header("Estadísticas Base")]
    public float healthBonus = 0f;
    public float damageBonus = 0f;
    public float speedBonus = 0f;

    [Header("Nuevas Estadísticas")]
    public float defenseBonus = 0f;
    public float resistanceBonus = 0f;
    public float cooldownReductionBonus = 0f;

    [Header("Datos de Asistente (Habilidad Activa)")]
    public AssistantBuffType buffType;
    public float buffDuration = 10f;
    public float buffCooldown = 30f;
    public GameObject buffVFXPrefab;

    [Tooltip("El Prefab de la bala (Solo necesario si el tipo es Metralleta)")]
    public GameObject projectilePrefab; // <--- PREFAB DE LA BALA
}