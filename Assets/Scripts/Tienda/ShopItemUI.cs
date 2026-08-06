using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopItemUI : MonoBehaviour
{
    [Header("Referencias Visuales")]
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI statsText;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private Button buyButton;

    private EquipmentItemSO myItem;

    public void Setup(EquipmentItemSO item)
    {
        myItem = item;

        if (nameText != null) nameText.text = item.itemName;
        if (iconImage != null) iconImage.sprite = item.itemIcon;
        if (priceText != null) priceText.text = $"{item.price}";

 
        string stats = "";
        if (item.healthBonus > 0) stats += $"+{item.healthBonus} Vida\n";
        if (item.damageBonus > 0) stats += $"+{item.damageBonus} Daño\n";
        if (item.speedBonus > 0) stats += $"+{item.speedBonus} Vel\n";
        if (statsText != null) statsText.text = stats;

        buyButton.onClick.AddListener(OnBuyClicked);
        CheckIfAlreadyOwned();
    }

    private void CheckIfAlreadyOwned()
    {
        if (TechLabManager.Instance != null && TechLabManager.Instance.ownedItems.Contains(myItem))
        {
            buyButton.interactable = false;
            priceText.text = "ADQUIRIDO";
        }
    }

    private void OnBuyClicked()
    {
        if (ShopManager.Instance != null)
        {
            ShopManager.Instance.TryBuyItem(myItem, this);
        }
    }

    public void MarkAsSold()
    {
        buyButton.interactable = false;
        priceText.text = "COMPRADO";
    }
}