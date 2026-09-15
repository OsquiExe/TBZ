using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Items Collection", menuName = "TechLab/Items Collection")]
public class ItemsCollection : ScriptableObject
{
    public EquipmentItemSO[] collection;

    public void Populate(EquipmentItemSO[] _c) {
        collection = _c;
    }

    public bool IsEmpty() {
        return collection == null || collection.Length <= 0;
    }

}
