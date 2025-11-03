using UnityEngine;

namespace DeathLine {
    [CreateAssetMenu(menuName = "DeathLine/Data/Skill", fileName = "Skill_")]
    public class SkillSO : ScriptableObject {
        [Header("Core")]
        public string skillId;
        public string skillName;
        [TextArea(2,4)] public string description;
        public Sprite icon;

        [Header("Cost & Priority")]
        public int mpCost = 0;
        public int apCost = 0; // if you use Action Points
        [Tooltip("Higher priority resolves earlier if same turn")]
        public int priority = 0;

        [Header("Targeting")]
        public TargetType targetType = TargetType.SingleEnemy;
        [Tooltip("Should UI ask for target even if single?")]
        public bool requireManualTargetSelection = true;

        [Header("Effects Pipeline")]
        [Tooltip("Ordered list of effect scriptable objects to run when skill is executed")]
        public EffectSO[] effects = new EffectSO[0];

        [Header("Tuning")]
        public bool refundableOnInterrupt = false;

        // SkillSO is data-only. Execution is performed by SkillExecutor at runtime
    }
}

