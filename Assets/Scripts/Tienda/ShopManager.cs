using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance;

    [Header("Catálogo de Ítems")]
    [SerializeField] private List<EquipmentItemSO> equipmentCatalog;
    [SerializeField] private GameObject shopItemPrefab;

    [Header("Catálogo de Personajes")]
    [SerializeField] private List<CharacterSO> characterCatalog;
    [SerializeField] private GameObject characterShopItemPrefab;

    [Header("Referencias UI")]
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private Transform shopContent;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (shopPanel != null) shopPanel.SetActive(false);
    }

    public void OpenShop()
    {
        if (shopPanel != null) shopPanel.SetActive(true);
        PopulateShop();
    }

    private void PopulateShop()
    {

        foreach (Transform child in shopContent)
        {
            Destroy(child.gameObject);
        }

        foreach (var character in characterCatalog)
        {

            if (TheGangManager.Instance != null && TheGangManager.Instance.ownedCharacters.Contains(character))
            {
                continue;
            }

            GameObject cardObj = Instantiate(characterShopItemPrefab, shopContent);
            CharacterShopItemUI cardUI = cardObj.GetComponent<CharacterShopItemUI>();
            cardUI.Setup(character);
        }


        foreach (var item in equipmentCatalog)
        {
            if (TechLabManager.Instance != null && TechLabManager.Instance.ownedItems.Contains(item))
            {
                continue; 
            }

            GameObject itemObj = Instantiate(shopItemPrefab, shopContent);
            ShopItemUI itemUI = itemObj.GetComponent<ShopItemUI>();
            itemUI.Setup(item);
        }
    }



    public void TryBuyCharacter(CharacterSO characterToBuy, CharacterShopItemUI cardUI)
    {
        if (CurrencyManager.Instance == null || TheGangManager.Instance == null) return;

        if (CurrencyManager.Instance.TotalCoins >= characterToBuy.price)
        {
            CurrencyManager.Instance.SpendCoins(characterToBuy.price);


            TheGangManager.Instance.ownedCharacters.Add(characterToBuy);


            cardUI.MarkAsSold();

            Debug.Log($"¡Personaje {characterToBuy.characterName} comprado!");
        }
    }

    public void TryBuyItem(EquipmentItemSO itemToBuy, ShopItemUI cardUI)
    {
        if (CurrencyManager.Instance == null || TechLabManager.Instance == null) return;

        if (CurrencyManager.Instance.TotalCoins >= itemToBuy.price)
        {
            CurrencyManager.Instance.SpendCoins(itemToBuy.price);
            TechLabManager.Instance.ownedItems.Add(itemToBuy);

   
            cardUI.MarkAsSold();
        }
    }

    public void CloseShop()
    {
        if (shopPanel != null) shopPanel.SetActive(false);
    }
}