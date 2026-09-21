using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryCardUI : MonoBehaviour
{
    [Header("Referencias Visuales")]
    [SerializeField] Image background;
    public TextMeshProUGUI nameText;
    public Image iconImage;
    public TextMeshProUGUI statsText;
    [SerializeField] Button bttn;

    private EquipmentItemSO myItem;

    public void Setup(EquipmentItemSO item)
    {
        myItem = item;

        if(item == null) {
            nameText.text = "(NONE)";

            return;
        }

        if (nameText != null) nameText.text = item.itemName;
        if (iconImage != null)
        {
            iconImage.sprite = item.itemIcon;
            iconImage.color = Color.white;
        }

        string stats = "";
        if (item.healthBonus > 0) stats += $"+{item.healthBonus} Vida\n";
        if (item.damageBonus > 0) stats += $"+{item.damageBonus} Daño\n";
        if (item.speedBonus > 0) stats += $"+{item.speedBonus} Vel\n";

        if (statsText != null) statsText.text = stats;
    }

    public void SetupAsInvisible()
    {
        background.enabled = false;
        iconImage.enabled = false;
        nameText.enabled = false;
        statsText.enabled = false;
        bttn.enabled = false;
    }

    public void OnEquipButtonClicked()
    {
        TechLabManager.Instance.EquipItem(myItem);
    }

    public EquipmentItemSO getItem() { return myItem; }
}