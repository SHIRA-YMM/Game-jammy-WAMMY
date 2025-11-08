using UnityEngine;

public enum SkillType
{
    Attack,     // Serangan ke musuh
    Heal,       // Menyembuhkan diri / tim
    DefenseBuff // Menambah pertahanan / efek defend semua
}

[CreateAssetMenu(menuName = "Combat/Skill")]
public class SkillSO : ScriptableObject
{
    public string skillName;
    [TextArea] public string description;
    public SkillType skillType = SkillType.Attack;

    [Header("Damage / Heal Config")]
    public int flatValue = 0;              // Damage / Heal / Shield nilai tetap
    public float valueMultiplier = 1f;     // Multiplier terhadap atk (untuk attack/heal)
    public bool affectsAll = false;        // Jika true, efek ke semua karakter

    [Header("Shop Config")]
    public bool purchasable = true;        // untuk shop nanti
    public int price = 0;                  // harga di shop (optional)
}
