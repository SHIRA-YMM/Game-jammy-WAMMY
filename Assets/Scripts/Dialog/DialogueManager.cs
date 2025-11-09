using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class DialogueManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI contentText;
    public Image portraitImage;

    [Header("Typing settings")]
    public bool useTypewriter = true;
    public float typeSpeed = 0.03f;

    [Header("Dialog Assets")]
    public List<DialogueData> dialogues = new List<DialogueData>();

    [Header("Events")]
    public UnityEvent OnDialogueStart;
    public UnityEvent OnDialogueEnd;
    public UnityEvent OnBeforeSceneLoad; // dipanggil tepat sebelum memulai load (opsional)

    [Header("Custom Events")]
    public UnityEvent OnIntroDialogueComplete;

    // Internal variables
    private DialogueData currentDialogue;
    private int currentLineIndex = -1;
    private Coroutine typingCoroutine;
    private bool isTyping = false;

    private void Awake()
    {
        if (dialoguePanel != null) dialoguePanel.SetActive(false);
    }

    public void StartDialogue(DialogueData dialogue)
    {
        if (dialogue == null || dialogue.lines.Count == 0) return;
        currentDialogue = dialogue;
        currentLineIndex = -1;
        if (dialoguePanel != null) dialoguePanel.SetActive(true);
        OnDialogueStart?.Invoke();
        NextLine();
    }

    public void StartDialogueByIndex(int index)
    {
        if (index < 0 || index >= dialogues.Count) return;
        StartDialogue(dialogues[index]);
    }

    /// <summary>
    /// Panggil ini dari tombol "Next" untuk lanjut ke baris berikutnya
    /// Jika sedang mengetik, fungsi ini akan langsung menampilkan seluruh teks baris saat ini.
    /// </summary>
    public void NextLine()
    {
        if (currentDialogue == null) return;

        if (isTyping)
        {
            FinishTypingInstantly();
            return;
        }

        currentLineIndex++;
        if (currentLineIndex >= currentDialogue.lines.Count)
        {
            EndDialogue();
            return;
        }

        ShowLine(currentDialogue.lines[currentLineIndex]);
    }

    private void ShowLine(DialogueLine line)
    {
        if (nameText != null)
            nameText.text = line.characterName;

        if (portraitImage != null)
        {
            if (line.portrait != null)
            {
                portraitImage.sprite = line.portrait;
                portraitImage.gameObject.SetActive(true);
            }
            else
            {
                portraitImage.gameObject.SetActive(false);
            }
        }

        if (useTypewriter && contentText != null)
        {
            if (typingCoroutine != null)
                StopCoroutine(typingCoroutine);
            typingCoroutine = StartCoroutine(TypeText(line.text));
        }
        else
        {
            if (contentText != null)
                contentText.text = line.text;
        }
    }

    private IEnumerator TypeText(string fullText)
    {
        isTyping = true;
        contentText.text = "";

        for (int i = 0; i < fullText.Length; i++)
        {
            contentText.text += fullText[i];
            yield return new WaitForSeconds(typeSpeed);
        }

        isTyping = false;
        typingCoroutine = null;
    }

    private void FinishTypingInstantly()
    {
        if (currentDialogue == null) return;
        if (currentLineIndex < 0 || currentLineIndex >= currentDialogue.lines.Count) return;

        var line = currentDialogue.lines[currentLineIndex];
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        contentText.text = line.text;
        isTyping = false;
        typingCoroutine = null;
    }

    private void EndDialogue()
    {
        // Simpan data yang mungkin kita butuhkan untuk scene load sebelum di-null-kan
        DialogueData dialogueToCheck = currentDialogue;

        currentDialogue = null;
        currentLineIndex = -1;
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);
        OnDialogueEnd?.Invoke();
        OnIntroDialogueComplete?.Invoke(); // Tambahkan ini

        // Jika DialogueData meminta load scene, jalankan coroutine untuk itu
        if (dialogueToCheck != null && dialogueToCheck.loadSceneOnEnd && !string.IsNullOrEmpty(dialogueToCheck.sceneToLoad))
        {
            StartCoroutine(HandleSceneLoadRoutine(dialogueToCheck.sceneToLoad, dialogueToCheck.delayBeforeLoad));
        }
    }

    private IEnumerator HandleSceneLoadRoutine(string sceneName, float delay)
    {
        // Event hook sebelum memulai load (mis: untuk play transition anim)
        OnBeforeSceneLoad?.Invoke();

        if (delay > 0f)
            yield return new WaitForSeconds(delay);

        // Mulai load scene async agar tidak freeze (bisa dimodifikasi jadi LoadScene untuk simplicity)
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        if (op == null)
        {
            Debug.LogWarning($"DialogueManager: Scene '{sceneName}' tidak ditemukan atau belum dimasukkan ke Build Settings.");
            yield break;
        }

        // optional: tunggu sampai selesai (atau biarkan background load)
        while (!op.isDone)
            yield return null;
    }

    /// <summary>
    /// Utility: cek apakah dialog sedang berlangsung
    /// </summary>
    public bool IsDialoguePlaying()
    {
        return currentDialogue != null;
    }

    /// <summary>
    /// Reset dialog jika diperlukan
    /// </summary>
    public void ForceEndDialogue()
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);
        EndDialogue();
    }

    // Di akhir dialog intro
    public void OnIntroComplete()
    {
        if (GameProgressManager.Instance != null)
        {
            GameProgressManager.Instance.GoToNextPhase(); // Akan load Day1
        }
    }

    // Di akhir dialog intro
    
}
