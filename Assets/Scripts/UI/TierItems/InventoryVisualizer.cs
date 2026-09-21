using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class InventoryVisualizer : MonoBehaviour
{
    [SerializeField] ItemsCollection inventory;

    [Space(10)]
    [SerializeField] int columns = 3;
    [SerializeField] GameObject rowPrefab;
    [SerializeField] GameObject SlotPrefab;
    [SerializeField] ScrollRect scroll;
    [SerializeField] Transform content;
    public InventoryFilter filter = new();

    private void Awake() {
        columns = Math.Clamp(columns, 0, columns + 1);
    }

    private void OnEnable() {
        PopulateInventory();
    }

    void PopulateInventory() {
        scroll.verticalNormalizedPosition = 1;

        if(content.childCount > 0) {
            Transform[] childs = content.GetComponentsInChildren<Transform>();
            for(int i = 0; i < childs.Length; i++) {
                if(childs[i] == content) continue;

                Destroy(childs[i].gameObject);
            }
        }

        //Columns

        int rows = (inventory.collection.Length / columns) + 1;
        int startIndex = filter.NoneOption() ? -1 : 0;

        for (int r = 0; r < rows; r++)
        {
            GameObject row = Instantiate(rowPrefab, content);

            for (int i = 0; i < columns; i++)
            {
                if (startIndex >= inventory.collection.Length)
                {
                    GameObject b = Instantiate(SlotPrefab, row.transform);
                    b.GetComponent<InventoryCardUI>().SetupAsInvisible();

                    continue;
                }

                if (startIndex >= 0)
                {
                    bool filtersCheck = filter.HasFilters() && !filter.TierIsFiltered(inventory.collection[startIndex].equipmentTier);
                    filtersCheck = filtersCheck || (filter.ExcludedItemsFilters() && filter.ItemIsFiltered(inventory.collection[startIndex]));

                    Debug.Log($"{startIndex} - {inventory.collection[startIndex].name} - {filtersCheck}");

                    if (filtersCheck)
                    {
                        startIndex++;
                        i--;
                        continue;
                    }
                }

                GameObject a = Instantiate(SlotPrefab, row.transform);

                var card = a.GetComponent<InventoryCardUI>();
                card.Setup(startIndex >= 0 ? inventory.collection[startIndex] : null);

                int index = startIndex; //Sin esto se pasa la referencia del int en lugar de su valor

                a.GetComponent<Button>().onClick.AddListener(() => A(index));

                startIndex++;
            }
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
