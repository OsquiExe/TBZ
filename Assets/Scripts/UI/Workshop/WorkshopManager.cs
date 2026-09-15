using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorkshopManager : MonoBehaviour
{
    [SerializeField] Image[] SlotsUI = new Image[3];
    EquipmentItemSO[] selectedItems = new EquipmentItemSO[3];

    [SerializeField] GameObject InventoryUi;
    [SerializeField] InventoryVisualizer inventoryVisualizer;

    int waitingForSelectIndex = -1;

    // Start is called before the first frame update
    void Start()
    {
        InventoryUi.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ResultSelected() {
        InventoryUi.SetActive(false);

        for (int i = 0; i < selectedItems.Length; i++) {
            if(selectedItems[i] == null) {
                return;
            }
        }

        InventoryVisualizer.InventoryFilter f = new();
        f.AddFilter((EquipmentTier)(((int)selectedItems[0].equipmentTier) + 1)).NoneOption(false);

        inventoryVisualizer.filter = f;
        InventoryUi.SetActive(true);
    }

    public void SetSelectedUI(int i) { 
        waitingForSelectIndex = i;

        InventoryUi.SetActive(false);

        InventoryVisualizer.InventoryFilter f = new();

        f.NoneOption(true);
        for(int j = 0; j < selectedItems.Length; j++) {
            if(selectedItems[j] == null) continue;

            f.AddExcludedItem(selectedItems[j]).AddFilter(selectedItems[j].equipmentTier);
        }

        inventoryVisualizer.filter = f;
        InventoryUi.SetActive(true);
    }

    public void OnItemSelected(EquipmentItemSO item) {
        if(waitingForSelectIndex < 0) return;

        if(item == null && selectedItems[waitingForSelectIndex] != null) { //Remove Filter
            inventoryVisualizer.filter.RemoveFilter(selectedItems[waitingForSelectIndex].equipmentTier);
            inventoryVisualizer.filter.RemoveExludedItem(selectedItems[waitingForSelectIndex]);

        } else if(item != null) { //Add Filter
            inventoryVisualizer.filter.AddFilter(item.equipmentTier);
            inventoryVisualizer.filter.AddExcludedItem(item);
        }

        selectedItems[waitingForSelectIndex] = item;
        SlotsUI[waitingForSelectIndex].sprite = item == null ? null : item.itemIcon;

        waitingForSelectIndex = -1;
        InventoryUi.SetActive(false);
    }
}
