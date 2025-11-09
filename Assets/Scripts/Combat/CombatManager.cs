using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CombatManager : MonoBehaviour
{
    [Header("Players (in order bert -> lie -> johan)")]
    public CharacterRuntime bertRuntime;
    public CharacterRuntime lieRuntime;
    public CharacterRuntime johanRuntime;

    [Header("Enemy SOs (5 different enemy SOs)")]
    public CharacterSO[] enemyPool; // at least totalDays entries

    [Header("Runtime Enemy")]
    public CharacterRuntime enemyRuntimePrefab; // scene instance or prefab asset
    private CharacterRuntime enemyRuntime;

    [Header("Battle Config")]
    public int totalDays = 5;
    public int currentDay = 1;
    public int normalEnemyHP = 200;
    public int bossHP = 300;

    [Header("References")]
    public UIManager uiManager;

    [Header("Victory UI")]
    public GameObject victoryPanel; // Assign di inspector

    // internal
    private List<CharacterSO> shuffledEnemies;
    private List<CharacterRuntime> turnOrder = new List<CharacterRuntime>();
    private bool waitingPlayerInput = false;
    private ActionSelection pendingAction;
    private CharacterRuntime currentActor;

    private enum ActionType { Attack, Defend, Skill, None }
    private class ActionSelection
    {
        public ActionType type;
        public SkillSO skill;
        public int skillIndex = -1;
    }

    void Start()
    {
        EnsureSceneReferences();

        if (enemyPool == null || enemyPool.Length < totalDays)
            Debug.LogWarning("Pastikan enemyPool memiliki minimal " + totalDays + " enemy SO.");

        ShuffleEnemiesForDays();
        SetupDay(currentDay);
        StartCoroutine(CombatLoop());
    }

    void ShuffleEnemiesForDays()
    {
        shuffledEnemies = enemyPool.ToList();
        for (int i = 0; i < shuffledEnemies.Count; i++)
        {
            int r = Random.Range(i, shuffledEnemies.Count);
            var tmp = shuffledEnemies[i];
            shuffledEnemies[i] = shuffledEnemies[r];
            shuffledEnemies[r] = tmp;
        }
    }

    void SetupDay(int day)
    {
        Debug.Log($"Setup Day {day}");

        // reset players (use scene instances assigned in inspector)
        bertRuntime.InitFromSO(bertRuntime.characterData);
        lieRuntime.InitFromSO(lieRuntime.characterData);
        johanRuntime.InitFromSO(johanRuntime.characterData);

        // pick enemy SO
        CharacterSO chosenEnemySO = shuffledEnemies[Mathf.Clamp(day - 1, 0, shuffledEnemies.Count - 1)];
        int enemyHP = (day == totalDays) ? bossHP : normalEnemyHP;

        if (enemyRuntimePrefab == null)
        {
            Debug.LogError("Assign Enemy Runtime Prefab (scene instance or prefab asset)!");
            return;
        }

        bool isSceneInstance = enemyRuntimePrefab.gameObject.scene.IsValid();
        if (isSceneInstance)
        {
            enemyRuntime = enemyRuntimePrefab;
        }
        else
        {
            if (enemyRuntime != null && enemyRuntime.gameObject != null)
                Destroy(enemyRuntime.gameObject);

            enemyRuntime = Instantiate(enemyRuntimePrefab, transform);
        }

        // ensure enemy active and UI visible
        enemyRuntime.gameObject.SetActive(true);
        if (enemyRuntime.rootCanvasGroup != null)
        {
            enemyRuntime.rootCanvasGroup.alpha = 1f;
            enemyRuntime.rootCanvasGroup.interactable = true;
            enemyRuntime.rootCanvasGroup.blocksRaycasts = true;
        }

        enemyRuntime.InitFromSO(chosenEnemySO, enemyHP);

        // build turn order
        turnOrder.Clear();
        turnOrder.Add(bertRuntime);
        turnOrder.Add(lieRuntime);
        turnOrder.Add(johanRuntime);
        turnOrder.Add(enemyRuntime);

        Debug.Log($"Enemy assigned: {enemyRuntime.name} active:{enemyRuntime.gameObject.activeSelf} hp:{enemyRuntime.currentHP}/{enemyHP}");
    }

    IEnumerator CombatLoop()
    {
        yield return null;
        int index = 0;
        while (true)
        {
            currentActor = null;

            if (!IsAnyPlayerAlive())
            {
                if (uiManager != null) uiManager.SetTurnText("YOU LOSE");
                Debug.Log("Kalah - Restart day");
                yield return new WaitForSeconds(1f);
                RestartDay();
                yield break;
            }

            if (!enemyRuntime.IsAlive())
            {
                if (uiManager != null)
                    uiManager.ShowInfo("Enemy Defeated!");
                ShowVictoryPanel(); // Tambahkan ini
                yield break;
            }

            var actor = turnOrder[index % turnOrder.Count];
            currentActor = actor;
            if (!actor.IsAlive())
            {
                index++;
                continue;
            }

            // dim players (enemy never dimmed)
            if (actor == enemyRuntime)
            {
                UpdatePlayerDimStates(null);
                if (uiManager != null) uiManager.SetTurnText("ENEMY TURN");
            }
            else
            {
                UpdatePlayerDimStates(actor);
                if (uiManager != null) uiManager.SetTurnText(actor.characterData.characterName + " TURN");
            }

            // enemy turn
            if (actor == enemyRuntime)
            {
                if (uiManager != null) uiManager.SetActionButtonsEnabled(false, false);
                yield return new WaitForSeconds(0.5f);
                yield return StartCoroutine(EnemyTurn());
                if (uiManager != null) uiManager.turnText.gameObject.SetActive(false);
            }
            else
            {
                // player's turn
                bool skillAvailable = false;
                if (actor.characterData.skills != null && actor.characterData.skills.Length > 0)
                    skillAvailable = actor.IsSkillAvailable(0);

                if (uiManager != null) uiManager.SetActionButtonsEnabled(true, skillAvailable);

                waitingPlayerInput = true;
                pendingAction = new ActionSelection { type = ActionType.None, skillIndex = -1 };

                while (waitingPlayerInput) yield return null;

                yield return StartCoroutine(ProcessPlayerAction(actor, pendingAction));

                if (uiManager != null) uiManager.SetActionButtonsEnabled(false, false);
                if (uiManager != null) uiManager.turnText.gameObject.SetActive(false);
            }

            index++;
            yield return new WaitForSeconds(0.2f);
        }
    }

    IEnumerator ProcessPlayerAction(CharacterRuntime actor, ActionSelection action)
    {
        if (action.type == ActionType.Attack)
        {
            int dmg = actor.characterData.atk;
            enemyRuntime.TakeDamage(dmg);
            if (uiManager != null) uiManager.ShowInfo($"{actor.characterData.characterName} attacks {dmg} dmg");
            Debug.Log($"{actor.characterData.characterName} attack -> {dmg}");
            yield return new WaitForSeconds(0.6f);
        }
        else if (action.type == ActionType.Defend)
        {
            actor.isDefending = true;
            if (uiManager != null) uiManager.ShowInfo($"{actor.characterData.characterName} defend!");
            Debug.Log($"{actor.characterData.characterName} defend");
            yield return new WaitForSeconds(0.6f);
        }
        else if (action.type == ActionType.Skill)
        {
            if (action.skill == null)
            {
                if (uiManager != null) uiManager.ShowSkillMissing();
                yield return new WaitForSeconds(0.6f);
            }
            else
            {
                SkillSO skill = action.skill;
                string user = actor.characterData.characterName;

                switch (skill.skillType)
                {
                    case SkillType.Attack:
                        int dmg = Mathf.CeilToInt(skill.flatValue + actor.characterData.atk * skill.valueMultiplier);
                        enemyRuntime.TakeDamage(dmg);
                        if (uiManager != null) uiManager.ShowInfo($"{user} uses {skill.skillName}! Damage {dmg}");
                        Debug.Log($"{user} skill Attack {skill.skillName} -> {dmg}");
                        break;

                    case SkillType.Heal:
                        List<CharacterRuntime> targets = new List<CharacterRuntime>();
                        if (skill.affectsAll)
                        {
                            if (bertRuntime.IsAlive()) targets.Add(bertRuntime);
                            if (lieRuntime.IsAlive()) targets.Add(lieRuntime);
                            if (johanRuntime.IsAlive()) targets.Add(johanRuntime);
                        }
                        else
                        {
                            targets.Add(actor);
                        }

                        foreach (var t in targets)
                        {
                            int heal = Mathf.CeilToInt(skill.flatValue + actor.characterData.atk * skill.valueMultiplier);
                            t.currentHP += heal;
                            if (t.currentHP > t.GetMaxHP()) t.currentHP = t.GetMaxHP();
                            t.UpdateHPUI();
                            if (uiManager != null) uiManager.ShowInfo($"{user} heals {t.characterData.characterName} +{heal} HP");
                        }
                        Debug.Log($"{user} uses {skill.skillName} (Heal)");
                        break;

                    case SkillType.DefenseBuff:
                        bertRuntime.isDefending = bertRuntime.IsAlive();
                        lieRuntime.isDefending = lieRuntime.IsAlive();
                        johanRuntime.isDefending = johanRuntime.IsAlive();
                        if (uiManager != null) uiManager.ShowInfo($"{user} uses {skill.skillName}! defense for everyone!");
                        Debug.Log($"{user} skill defense buff active");
                        break;
                }

                if (action.skillIndex >= 0)
                {
                    bool used = actor.UseSkill(action.skillIndex);
                    if (used)
                    {
                        if (uiManager != null) uiManager.SetActionButtonsEnabled(false, false);
                    }
                }

                yield return new WaitForSeconds(0.8f);
            }
        }
    }

    IEnumerator EnemyTurn()
    {
        var alive = new List<CharacterRuntime>();
        if (bertRuntime.IsAlive()) alive.Add(bertRuntime);
        if (lieRuntime.IsAlive()) alive.Add(lieRuntime);
        if (johanRuntime.IsAlive()) alive.Add(johanRuntime);

        if (alive.Count == 0) yield break;

        var target = alive[Random.Range(0, alive.Count)];
        float pct = Random.Range(0.07f, 0.10f);
        int dmg = Mathf.CeilToInt(target.GetMaxHP() * pct);
        target.TakeDamage(dmg);

        if (uiManager != null) uiManager.ShowInfo($"Enemy attacks {target.characterData.characterName} -> {dmg} dmg");
        Debug.Log($"Enemy attack {target.characterData.characterName} dmg {dmg}");

        yield return new WaitForSeconds(0.8f);
    }

    void UpdatePlayerDimStates(CharacterRuntime activeActor)
    {
        bool bertActive = (activeActor == bertRuntime);
        bool lieActive  = (activeActor == lieRuntime);
        bool johanActive= (activeActor == johanRuntime);

        if (bertRuntime != null) bertRuntime.SetDimmed(!bertActive);
        if (lieRuntime != null) lieRuntime.SetDimmed(!lieActive);
        if (johanRuntime != null) johanRuntime.SetDimmed(!johanActive);

        if (enemyRuntime != null)
        {
            // ensure enemy visible and not dimmed
            enemyRuntime.SetDimmed(false);
            if (!enemyRuntime.gameObject.activeSelf) enemyRuntime.gameObject.SetActive(true);
            if (enemyRuntime.rootCanvasGroup != null) enemyRuntime.rootCanvasGroup.alpha = 1f;
        }
    }

    bool IsAnyPlayerAlive()
    {
        return bertRuntime.IsAlive() || lieRuntime.IsAlive() || johanRuntime.IsAlive();
    }

    // UI callbacks
    public void OnAttackButton()
    {
        if (!waitingPlayerInput) return;
        pendingAction.type = ActionType.Attack;
        waitingPlayerInput = false;
    }
    public void OnDefendButton()
    {
        if (!waitingPlayerInput) return;
        pendingAction.type = ActionType.Defend;
        waitingPlayerInput = false;
    }
    public void OnSkillButton()
    {
        if (!waitingPlayerInput) return;
        CharacterRuntime active = GetActivePlayer();
        if (active == null) { pendingAction.type = ActionType.None; waitingPlayerInput = false; return; }

        if (active.characterData.skills == null || active.characterData.skills.Length == 0)
        {
            if (uiManager != null) uiManager.ShowSkillMissing();
            return;
        }

        int skillIndex = 0;
        if (!active.IsSkillAvailable(skillIndex))
        {
            if (uiManager != null) uiManager.ShowInfo("Skill sudah digunakan untuk hari ini");
            return;
        }

        SkillSO skill = active.characterData.skills[skillIndex];
        pendingAction.type = ActionType.Skill;
        pendingAction.skill = skill;
        pendingAction.skillIndex = skillIndex;
        waitingPlayerInput = false;
    }

    CharacterRuntime GetActivePlayer()
    {
        if (currentActor == bertRuntime) return bertRuntime;
        if (currentActor == lieRuntime) return lieRuntime;
        if (currentActor == johanRuntime) return johanRuntime;
        return null;
    }

    public void RestartDay()
    {
        Debug.Log($"Restarting day {currentDay}");
        bertRuntime.HealToFull();
        lieRuntime.HealToFull();
        johanRuntime.HealToFull();
        SetupDay(currentDay);
        StartCoroutine(CombatLoop());
    }

    void EnsureSceneReferences()
    {
        bertRuntime = FindSceneInstanceOrNull(bertRuntime, "bert") ?? bertRuntime;
        lieRuntime  = FindSceneInstanceOrNull(lieRuntime, "lie") ?? lieRuntime;
        johanRuntime= FindSceneInstanceOrNull(johanRuntime, "johan") ?? johanRuntime;

        var sceneEnemy = FindSceneInstanceOrNull(enemyRuntimePrefab, "monster");
        if (sceneEnemy != null) enemyRuntimePrefab = sceneEnemy;
    }

    CharacterRuntime FindSceneInstanceOrNull(CharacterRuntime assignedOrPrefab, string nameHint)
    {
        if (assignedOrPrefab != null && assignedOrPrefab.gameObject.scene.IsValid())
            return assignedOrPrefab;

        var all = FindObjectsOfType<CharacterRuntime>(true);
        if (assignedOrPrefab != null && assignedOrPrefab.characterData != null)
        {
            foreach (var r in all)
                if (r.characterData == assignedOrPrefab.characterData)
                    return r;
        }
        if (!string.IsNullOrEmpty(nameHint))
        {
            foreach (var r in all)
                if (r.name.ToLower().Contains(nameHint.ToLower()))
                    return r;
        }
        return null;
    }

    private void ShowVictoryPanel()
    {
        if (victoryPanel != null)
        {
            victoryPanel.SetActive(true);
        }
        
        // Disable combat controls
        if (uiManager != null)
        {
            uiManager.SetActionButtonsEnabled(false, false);
        }
    }

    // Dipanggil dari button "Lanjut" di victory panel
    public void OnVictoryContinuePressed()
    {
        if (GameProgressManager.Instance != null)
        {
            GameProgressManager.Instance.OnCombatComplete(); // Ini akan me-load day berikutnya
        }
    }

    private void OnCombatVictory()
    {
        if (GameProgressManager.Instance != null)
        {
            GameProgressManager.Instance.OnCombatComplete();
        }
    }
}
