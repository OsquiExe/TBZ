using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSpawner : MonoBehaviour
{
    [Header("Catálogo de Personajes")]
    public List<CharacterSO> characterCatalog = new List<CharacterSO>();

    [Header("Punto de Aparición")]
    public Transform spawnPoint;

    [Header("Interfaz (UI) de la Escena")]
    public Image sceneHealthBar;
    public Image sceneStaminaBar;

    [Header("Habilidades Activas Asistentes")]
    [Tooltip("Botón para el primer asistente")]
    public AssistantSkillButton skillButtonUI_1; // Botón 1
    [Tooltip("Botón para el segundo asistente")]
    public AssistantSkillButton skillButtonUI_2; // Botón 2

    private void Awake()
    {
        SpawnSelectedCharacter();
    }

    private void SpawnSelectedCharacter()
    {
        string savedName = PlayerPrefs.GetString("EquippedCharacter", "");
        CharacterSO characterToSpawn = characterCatalog.Find(c => c.characterName == savedName);

        if (characterToSpawn == null && characterCatalog.Count > 0)
        {
            characterToSpawn = characterCatalog[0];
        }

        if (characterToSpawn != null && characterToSpawn.characterPrefab != null)
        {
            Vector3 finalSpawnPos = spawnPoint.position;
            finalSpawnPos.z = 0f;

            GameObject playerInstance = Instantiate(characterToSpawn.characterPrefab, finalSpawnPos, Quaternion.identity);

            PlayerController pc = playerInstance.GetComponent<PlayerController>();
            if (pc != null)
            {
                pc.joystick = FindObjectOfType<VirtualJoystick>();
                if (sceneHealthBar != null) pc.healthBarFill = sceneHealthBar;
            }

            PlayerAttack pa = playerInstance.GetComponent<PlayerAttack>();
            if (pa != null && sceneStaminaBar != null)
            {
                pa.staminaBarFill = sceneStaminaBar;
            }

            // --- CONFIGURAR LOS BOTONES DE ASISTENTES ---
            SetupAssistantSkills(playerInstance.transform);
        }
    }

    private void SetupAssistantSkills(Transform playerTransform)
    {
        if (TechLabManager.equippedItems == null) return;

        // Configuramos el Asistente 1 si está equipado
        if (TechLabManager.equippedItems.ContainsKey(EquipmentType.Asistente))
        {
            EquipmentItemSO asistente1 = TechLabManager.equippedItems[EquipmentType.Asistente];
            if (asistente1 != null && skillButtonUI_1 != null)
            {
                skillButtonUI_1.Setup(asistente1, playerTransform);
            }
        }

        // Configuramos el Asistente 2 si está equipado
        if (TechLabManager.equippedItems.ContainsKey(EquipmentType.Asistente2))
        {
            EquipmentItemSO asistente2 = TechLabManager.equippedItems[EquipmentType.Asistente2];
            if (asistente2 != null && skillButtonUI_2 != null)
            {
                skillButtonUI_2.Setup(asistente2, playerTransform);
            }
        }
    }
}