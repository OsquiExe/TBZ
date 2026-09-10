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

    [Header("Animators")]
    public Animator kalaAnimator; // Referencia para controlar a Kala

    [Header("Dialogues (Llenar en el Inspector)")]
    public DialogueLine[] startTutorial;
    public DialogueLine[] levelUpTutorial;

    private Queue<DialogueLine> linesQueue = new Queue<DialogueLine>();
    private Action onTutorialComplete;
    private bool isTyping = false;
    private string currentSentence = "";

    public bool hasShownStartTutorial = false;
    public bool hasShownLevelUpTutorial = false;

    // Variable para saber si estamos en el tutorial de Level Up y disparar la animación final
    private bool isPlayingLevelUp = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    IEnumerator Start()
    {
        tutorialPanel.SetActive(false);
        yield return new WaitForEndOfFrame();
        TriggerStartTutorial();
    }

    public void TriggerStartTutorial()
    {
        if (hasShownStartTutorial) return;
        hasShownStartTutorial = true;
        isPlayingLevelUp = false;

        // Animación base al inicio
        if (kalaAnimator != null) kalaAnimator.Play("IdleKala");

        StartDialogue(startTutorial, null);
    }

    public void TriggerLevelUpTutorial(Action onComplete)
    {
        if (hasShownLevelUpTutorial)
        {
            onComplete?.Invoke();
            return;
        }
        hasShownLevelUpTutorial = true;
        isPlayingLevelUp = true;

        // Transición a KalaSignal antes de los powerups
        if (kalaAnimator != null) kalaAnimator.Play("KalaSignal");

        StartDialogue(levelUpTutorial, onComplete);
    }

    private void StartDialogue(DialogueLine[] lines, Action onComplete)
    {
        tutorialPanel.SetActive(true);
        Time.timeScale = 0f;
        onTutorialComplete = onComplete;
        linesQueue.Clear();

        foreach (DialogueLine line in lines)
        {
            linesQueue.Enqueue(line);
        }

        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if (isTyping)
        {
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
        // Si estamos en el tutorial de subir nivel, hacemos la pausa para la animación
        if (isPlayingLevelUp && kalaAnimator != null)
        {
            StartCoroutine(WaitAnimationAndClose());
        }
        else
        {
            // Si es el tutorial de inicio, cerramos de golpe como siempre
            CloseTutorial();
        }
    }

    private IEnumerator WaitAnimationAndClose()
    {
        // 1. Borramos el texto para que la pantalla quede limpia
        dialogueText.text = "";
        nameText.text = "";

        // 2. Disparamos la animación final
        kalaAnimator.Play("KalaWhatever");

        // 3. Esperamos 1.5 segundos (en tiempo real porque el juego está pausado)
        // Podés cambiar este 1.5f por lo que dure exactamente tu animación
        yield return new WaitForSecondsRealtime(1.5f);

        // 4. Ahora sí, cerramos todo
        CloseTutorial();
    }

    private void CloseTutorial()
    {
        tutorialPanel.SetActive(false);
        Time.timeScale = 1f;
        onTutorialComplete?.Invoke();
    }
}