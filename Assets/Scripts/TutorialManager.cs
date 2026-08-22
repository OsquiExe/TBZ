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

    [Header("Animadores de Personajes")]
    public Animator kalaAnimator;
    public Animator ramoAnimator;

    [Header("Dialogues (Llenar en el Inspector)")]
    public DialogueLine[] startTutorial;
    public DialogueLine[] levelUpTutorial;

    private Queue<DialogueLine> linesQueue = new Queue<DialogueLine>();
    private Action onTutorialComplete;
    private bool isTyping = false;
    private string currentSentence = "";

    public bool hasShownStartTutorial = false;
    public bool hasShownLevelUpTutorial = false;

    private bool isLevelUpTutorialActive = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    IEnumerator Start()
    {
        tutorialPanel.SetActive(false);

        if (kalaAnimator != null) kalaAnimator.gameObject.SetActive(false);
        if (ramoAnimator != null) ramoAnimator.gameObject.SetActive(false);

        yield return new WaitForEndOfFrame();
        TriggerStartTutorial();
    }

    public void TriggerStartTutorial()
    {
        if (hasShownStartTutorial) return;
        hasShownStartTutorial = true;
        isLevelUpTutorialActive = false;
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
        isLevelUpTutorialActive = true;
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


        if (isLevelUpTutorialActive)
        {
            if (line.characterName.Contains("Ramo"))
            {

                if (ramoAnimator != null)
                {
                    ramoAnimator.gameObject.SetActive(true);
                    ramoAnimator.Play("Ramo");
                }
                if (kalaAnimator != null)
                {
                    kalaAnimator.gameObject.SetActive(true);
                    kalaAnimator.Play("IdleKala");
                }
            }
            else
            {
     
                if (kalaAnimator != null)
                {
                    kalaAnimator.gameObject.SetActive(true);
                }

                if (ramoAnimator != null)
                {
                    ramoAnimator.gameObject.SetActive(true);
                    ramoAnimator.Play("IdleRamo");
                }

                if (linesQueue.Count == 0)
                {
                    if (kalaAnimator != null) kalaAnimator.Play("KalaWhatever");
                }
                else
                {
                    if (kalaAnimator != null) kalaAnimator.Play("KalaSignal");
                }
            }
        }
        else
        {
   
            if (kalaAnimator != null)
            {
                kalaAnimator.gameObject.SetActive(true);
                kalaAnimator.Play("IdleKala");
            }
            if (ramoAnimator != null) ramoAnimator.gameObject.SetActive(false);
        }

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

        if (kalaAnimator != null) kalaAnimator.gameObject.SetActive(false);
        if (ramoAnimator != null) ramoAnimator.gameObject.SetActive(false);

        onTutorialComplete?.Invoke();
    }
}