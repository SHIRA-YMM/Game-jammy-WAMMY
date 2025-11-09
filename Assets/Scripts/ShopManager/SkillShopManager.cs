using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillShopManager : MonoBehaviour
{
    [Header("Skill Data")]
    public SkillSO[] availableSkills;
    public CharacterSO johan;
    public CharacterSO lie;
    public CharacterSO bert;

    [Header("Manual Skill Buttons (assign in Inspector)")]
    public Button[] skillButtons;               // tombol (3)
    public TextMeshProUGUI[] priceTexts;        // teks harga per skill (3)
    public TextMeshProUGUI[] skillNameTexts;    // teks nama per skill (3)

    [Header("Coin UI")]
    public TextMeshProUGUI coinText;            // tampilan koin global

    [Header("Info Message")]
    public TextMeshProUGUI infoText;            // untuk "sudah dibeli", "koin tidak cukup", dll
    public float messageDuration = 2f;          // berapa lama pesan tampil

    Coroutine infoCoroutine;

    void Start()
    {
        // load saved skills from PlayerPrefs into CharacterSO assets so runtime scenes read them
        var allSkills = Resources.FindObjectsOfTypeAll<SkillSO>();
        if (johan != null) johan.LoadSkills(allSkills);
        if (lie != null) lie.LoadSkills(allSkills);
        if (bert != null) bert.LoadSkills(allSkills);

        SetupShop();
        UpdateCoinUI();
    }

    void OnEnable()
    {
        // jika memakai event di GlobalCoinManager, bisa subscribe di sini.
        // GlobalCoinManager.OnCoinChanged += UpdateCoinUI; // jika event ada
    }

    void OnDisable()
    {
        // GlobalCoinManager.OnCoinChanged -= UpdateCoinUI;
    }

    void SetupShop()
    {
        // safety checks
        int count = availableSkills != null ? availableSkills.Length : 0;
        for (int i = 0; i < skillButtons.Length; i++)
        {
            // if index beyond available skills -> disable leftover UI
            if (i >= count || availableSkills[i] == null)
            {
                skillButtons[i].gameObject.SetActive(false);
                if (i < skillNameTexts.Length) skillNameTexts[i].text = "";
                if (i < priceTexts.Length) priceTexts[i].text = "";
                continue;
            }

            var skill = availableSkills[i];

            // remove old listeners (prevents multiple calls)
            skillButtons[i].onClick.RemoveAllListeners();

            // set texts (ensure arrays are big enough)
            if (i < skillNameTexts.Length && skillNameTexts[i] != null)
                skillNameTexts[i].text = skill.skillName;

            if (i < priceTexts.Length && priceTexts[i] != null)
                priceTexts[i].text = $"{skill.price} Coins";

            // set button interactable based on ownership for the target character
            bool owned = IsSkillOwned(skill);
            skillButtons[i].interactable = !owned;

            // if owned, tampilkan keterangan
            if (owned && i < priceTexts.Length && priceTexts[i] != null)
                priceTexts[i].text = "Owned";

            // add listener
            int index = i;
            skillButtons[i].onClick.AddListener(() => BuySkill(availableSkills[index]));
        }
    }

    // helper baru: tentukan karakter target berdasarkan tipe skill
    CharacterSO GetTargetCharacterForSkill(SkillSO skill)
    {
        if (skill == null) return null;
        switch (skill.skillType)
        {
            case SkillType.Attack: return johan;
            case SkillType.Heal: return lie;
            case SkillType.DefenseBuff: return bert;
            default: return johan;
        }
    }

    bool IsSkillOwned(SkillSO skill)
    {
        if (skill == null) return false;
        var target = GetTargetCharacterForSkill(skill);
        if (target == null || target.skills == null) return false;
        foreach (var s in target.skills)
        {
            if (s == skill) return true;
        }
        return false;
    }

    void BuySkill(SkillSO skill)
    {
        if (skill == null)
        {
            ShowInfo("Skill tidak valid.");
            return;
        }

        if (GlobalCoinManager.Instance == null)
        {
            ShowInfo("GlobalCoinManager tidak ditemukan.");
            return;
        }

        var target = GetTargetCharacterForSkill(skill);
        if (target == null)
        {
            ShowInfo("Tidak ada karakter target untuk skill ini.");
            return;
        }

        // jika sudah dimiliki (cek pada karakter target)
        if (IsSkillOwned(skill))
        {
            ShowInfo("Sudah dibeli.");
            return;
        }

        // coba pengeluaran koin
        if (GlobalCoinManager.Instance.SpendCoins(skill.price))
        {
            // sukses beli -> tambahkan hanya ke karakter target
            target.AddSkill(skill); // AddSkill memanggil SaveSkills()

            // update UI
            UpdateCoinUI();
            SetupShop(); // untuk men-disable tombol dan ubah teks jadi "Owned"

            ShowInfo($"Berhasil membeli {skill.skillName} untuk {target.characterName}.");
        }
        else
        {
            // koin tidak cukup
            ShowInfo("Koin tidak cukup!");
        }
    }

    void UpdateCoinUI()
    {
        if (coinText != null)
        {
            int c = (GlobalCoinManager.Instance != null) ? GlobalCoinManager.Instance.coins : 0;
            coinText.text = $"Coins: {c}";
        }
    }

    void ShowInfo(string message)
    {
        if (infoText == null)
            return;

        // stop previous coroutine jika masih berjalan
        if (infoCoroutine != null)
            StopCoroutine(infoCoroutine);

        infoCoroutine = StartCoroutine(ShowInfoRoutine(message));
    }

    IEnumerator ShowInfoRoutine(string message)
    {
        infoText.text = message;
        infoText.gameObject.SetActive(true);
        yield return new WaitForSeconds(messageDuration);
        infoText.text = "";
        infoText.gameObject.SetActive(false);
        infoCoroutine = null;
    }
}
