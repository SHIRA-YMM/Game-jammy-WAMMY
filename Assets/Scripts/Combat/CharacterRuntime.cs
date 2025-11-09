using UnityEngine;
using UnityEngine.UI;

public class CharacterRuntime : MonoBehaviour
{
    public CharacterSO characterData;
    [HideInInspector] public int currentHP;
    [HideInInspector] public bool isDefending = false;

    // new: defense buff that blocks the next incoming hit (used by DefenseBuff skill)
    [HideInInspector] public bool defenseBuffActive = false;

    public Slider hpSlider;
    // NOTE: remove any hpText reference in inspector/prefab if exists
    // public TextMeshProUGUI hpText; // removed

    // UI root (kartu) dan portrait image untuk di-dim
    public CanvasGroup rootCanvasGroup; // assign di inspector (kartu UI)
    public Image portraitImage; // assign portrait Image di kartu

    // runtime values
    private int runtimeMaxHP;
    private bool[] skillUsed;

    void Awake()
    {
        if (characterData != null)
            InitFromSO(characterData);
    }

    public void InitFromSO(CharacterSO so)
    {
        InitFromSO(so, so.maxHP);
    }

    public void InitFromSO(CharacterSO so, int overrideMaxHP)
    {
        characterData = so;
        runtimeMaxHP = overrideMaxHP;
        currentHP = runtimeMaxHP;
        UpdateHPUI();

        int len = (so.skills != null) ? so.skills.Length : 0;
        skillUsed = new bool[len];
        for (int i = 0; i < len; i++) skillUsed[i] = false;

        // ensure visible (do not disable)
        if (rootCanvasGroup != null)
        {
            rootCanvasGroup.alpha = 1f;
            rootCanvasGroup.blocksRaycasts = true;
            rootCanvasGroup.interactable = true;
        }
        if (portraitImage != null) portraitImage.color = Color.white;
    }

    // expose runtime max HP for other systems
    public int GetMaxHP() => runtimeMaxHP;

    public void ResetSkills()
    {
        if (skillUsed == null) return;
        for (int i = 0; i < skillUsed.Length; i++) skillUsed[i] = false;

        // reset defend flags
        isDefending = false;
        defenseBuffActive = false;
    }

    public bool IsSkillAvailable(int index)
    {
        if (characterData == null || characterData.skills == null) return false;
        if (index < 0 || index >= characterData.skills.Length) return false;
        if (skillUsed == null) return true;
        return !skillUsed[index];
    }

    public bool UseSkill(int index)
    {
        if (!IsSkillAvailable(index)) return false;
        skillUsed[index] = true;
        return true;
    }

    public void TakeDamage(int dmg)
    {
        // if either player pressed Defend (isDefending) OR got DefenseBuff (defenseBuffActive),
        // block the incoming damage once and clear the flag.
        if (isDefending || defenseBuffActive)
        {
            dmg = 0;
            isDefending = false;
            defenseBuffActive = false;
        }

        currentHP -= dmg;
        if (currentHP < 0) currentHP = 0;
        UpdateHPUI();
    }

    public bool IsAlive() => currentHP > 0;

    public void HealToFull()
    {
        currentHP = runtimeMaxHP;
        UpdateHPUI();
        ResetSkills();
        isDefending = false;
        defenseBuffActive = false;
    }

    public void UpdateHPUI()
    {
        if (hpSlider != null && runtimeMaxHP > 0)
            hpSlider.value = (float)currentHP / runtimeMaxHP;
        // intentionally no hp text
    }

    // dim the non-active players (don't disable gameobject)
    // dim == true => gray/darker; dim == false => normal
    public void SetDimmed(bool dim)
    {
        if (rootCanvasGroup != null)
        {
            rootCanvasGroup.alpha = dim ? 0.45f : 1f;
            rootCanvasGroup.interactable = !dim;
            rootCanvasGroup.blocksRaycasts = !dim;
        }

        if (portraitImage != null)
        {
            portraitImage.color = dim ? new Color(0.5f, 0.5f, 0.5f, 1f) : Color.white;
        }
    }

    // new helper to apply defense buff (used by skill)
    public void ApplyDefenseBuff()
    {
        if (IsAlive())
            defenseBuffActive = true;
    }
}
