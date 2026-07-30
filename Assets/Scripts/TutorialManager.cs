using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Serializable]
public struct DialogueLine
{
    public string characterName;
    [TextArea(2, 5)] public string dialogueText;
}

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance;

    [Header("UI References")]
    public GameObject tutorialPanel;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;

    [Header("Dialogues (Llenar en el Inspector)")]
    public DialogueLine[] startTutorial;
    public DialogueLine[] levelUpTutorial;

    private Queue<DialogueLine> linesQueue = new Queue<DialogueLine>();
    private Action onTutorialComplete;
    private bool isTyping = false;
    private string currentSentence = "";

    // Banderas para saber si ya los mostramos en esta partida
    public bool hasShownStartTutorial = false;
    public bool hasShownLevelUpTutorial = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // CORRECCIÓN: Ahora Start es una Corutina que espera un frame.
    IEnumerator Start()
    {
        tutorialPanel.SetActive(false);

        // Esperamos un frame para que cualquier otro script que ponga Time.timeScale = 1 termine primero.
        yield return new WaitForEndOfFrame();

        // Al arrancar el nivel 1-1, disparamos el tutorial inicial asegurando la pausa.
        TriggerStartTutorial();
    }

    public void TriggerStartTutorial()
    {
        if (hasShownStartTutorial) return;
        hasShownStartTutorial = true;

        // Le pasamos 'null' porque al terminar solo queremos despausar
        StartDialogue(startTutorial, null);
    }

    public void TriggerLevelUpTutorial(Action onComplete)
    {
        if (hasShownLevelUpTutorial)
        {
            // Si ya lo vimos antes, abrimos los powerups directamente
            onComplete?.Invoke();
            return;
        }
        hasShownLevelUpTutorial = true;

        // Le pasamos la función de abrir los powerups para que la ejecute al terminar de hablar
        StartDialogue(levelUpTutorial, onComplete);
    }

    private void StartDialogue(DialogueLine[] lines, Action onComplete)
    {
        tutorialPanel.SetActive(true);
        Time.timeScale = 0f; // Ahora esta pausa sí va a ser la orden definitiva
        onTutorialComplete = onComplete;
        linesQueue.Clear();

        foreach (DialogueLine line in lines)
        {
            linesQueue.Enqueue(line);
        }

        DisplayNextSentence();
    }

    // ESTA FUNCIÓN VA EN EL ONCLICK DEL BOTÓN DEL PANEL
    public void DisplayNextSentence()
    {
        if (isTyping)
        {
            // Si el jugador toca la pantalla mientras escribe, autocompleta el texto
            StopAllCoroutines();
            dialogueText.text = currentSentence;
            isTyping = false;
            return;
        }

        if (linesQueue.Count == 0)
        {
            EndDialogue();
            return;
        }

        DialogueLine line = linesQueue.Dequeue();
        nameText.text = line.characterName;
        currentSentence = line.dialogueText;

        StopAllCoroutines();
        StartCoroutine(TypeSentence(currentSentence));
    }

    IEnumerator TypeSentence(string sentence)
    {
        isTyping = true;
        dialogueText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSecondsRealtime(0.03f); 
        }
        isTyping = false;
    }

    private void EndDialogue()
    {
        tutorialPanel.SetActive(false);
        Time.timeScale = 1f;

       
        onTutorialComplete?.Invoke();
    }
}