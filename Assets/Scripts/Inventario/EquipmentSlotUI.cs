using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Button))] 
public class EquipmentSlotUI : MonoBehaviour
{
    [Header("Configuración del Slot")]
    public EquipmentType slotType;

    [Header("Referencias Visuales")]
    [SerializeField] private Image itemIconImage;
    [SerializeField] private TextMeshProUGUI slotNameText;

    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnSlotClicked);
    }

    private void Start()
    {
        if (itemIconImage != null && itemIconImage.sprite == null)
        {
            ClearSlot();
        }
    }

    public void EquipItem(EquipmentItemSO item)
    {
        if (itemIconImage != null)
        {
            itemIconImage.sprite = item.itemIcon;
            itemIconImage.color = Color.white;
            itemIconImage.gameObject.SetActive(true);
        }
        if (slotNameText != null) slotNameText.text = item.itemName;
    }

    public void ClearSlot()
    {
        if (itemIconImage != null)
        {
            itemIconImage.sprite = null;
            itemIconImage.color = new Color(0, 0, 0, 0);
        }
        if (slotNameText != null) slotNameText.text = slotType.ToString();
    }

    private void OnSlotClicked()
    {

        if (TechLabManager.Instance != null)
        {
            TechLabManager.Instance.OpenInventoryForSlot(slotType);
        }
    }
}