using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterShopItemUI : MonoBehaviour
{
    [Header("Referencias Visuales")]
    public TextMeshProUGUI nameText;
    public Image iconImage;
    public TextMeshProUGUI statsText;
    public TextMeshProUGUI priceText;
    public Button buyButton;

    private CharacterSO myCharacter;

    public void Setup(CharacterSO character)
    {
        myCharacter = character;

        if (nameText != null) nameText.text = character.characterName;
        if (iconImage != null) iconImage.sprite = character.characterIcon;
        if (priceText != null) priceText.text = $"{character.price}";


        if (statsText != null)
        {
            statsText.text = $"Vida Base: {character.baseHealth}\nDaño Base: {character.baseDamage}\nVel Base: {character.baseSpeed}";
        }

        buyButton.onClick.AddListener(OnBuyClicked);
    }

    private void OnBuyClicked()
    {
        if (ShopManager.Instance != null)
        {
            ShopManager.Instance.TryBuyCharacter(myCharacter, this);
        }
    }

    public void MarkAsSold()
    {

        Destroy(gameObject);
    }
}