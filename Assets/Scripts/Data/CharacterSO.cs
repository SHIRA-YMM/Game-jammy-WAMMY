using UnityEngine;

[CreateAssetMenu(menuName = "Combat/Character")]
public class CharacterSO : ScriptableObject
{
    public string characterName;
    public Sprite portrait;
    public int maxHP = 100;
    public int atk = 10;
    public SkillSO[] skills;
    public bool isEnemy = false; // true for enemy/boss SO
}
