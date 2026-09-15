using System;
using UnityEngine;
using UnityEngine.Events;

public class UIButtonManager : MonoBehaviour
{
    public delegate bool EquipmentOperation(EquipmentItemSO k);
    public static EquipmentOperation onSelect;
    [SerializeField] UnityEvent<EquipmentItemSO> onButtonSelected;

    private void Awake() {
        onSelect = OnButtonSelected;
    }

    bool OnButtonSelected(EquipmentItemSO a) {
        onButtonSelected?.Invoke(a);
        return a != null;
    }
}
