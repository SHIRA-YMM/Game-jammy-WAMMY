using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class intro : MonoBehaviour
{
    public GameObject fadein;
    DialogueManager dialogueManager;
    private void Awake() {
        dialogueManager = FindObjectOfType<DialogueManager>();
        
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(startAnimation());
        dialogueManager.StartDialogueByIndex(0);
    }

    IEnumerator startAnimation()
    {
        fadein.SetActive(true);
        yield return new WaitForSeconds(2f);
        fadein.SetActive(false);
    }

}