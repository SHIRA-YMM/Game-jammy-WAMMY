using UnityEngine;

public class intro : MonoBehaviour
{
    DialogueManager dialogueManager;
    private void Awake() {
        dialogueManager = FindObjectOfType<DialogueManager>();
        
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        dialogueManager.StartDialogueByIndex(0);
    }

    
}
