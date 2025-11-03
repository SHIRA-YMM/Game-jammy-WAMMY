using System;
using UnityEngine;

namespace DeathLine {
    [Serializable]
    public class EquipmentInstance {
        public string instanceId; // optional unique id for persistence
        public EquipmentDataSO data;
        public int currentLevel;

        public EquipmentInstance() { }

        public EquipmentInstance(EquipmentDataSO dataSO) {
            data = dataSO;
            currentLevel = 1;
            instanceId = System.Guid.NewGuid().ToString();
        }

        public int GetAtkBonus() {
            if (data == null) return 0;
            int atk = data.baseAtk;
            if (data.upgradeSteps != null && data.upgradeSteps.Length > 0) {
                for (int i = 0; i < data.upgradeSteps.Length; i++) {
                    var step = data.upgradeSteps[i];
                    if (step.level <= currentLevel) atk += step.addAtk;
                }
            }
            return atk;
        }

        public int GetDefBonus() {
            if (data == null) return 0;
            int def = data.baseDef;
            if (data.upgradeSteps != null && data.upgradeSteps.Length > 0) {
                for (int i = 0; i < data.upgradeSteps.Length; i++) {
                    var step = data.upgradeSteps[i];
                    if (step.level <= currentLevel) def += step.addDef;
                }
            }
            return def;
        }

        public int GetHpBonus() {
            if (data == null) return 0;
            int hp = data.baseHp;
            if (data.upgradeSteps != null && data.upgradeSteps.Length > 0) {
                for (int i = 0; i < data.upgradeSteps.Length; i++) {
                    var step = data.upgradeSteps[i];
                    if (step.level <= currentLevel) hp += step.addHp;
                }
            }
            return hp;
        }

        public bool CanUpgrade(out int nextCost) {
            nextCost = 0;
            if (data == null) return false;
            if (currentLevel >= data.maxLevel) return false;
            int nextLevel = currentLevel + 1;
            if (data.upgradeSteps == null || data.upgradeSteps.Length == 0) {
                // if no steps, disallow
                return false;
            }
            // find step with level == nextLevel or fallback to index nextLevel-2
            for (int i = 0; i < data.upgradeSteps.Length; i++) {
                if (data.upgradeSteps[i].level == nextLevel) {
                    nextCost = data.upgradeSteps[i].costGold;
                    return true;
                }
            }
            // fallback: try index
            int idx = nextLevel - 2;
            if (idx >= 0 && idx < data.upgradeSteps.Length) {
                nextCost = data.upgradeSteps[idx].costGold;
                return true;
            }
            return false;
        }

        public bool Upgrade() {
            if (data == null) return false;
            if (currentLevel >= data.maxLevel) return false;
            currentLevel++;
            return true;
        }
    }
}
