using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public List<BattleCharacter> playerTeam;
    public List<BattleCharacter> enemyTeam;

    private bool playerTurn = true;

    void Start()
    {
        foreach (var p in playerTeam) p.Initialize();
        foreach (var e in enemyTeam) e.Initialize();

        StartCoroutine(BattleLoop());
    }

    private IEnumerator BattleLoop()
    {
        while (true)
        {
            if (playerTurn)
            {
                Debug.Log("=== Player Turn ===");
                foreach (var player in playerTeam)
                {
                    if (player.IsAlive())
                    {
                        var target = enemyTeam.Find(e => e.IsAlive());
                        if (target != null)
                        {
                            player.Attack(target);
                            yield return new WaitForSeconds(1f);
                        }
                    }
                }
                playerTurn = false;
            }
            else
            {
                Debug.Log("=== Enemy Turn ===");
                foreach (var enemy in enemyTeam)
                {
                    if (enemy.IsAlive())
                    {
                        var target = playerTeam.Find(p => p.IsAlive());
                        if (target != null)
                        {
                            enemy.Attack(target);
                            yield return new WaitForSeconds(1f);
                        }
                    }
                }
                playerTurn = true;
            }

            yield return new WaitForSeconds(1.5f);

            if (AllDead(playerTeam))
            {
                Debug.Log("Enemies win!");
                break;
            }
            if (AllDead(enemyTeam))
            {
                Debug.Log("Players win!");
                break;
            }
        }
    }

    private bool AllDead(List<BattleCharacter> team)
    {
        foreach (var c in team)
        {
            if (c.IsAlive()) return false;
        }
        return true;
    }
}
