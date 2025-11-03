using UnityEngine;
using UnityEngine.Events;

namespace DeathLine {
    public static class UIEventBus {
        // Event for equipment changes
        public static UnityEvent<CharacterRuntime> OnEquipmentChanged = new UnityEvent<CharacterRuntime>();

        // Static method to raise the event
        public static void RaiseOnEquipmentChanged(CharacterRuntime character) {
            OnEquipmentChanged?.Invoke(character);
        }

        // Add any other UI events here as needed
    }
}