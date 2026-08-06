using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterCardUI : MonoBehaviour
{
    [Header("Referencias Visuales")]
    public TextMeshProUGUI nameText;
    public Image iconImage;

    [Header("Botones")]
    public Button equipButton; 
    public Button buildButton; 

    private CharacterSO myCharacter;

    public void Setup(CharacterSO character)
    {
        myCharacter = character;

        if (nameText != null) nameText.text = character.characterName;
        if (iconImage != null) iconImage.sprite = character.characterIcon;


        equipButton.onClick.RemoveAllListeners();
        buildButton.onClick.RemoveAllListeners();

        equipButton.onClick.AddListener(OnEquipClicked);
        buildButton.onClick.AddListener(OnBuildClicked);
    }


    public void UpdateUIState(CharacterSO currentlyEquipped)
    {
        bool isEquipped = (myCharacter == currentlyEquipped);

        equipButton.gameObject.SetActive(!isEquipped);
        buildButton.gameObject.SetActive(isEquipped);
    }

    private void OnEquipClicked()
    {
        if (TheGangManager.Instance != null)
        {
            TheGangManager.Instance.EquipCharacter(myCharacter);
        }
    }

    private void OnBuildClicked()
    {
        if (TheGangManager.Instance != null)
        {
            TheGangManager.Instance.SelectCharacterForBuild(myCharacter);
        }
    }
}