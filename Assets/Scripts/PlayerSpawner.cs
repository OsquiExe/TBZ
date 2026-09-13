using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSpawner : MonoBehaviour
{
    [Header("Catálogo de Personajes")]
    [Tooltip("Arrastrá acá a Lumina, Ninja y todos los CharacterSO que existan")]
    public List<CharacterSO> characterCatalog = new List<CharacterSO>();

    [Header("Punto de Aparición")]
    public Transform spawnPoint;

    [Header("Interfaz (UI) de la Escena")] 
    public Image sceneHealthBar;

#if UNITY_EDITOR
    [Space(10)]
    public string DEBUG_CHARACTER;
#endif

    private void Awake()
    {
#if UNITY_EDITOR
        SpawnSelectedCharacter(DEBUG_CHARACTER);
#else
        SpawnSelectedCharacter();
#endif
    }

    void SpawnSelectedCharacter(string savedName) {
        CharacterSO characterToSpawn = characterCatalog.Find(c => c.characterName == savedName);

        if(characterToSpawn == null && characterCatalog.Count > 0) {
            characterToSpawn = characterCatalog[0];
        }

        if(characterToSpawn != null && characterToSpawn.characterPrefab != null) {
            Vector3 finalSpawnPos = spawnPoint.position;
            finalSpawnPos.z = 0f;

            GameObject playerInstance = Instantiate(characterToSpawn.characterPrefab, finalSpawnPos, Quaternion.identity);

            PlayerController pc = playerInstance.GetComponent<PlayerController>();
            if(pc != null) {

                pc.joystick = FindObjectOfType<VirtualJoystick>();


                if(sceneHealthBar != null) {
                    pc.healthBarFill = sceneHealthBar;
                }
            }

            Debug.Log($"[Spawner] Ha aparecido: {characterToSpawn.characterName}");
        } else {
            Debug.LogError("[Spawner] Faltan configurar los personajes en el catálogo.");
        }
    }

    private void SpawnSelectedCharacter()
    {
        string savedName = PlayerPrefs.GetString("EquippedCharacter", "");
        SpawnSelectedCharacter(savedName);
    }
}