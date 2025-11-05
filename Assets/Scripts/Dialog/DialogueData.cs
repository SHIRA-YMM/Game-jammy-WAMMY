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
}