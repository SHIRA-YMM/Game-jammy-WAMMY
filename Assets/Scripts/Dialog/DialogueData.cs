using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class DialogueLine
{
    [TextArea(1, 4)] public string text;
    public string characterName;
    public Sprite portrait;
}

[CreateAssetMenu(fileName = "DialogueData", menuName = "Dialogue/Dialogue Data", order = 0)]
public class DialogueData : ScriptableObject
{
    public List<DialogueLine> lines = new List<DialogueLine>();
    [Header("Optional: load scene when this DialogueData finishes")]
    public bool loadSceneOnEnd = false;
    [Tooltip("Nama scene yang akan dimuat (isi Build Settings dengan scene ini).")]
    public string sceneToLoad = "";
    [Tooltip("Delay (detik) sebelum memulai load scene. Berguna untuk efek/transition.")]
    public float delayBeforeLoad = 0f;
}