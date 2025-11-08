using UnityEngine;

public abstract class BattleCharacter : MonoBehaviour
{
    [Header("Character Info")]
    public string characterName;

    [Header("Stats")]
    public int maxHealth;
    public int currentHealth;
    public int attackPower;

    [Header("Team")]
    public bool isPlayerTeam;

    [Header("UI References")]
    public HealthBar healthBar;

    // This will link to the role behavior script (BertRole, JohanRole, LieRole)
    private IRoleBehavior roleBehavior;

    public virtual void Initialize()
    {
        roleBehavior = GetComponent<IRoleBehavior>();

        if (roleBehavior != null)
        {
            maxHealth = roleBehavior.GetMaxHealth();
            attackPower = roleBehavior.GetAttackPower();
        }
        else
        {
            Debug.LogWarning($"{characterName} has no role behavior attached!");
        }

        currentHealth = maxHealth;

        if (healthBar != null)
            healthBar.SetMaxHealth(maxHealth);

        Debug.Log($"{characterName} initialized: {maxHealth} HP, {attackPower} ATK.");
    }

    public virtual void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Max(0, currentHealth);

        Debug.Log($"{characterName} took {damage} damage! HP: {currentHealth}/{maxHealth}");

        if (healthBar != null)
            healthBar.SetHealth(currentHealth);

        if (currentHealth <= 0)
            Die();
    }

    public virtual void Attack(BattleCharacter target)
    {
        Debug.Log($"{characterName} attacks {target.characterName} for {attackPower} damage!");
        target.TakeDamage(attackPower);
    }

    protected virtual void Die()
    {
        Debug.Log($"{characterName} has been defeated!");
        gameObject.SetActive(false);
    }

    public bool IsAlive()
    {
        return currentHealth > 0;
    }
}