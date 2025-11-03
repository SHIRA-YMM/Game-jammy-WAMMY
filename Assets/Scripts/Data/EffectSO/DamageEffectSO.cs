// DamageEffectSO.cs
using UnityEngine;

namespace DeathLine {
    [CreateAssetMenu(menuName = "DeathLine/Effects/Damage", fileName = "Effect_Damage_")]
    public class DamageEffectSO : EffectSO {
        public int flatDamage = 0;
        public float scalingAtk = 1f; // damage = flatDamage + scalingAtk * sourceAtk
        public bool isPhysical = true;
        public bool canCrit = true;
        [Tooltip("Minimum damage (after defense)")]
        public int minDamage = 1;

        public override EffectResult Execute(EffectExecutionContext ctx) {
            var result = new EffectResult { effectId = effectId };
            result.EnsureList();

            if (ctx.targetIds == null || ctx.targetIds.Length == 0) return result;

            for (int i = 0; i < ctx.targetIds.Length; i++) {
                var tid = ctx.targetIds[i];
                // We do not have direct access to CharacterRuntime here in SO; runtime will interpret hpDelta.
                int calc = Mathf.FloorToInt(flatDamage + scalingAtk * ctx.sourceAtk);
                // runtime will apply target defense and flags; we output preliminary hpDelta as negative value
                int hpDelta = -Mathf.Max(minDamage, calc); // negative means damage
                var tr = new EffectTargetResult {
                    targetId = tid,
                    hpDelta = hpDelta,
                    mpDelta = 0,
                    note = isPhysical ? "Physical" : "Magic"
                };
                result.targetResults.Add(tr);
            }
            return result;
        }
    }
}
