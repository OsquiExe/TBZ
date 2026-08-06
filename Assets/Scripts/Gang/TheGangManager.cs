using System.Collections.Generic;
using UnityEngine;

public class TheGangManager : MonoBehaviour
{
    public static TheGangManager Instance;

    [Header("Personajes Desbloqueados")]
    public List<CharacterSO> ownedCharacters = new List<CharacterSO>();

    [Header("Referencias UI")]
    public Transform charactersContent;
    public GameObject characterCardPrefab;

    [Header("Navegación")]
    public GameObject theGangPanel;
    public GameObject techLabPanel;


    [HideInInspector] public CharacterSO equippedCharacter; 
    private List<CharacterCardUI> activeCards = new List<CharacterCardUI>();

    private void Awake()
    {
        Instance = this;
        LoadEquippedCharacter();
    }

    private void OnEnable()
    {
        PopulateGang();
    }


    private void LoadEquippedCharacter()
    {
        string savedName = PlayerPrefs.GetString("EquippedCharacter", "");

        if (ownedCharacters.Count > 0)
        {
            equippedCharacter = ownedCharacters.Find(c => c.characterName == savedName);

       
            if (equippedCharacter == null)
            {
                equippedCharacter = ownedCharacters[0];


                PlayerPrefs.SetString("EquippedCharacter", equippedCharacter.characterName);
                PlayerPrefs.Save();

                Debug.Log("Se corrigió la memoria. Personaje forzado a: " + equippedCharacter.characterName);
            }
        }
    }

    private void PopulateGang()
    {
        foreach (Transform child in charactersContent)
        {
            Destroy(child.gameObject);
        }
        activeCards.Clear();

        foreach (var character in ownedCharacters)
        {
            GameObject cardObj = Instantiate(characterCardPrefab, charactersContent);
            CharacterCardUI cardUI = cardObj.GetComponent<CharacterCardUI>();
            cardUI.Setup(character);
            activeCards.Add(cardUI);
        }


        RefreshAllCards();
    }


    public void EquipCharacter(CharacterSO newCharacter)
    {
        equippedCharacter = newCharacter;


        PlayerPrefs.SetString("EquippedCharacter", newCharacter.characterName);
        PlayerPrefs.Save();

        Debug.Log("Has equipado a: " + newCharacter.characterName);


        RefreshAllCards();
    }

    private void RefreshAllCards()
    {
        foreach (var card in activeCards)
        {
            card.UpdateUIState(equippedCharacter);
        }
    }

    public void SelectCharacterForBuild(CharacterSO selectedCharacter)
    {
        if (TechLabManager.Instance != null)
        {
            TechLabManager.Instance.baseHealth = selectedCharacter.baseHealth;
            TechLabManager.Instance.baseDamage = selectedCharacter.baseDamage;
            TechLabManager.Instance.baseSpeed = selectedCharacter.baseSpeed;
        }

        if (theGangPanel != null) theGangPanel.SetActive(false);
        if (techLabPanel != null) techLabPanel.SetActive(true);
    }
}