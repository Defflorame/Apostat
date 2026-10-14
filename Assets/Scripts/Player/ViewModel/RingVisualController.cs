using System;
using System.Collections.Generic;
using UnityEngine;
using Game.Items;
using Game.Player.Equipment;

namespace Game.Player.ViewModel
{
    [Serializable]
    public struct RingSlot
    {
        public HandVisual hand;
        public FingerType finger;
    }

    /// <summary>
    /// Показывает надетые кольца на пальцах. Только отображает: читает
    /// PlayerEquipment.EquippedRings и ставит RingData.RingPrefab на метки
    /// FingerSocket. N-е надетое кольцо идёт в N-й слот списка slots
    /// (порядок и пальцы задаются в инспекторе, а не в коде).
    /// </summary>
    public class RingVisualController : MonoBehaviour
    {
        [SerializeField] private PlayerEquipment playerEquipment;
        [Tooltip("Слоты по порядку: 1-е надетое кольцо → slots[0], 2-е → slots[1] и т.д.")]
        [SerializeField] private RingSlot[] slots;

        private readonly List<GameObject> _spawned = new List<GameObject>();

        private void OnEnable()
        {
            playerEquipment.OnEquipmentChanged += Refresh;

            foreach (RingSlot slot in slots)
            {
                if (slot.hand != null) slot.hand.OnModelRebuilt += Refresh;
            }
        }

        private void OnDisable()
        {
            playerEquipment.OnEquipmentChanged -= Refresh;

            foreach (RingSlot slot in slots)
            {
                if (slot.hand != null) slot.hand.OnModelRebuilt -= Refresh;
            }
        }

        private void Start()
        {
            Refresh();
        }

        private void Refresh()
        {
            foreach (GameObject ring in _spawned)
            {
                if (ring != null) Destroy(ring);
            }
            _spawned.Clear();

            IReadOnlyList<ItemInstance> rings = playerEquipment.EquippedRings;

            if (rings.Count > slots.Length)
            {
                Debug.LogWarning($"RingVisualController: надето {rings.Count} колец, а слотов только {slots.Length}.", this);
            }

            for (int i = 0; i < rings.Count && i < slots.Length; i++)
            {
                GameObject prefab = (rings[i].Data as RingData)?.RingPrefab;
                if (prefab == null) continue;

                RingSlot slot = slots[i];
                FingerSocket socket = slot.hand != null ? slot.hand.FindFinger(slot.finger) : null;

                // Модель руки может быть ещё не создана — HandVisual.OnModelRebuilt вызовет Refresh позже.
                if (socket == null) continue;

                GameObject instance = Instantiate(prefab, socket.transform);
                instance.transform.localPosition = Vector3.zero;
                instance.transform.localRotation = Quaternion.identity;
                _spawned.Add(instance);
            }
        }
    }
}