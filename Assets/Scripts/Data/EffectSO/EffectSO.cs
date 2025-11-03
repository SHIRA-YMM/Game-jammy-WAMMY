// EffectSO.cs
using UnityEngine;

namespace DeathLine {
    public abstract class EffectSO : ScriptableObject {
        public string effectId;
        public string effectName;
        [TextArea(1,3)] public string designerNotes;

        public abstract EffectResult Execute(EffectExecutionContext ctx);
    }

    
    public struct EffectExecutionContext {
        public string sourceId;
        public int sourceAtk;
        public int sourceDef;
        public int sourceLevel;
        public string[] targetIds; // targets resolved by SkillExecutor/CombatManager
        public CombatContext combatContext; // optional read-only snapshot
    }

    // read-only snapshot (keep minimal)
    public struct CombatContext {
        public int currentRound;
        public string[] allyIds;
        public string[] enemyIds;
    }
}
