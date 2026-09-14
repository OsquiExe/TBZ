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

public enum EquipmentTier {
    C,
    B,
    A,
    S
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
    public EquipmentTier equipmentTier;

    [Header("Estadísticas")]
    public float healthBonus = 0f;
    public float damageBonus = 0f;
    public float speedBonus = 0f;

    public NivelDatos[] niveles;

    [System.Serializable]
    public class NivelDatos {
        [Header("Datos de la Tienda")]
        public int price = 100;

        [Header("Datos Visuales")]
        public string itemName = "Nuevo Ítem";
        public Sprite itemIcon;
        public EquipmentType equipmentType;
        public EquipmentTier equipmentTier;

        [Header("Estadísticas")]
        public float healthBonus = 0f;
        public float damageBonus = 0f;
        public float speedBonus = 0f;
    }

}
