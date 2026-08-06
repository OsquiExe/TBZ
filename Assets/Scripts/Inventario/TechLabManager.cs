using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TechLabManager : MonoBehaviour
{
    public static TechLabManager Instance;

    [Header("Estadísticas Base del Jugador")]
    public float baseHealth = 100f;
    public float baseDamage = 10f;
    public float baseSpeed = 5f;

    [Header("Textos de Estadísticas (UI)")]
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI damageText;
    public TextMeshProUGUI speedText;

    [Header("Inventario y Equipamiento")]
    public List<EquipmentItemSO> ownedItems = new List<EquipmentItemSO>(); 
    private Dictionary<EquipmentType, EquipmentItemSO> equippedItems = new Dictionary<EquipmentType, EquipmentItemSO>();

    [Header("Referencias de los 6 Slots (UI)")]
    public EquipmentSlotUI slotCasco;
    public EquipmentSlotUI slotArmadura;
    public EquipmentSlotUI slotBotas;
    public EquipmentSlotUI slotAlas;
    public EquipmentSlotUI slotProtesisCibernetica;
    public EquipmentSlotUI slotProtesisFisica;

    [Header("Panel de Inventario (Pop-up)")]
    public GameObject inventoryPanel;
    public Transform inventoryContent; 
    public GameObject inventoryCardPrefab; 

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (inventoryPanel != null) inventoryPanel.SetActive(false);
        UpdateStatsUI();
    }


    public void OpenInventoryForSlot(EquipmentType type)
    {
        inventoryPanel.SetActive(true);

 
        foreach (Transform child in inventoryContent)
        {
            Destroy(child.gameObject);
        }

        bool foundAny = false;

  
        foreach (var item in ownedItems)
        {
            if (item.equipmentType == type) 
            {
                foundAny = true;
                GameObject cardObj = Instantiate(inventoryCardPrefab, inventoryContent);
                InventoryCardUI cardUI = cardObj.GetComponent<InventoryCardUI>();
                cardUI.Setup(item);
            }
        }

        if (!foundAny)
        {
            Debug.Log($"No tenés ítems del tipo {type} en el inventario.");
        }
    }

 
    public void EquipItem(EquipmentItemSO item)
    {
  
        equippedItems[item.equipmentType] = item;

    
        switch (item.equipmentType)
        {
            case EquipmentType.Casco: slotCasco.EquipItem(item); break;
            case EquipmentType.Armadura: slotArmadura.EquipItem(item); break;
            case EquipmentType.Botas: slotBotas.EquipItem(item); break;
            case EquipmentType.Alas: slotAlas.EquipItem(item); break;
            case EquipmentType.ProtesisCibernetica: slotProtesisCibernetica.EquipItem(item); break;
            case EquipmentType.ProtesisFisica: slotProtesisFisica.EquipItem(item); break;
        }

        inventoryPanel.SetActive(false); 
        UpdateStatsUI();
    }

  
    private void UpdateStatsUI()
    {
        float totalHealth = baseHealth;
        float totalDamage = baseDamage;
        float totalSpeed = baseSpeed;

        foreach (var item in equippedItems.Values)
        {
            totalHealth += item.healthBonus;
            totalDamage += item.damageBonus;
            totalSpeed += item.speedBonus;
        }


        if (healthText != null) healthText.text = $"VIDA: {totalHealth}";
        if (damageText != null) damageText.text = $"DAÑO: {totalDamage}";
        if (speedText != null) speedText.text = $"VEL: {totalSpeed}";
    }


    public void CloseInventory()
    {
        if (inventoryPanel != null) inventoryPanel.SetActive(false);
    }
}