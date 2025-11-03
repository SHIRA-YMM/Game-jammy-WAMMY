// EffectResult.cs
using System;
using System.Collections.Generic;
using UnityEngine;

namespace DeathLine {
    [Serializable]
    public struct EffectTargetResult {
        public string targetId; // runtime will fill char id
        public int hpDelta;     // negative for damage, positive for heal
        public int mpDelta;
        public string note;     // e.g. "Critical", "Blocked"
    }

    [Serializable]
    public struct EffectResult {
        public string effectId; // source effect id (optional)
        public List<EffectTargetResult> targetResults;

        public void EnsureList() {
            if (targetResults == null) targetResults = new List<EffectTargetResult>(2);
        }
    }
}
