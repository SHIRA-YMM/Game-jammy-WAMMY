using UnityEngine;

public class BertRole : MonoBehaviour, IRoleBehavior
{
    public int GetMaxHealth() => 100;
    public int GetAttackPower() => 20;
}