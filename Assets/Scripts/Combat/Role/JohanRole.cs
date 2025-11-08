using UnityEngine;

public class JohanRole : MonoBehaviour, IRoleBehavior
{
    public int GetMaxHealth() => 60;
    public int GetAttackPower() => 35;
}