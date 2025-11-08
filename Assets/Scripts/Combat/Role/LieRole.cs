using UnityEngine;

public class LieRole : MonoBehaviour, IRoleBehavior
{
    public int GetMaxHealth() => 80;
    public int GetAttackPower() => 15;
}
