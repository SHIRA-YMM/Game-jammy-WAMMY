using UnityEngine;

namespace DeathLine {
    [CreateAssetMenu(menuName = "DeathLine/Data/CharacterData", fileName = "CharacterData_")]
    public class CharacterDataSO : ScriptableObject {
        [Header("Identity")]
        public string characterId; // unique id (used by runtime / save)
        public string displayName;
        public Sprite portrait;
        [Tooltip("Main role for balancing and AI hints")]
        public RoleType role = RoleType.DPS;
        [Tooltip("Default affinity (affects skills or passive)")]
        public ColourAffinity defaultAffinity = ColourAffinity.Neutral;

        [Header("Base Stats")]
        public int baseHP = 100;
        public int baseMP = 30;
        public int baseAtk = 10;
        public int baseDef = 5;
        public int speed = 10; // used for turn order

        [Header("Starting Loadout")]
        public SkillSO[] startingSkills = new SkillSO[0];
        public EquipmentDataSO[] startingEquipment = new EquipmentDataSO[0]; // optional; define if you have equipment SO

        [Header("Runtime Hints")]
        [Tooltip("Should this character be controllable during daytime? (Johan yes, others no)")]
        public bool controllableDuringDay = false;

        // NOTE: keep this class purely data; all runtime values are in CharacterRuntime.
    }
}
