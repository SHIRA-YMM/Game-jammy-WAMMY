using UnityEngine;
using System.Collections.Generic;
using DeathLine;
using UnityEngine.UI;

namespace DeathLine {
    public class CharacterRuntime : MonoBehaviour {
        public CharacterDataSO data;

        // runtime stats
        public int CurrentHP { get; private set; }
        public int MaxHP { get; private set; }
        public int CurrentMP { get; private set; }
        public int CurrentAtk { get; private set; }
        public int CurrentDef { get; private set; }
        public int Speed => data != null ? data.speed : 0;

        // equipment instances (serializable fields so you can inspect/save)
        [SerializeField] private List<EquipmentInstance> _equipment = new List<EquipmentInstance>(2);

        // cached sums to avoid recalculating every frame
        private int _equipAtkSum = 0;
        private int _equipDefSum = 0;
        private int _equipHpSum = 0;

        void Awake() {
            InitializeFromData();
        }

        public void InitializeFromData() {
            if (data == null) return;
            // create equipment instances from data startingEquipment if not already present
            if (_equipment == null) _equipment = new List<EquipmentInstance>(2);

            if (_equipment.Count == 0 && data.startingEquipment != null) {
                for (int i = 0; i < data.startingEquipment.Length; i++) {
                    var eq = data.startingEquipment[i];
                    if (eq != null) _equipment.Add(new EquipmentInstance(eq));
                }
            }

            RecalculateStats();
            CurrentHP = MaxHP;
            CurrentMP = data.baseMP;
        }

        private void RecalculateStats() {
            // base
            int baseHp = data.baseHP;
            int baseAtk = data.baseAtk;
            int baseDef = data.baseDef;

            // sum equipment bonuses (recompute when equipment changes)
            _equipAtkSum = 0; _equipDefSum = 0; _equipHpSum = 0;
            if (_equipment != null && _equipment.Count > 0) {
                for (int i = 0; i < _equipment.Count; i++) {
                    var inst = _equipment[i];
                    if (inst?.data == null) continue;
                    _equipAtkSum += inst.GetAtkBonus();
                    _equipDefSum += inst.GetDefBonus();
                    _equipHpSum += inst.GetHpBonus();
                }
            }

            MaxHP = Mathf.Max(1, baseHp + _equipHpSum);
            CurrentAtk = baseAtk + _equipAtkSum;
            CurrentDef = baseDef + _equipDefSum;
        }

        // expose equip/unequip/upgrade APIs
        public IReadOnlyList<EquipmentInstance> GetEquipmentInstances() => _equipment;

        public void Equip(EquipmentDataSO eqData) {
            if (eqData == null) return;
            _equipment.Add(new EquipmentInstance(eqData));
            RecalculateStats();
            UIEventBus.RaiseOnEquipmentChanged(this); // (you can implement this event)
        }

        public bool Unequip(EquipmentInstance instance) {
            if (instance == null) return false;
            bool removed = _equipment.Remove(instance);
            if (removed) {
                RecalculateStats();
                UIEventBus.RaiseOnEquipmentChanged(this);
            }
            return removed;
        }

        public bool TryUpgradeEquipment(EquipmentInstance instance, out int cost) {
            cost = 0;
            if (instance == null || instance.data == null) return false;
            if (!instance.CanUpgrade(out cost)) return false;

            // Here decide who pays cost (ShopManager / Player inventory)
            // We just perform upgrade; caller should check player's gold and deduct before calling
            bool ok = instance.Upgrade();
            if (ok) {
                RecalculateStats();
                UIEventBus.RaiseOnEquipmentChanged(this);
            }
            return ok;
        }
    }
}
