using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class InventoryVisualizer : MonoBehaviour
{
    [SerializeField] ItemsCollection inventory;
    [SerializeField] GameObject prefab;
    [SerializeField] Transform content;
    public InventoryFilter filter = new();

    private void Awake() {

    }

    private void OnEnable() {
        PopulateInventory();
    }

    void PopulateInventory() {
        if(content.childCount > 0) {
            Transform[] childs = content.GetComponentsInChildren<Transform>();
            for(int i = 0; i < childs.Length; i++) {
                if(childs[i] == content) continue;

                Destroy(childs[i].gameObject);
            }
        }


        int startIndex = filter.NoneOption() ? -1 : 0;

        for(int i = startIndex; i < inventory.collection.Length; i++) {

            if(i >= 0) {
                bool filtersCheck = filter.HasFilters() && !filter.TierIsFiltered(inventory.collection[i].equipmentTier);
                filtersCheck = filtersCheck || (filter.ExcludedItemsFilters() && filter.ItemIsFiltered(inventory.collection[i]));

                if(filtersCheck) continue;
            }

            GameObject a = Instantiate(prefab, content);

            var card = a.GetComponent<InventoryCardUI>();
            card.Setup(i >= 0 ? inventory.collection[i] : null);

            int index = i;

            a.GetComponent<Button>().onClick.AddListener(() => A(index));
        }
    }

    void A(int i) {
        bool? a = UIButtonManager.onSelect?.Invoke(i >= 0 ? inventory.collection[i] : null);

        if(a == null || !a.Value || i < 0) return;

        PopulateInventory();
    }

    [System.Serializable]
    public class InventoryFilter {

        bool insertNoneOption;
        List<EquipmentTier> TierFilters = new();
        List<EquipmentItemSO> ItemsExluded = new();

        public bool HasFilters() {
            return TierFilters.Count > 0;
        }

        public bool ExcludedItemsFilters() {
            return ItemsExluded.Count > 0;
        }

        public bool NoneOption() {
            return insertNoneOption;
        }

        public bool TierIsFiltered(EquipmentTier tier) {
            return TierFilters.Count > 0 && TierFilters.Contains(tier);
        }

        public bool ItemIsFiltered(EquipmentItemSO item) {
            return ItemsExluded.Count > 0 && ItemsExluded.Contains(item);
        }

        public InventoryFilter AddFilter(EquipmentTier Tier) {
            if(TierFilters.Contains(Tier)) return this;

            TierFilters.Add(Tier);
            return this;
        }

        public InventoryFilter RemoveFilter(EquipmentTier Tier) {
            if(!TierFilters.Contains(Tier)) return this;

            TierFilters.Remove(Tier);
            return this;
        }

        public InventoryFilter ClearFilter() {

            TierFilters.Clear();
            return this;
        }

        public InventoryFilter AddExcludedItem(EquipmentItemSO Item) {
            if(ItemsExluded.Contains(Item)) return this;

            ItemsExluded.Add(Item);
            return this;
        }
        public InventoryFilter RemoveExludedItem(EquipmentItemSO Item) {
            if(!ItemsExluded.Contains(Item)) return this;

            ItemsExluded.Remove(Item);
            return this;
        }
        public InventoryFilter ClearExcludedItem() {
            ItemsExluded.Clear();
            return this;
        }

        public InventoryFilter NoneOption(bool t) {
            insertNoneOption = t;
            return this;
        }
    }
}
