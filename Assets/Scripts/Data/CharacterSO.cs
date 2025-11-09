using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Combat/Character")]
public class CharacterSO : ScriptableObject
{
    public string characterName;
    public Sprite portrait;
    public int maxHP = 100;
    public int atk = 10;
    public SkillSO[] skills;
    public bool isEnemy = false;

    // Simpan skill yang dimiliki player ke PlayerPrefs (per character)
    public void AddSkill(SkillSO newSkill)
    {
        List<SkillSO> skillList = new List<SkillSO>(skills);
        if (!skillList.Contains(newSkill))
        {
            skillList.Add(newSkill);
            skills = skillList.ToArray();
            SaveSkills();
        }
    }

    public void SaveSkills()
    {
        List<string> skillNames = new List<string>();
        foreach (var s in skills)
        {
            if (s != null)
                skillNames.Add(s.skillName);
        }

        string key = $"Skills_{characterName}";
        string saveData = string.Join(",", skillNames);
        PlayerPrefs.SetString(key, saveData);
        PlayerPrefs.Save();
    }

    public void LoadSkills(SkillSO[] allSkills)
    {
        string key = $"Skills_{characterName}";
        if (!PlayerPrefs.HasKey(key)) return;

        string saved = PlayerPrefs.GetString(key);
        string[] skillNames = saved.Split(',');

        List<SkillSO> loaded = new List<SkillSO>();
        foreach (var name in skillNames)
        {
            SkillSO skill = System.Array.Find(allSkills, s => s.skillName == name);
            if (skill != null)
                loaded.Add(skill);
        }
        skills = loaded.ToArray();
    }
}
