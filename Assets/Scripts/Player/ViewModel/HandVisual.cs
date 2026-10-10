using UnityEngine;

namespace Game.Player.ViewModel
{
    /// <summary>
    /// Внешний вид одной руки: голая (defaultHandPrefab) или в перчатке.
    /// Модель руки можно временно перенести на другую точку (например,
    /// на Secondary-хват двуручного оружия) через SetModelAnchor.
    ///
    /// Положение и поворот корня префаба руки игнорируются: поза определяется
    /// только контейнером HandModel или хватом. Масштаб префаба не меняется,
    /// зеркалирование делается промежуточным объектом HandMirror.
    /// Не использует Awake — методы можно вызывать и на выключенной руке.
    /// </summary>
    public class HandVisual : MonoBehaviour
    {
        [Tooltip("Контейнер HandModel — родитель модели руки по умолчанию.")]
        [SerializeField] private Transform modelParent;
        [Tooltip("Голая рука. Должна быть prefab-ассетом из окна Project, не объектом сцены.")]
        [SerializeField] private GameObject defaultHandPrefab;
        [Tooltip("Включить для левой руки, если модель одна и та же для обеих рук.")]
        [SerializeField] private bool mirrorX;

        private GameObject _glovePrefab;
        private Transform _anchorOverride;
        private Transform _mirror;
        private GameObject _instance;
        private GameObject _instanceSource;

        /// <summary>null — вернуть голую руку.</summary>
        public void SetGlove(GameObject handPrefab)
        {
            _glovePrefab = handPrefab;
            Rebuild();
        }

        /// <summary>Перенести модель руки на точку (например, Secondary-хват меча). null — вернуть на место.</summary>
        public void SetModelAnchor(Transform anchor)
        {
            _anchorOverride = anchor;
            Rebuild();
        }

        public void SetVisible(bool visible)
        {
            gameObject.SetActive(visible);
        }

        private void Rebuild()
        {
            GameObject wanted = _glovePrefab != null ? _glovePrefab : defaultHandPrefab;
            Transform parent = _anchorOverride != null ? _anchorOverride : modelParent;

            if (wanted == null || parent == null)
            {
                Debug.LogWarning("HandVisual: не назначен defaultHandPrefab или modelParent.", this);
                DestroyInstance();
                return;
            }

            if (_mirror == null)
            {
                _mirror = new GameObject("HandMirror").transform;
            }

            _mirror.SetParent(parent, false);
            _mirror.localPosition = Vector3.zero;
            _mirror.localRotation = Quaternion.identity;
            _mirror.localScale = mirrorX ? new Vector3(-1f, 1f, 1f) : Vector3.one;

            if (_instance == null || _instanceSource != wanted)
            {
                DestroyInstance();
                _instance = Instantiate(wanted, _mirror); // масштаб префаба сохраняется как есть
                _instance.SetActive(true);
                _instanceSource = wanted;
            }

            _instance.transform.localPosition = Vector3.zero;
            _instance.transform.localRotation = Quaternion.identity;
        }

        private void DestroyInstance()
        {
            if (_instance != null)
            {
                Destroy(_instance);
            }

            _instance = null;
            _instanceSource = null;
        }
    }
}