using UnityEngine;

namespace Game.Player.ViewModel
{
    /// <summary>
    /// Ставит и убирает префаб предмета в руке. Если у предмета есть
    /// ItemGrip типа Primary, предмет выравнивается так, чтобы эта метка
    /// совпала с позицией и осями сокета. Без метки используется поза
    /// из самого префаба.
    /// </summary>
    public class HeldItemSocket : MonoBehaviour
    {
        private GameObject _currentPrefab;
        private GameObject _instance;

        /// <summary>Хват второй руки на текущем предмете (null, если у предмета его нет).</summary>
        public ItemGrip SecondaryGrip { get; private set; }

        public void Attach(GameObject prefab)
        {
            if (prefab == _currentPrefab && (prefab == null || _instance != null)) return;

            Clear();
            if (prefab == null) return;

            _instance = Instantiate(prefab, transform);
            _currentPrefab = prefab;

            ItemGrip primary = null;
            foreach (ItemGrip grip in _instance.GetComponentsInChildren<ItemGrip>())
            {
                if (grip.Type == GripType.Primary) primary = grip;
                else SecondaryGrip = grip;
            }

            if (primary != null)
            {
                AlignToGrip(_instance.transform, primary.transform);
            }
        }

        public void Clear()
        {
            if (_instance != null)
            {
                Destroy(_instance);
            }

            _instance = null;
            _currentPrefab = null;
            SecondaryGrip = null;
        }

        /// <summary>
        /// Двигает корень предмета так, чтобы grip оказался в нулевой позе сокета.
        /// Public static — тот же расчёт использует Editor-превью.
        /// </summary>
        public static void AlignToGrip(Transform root, Transform grip)
        {
            Vector3 gripLocalPos = root.InverseTransformPoint(grip.position);
            Quaternion gripLocalRot = Quaternion.Inverse(root.rotation) * grip.rotation;

            root.localRotation = Quaternion.Inverse(gripLocalRot);
            root.localPosition = -(root.localRotation * Vector3.Scale(gripLocalPos, root.localScale));
        }
    }
}