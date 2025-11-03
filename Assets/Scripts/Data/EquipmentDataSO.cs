using UnityEngine;
using System;

namespace DeathLine {
    [CreateAssetMenu(menuName = "DeathLine/Data/Equipment", fileName = "Equipment_")]
    public class EquipmentDataSO : ScriptableObject {
        [Header("Identity")]
        public string equipmentId;
        public string displayName;
        public Sprite icon;

        [Header("Base stats (level 1)")]
        public int baseAtk = 0;
        public int baseDef = 0;
        public int baseHp = 0;

        [Header("Leveling")]
        [Tooltip("Max level for this equipment")]
        public int maxLevel = 5;

        [Tooltip("If empty, leveling won't change stats. Each step index i corresponds to level i+1")]
        public EquipmentUpgradeStep[] upgradeSteps = new EquipmentUpgradeStep[0];

        [Header("Designer notes")]
        [TextArea(2,4)] public string notes;
    }

    [Serializable]
    public struct EquipmentUpgradeStep {
        [Tooltip("Level number this step represents (informational)")]
        public int level; // usually 2..maxLevel
        public int costGold;
        public int addAtk;
        public int addDef;
        public int addHp;
        [TextArea(1,2)] public string note;
    }
}
