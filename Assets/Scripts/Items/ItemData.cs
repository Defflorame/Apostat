using UnityEngine;

namespace Game.Items
{
    /// <summary>
    /// Общая база данных предмета: то, чем предмет ЯВЛЯЕТСЯ (не хранит runtime state
    /// конкретного экземпляра — это делает ItemInstance).
    /// </summary>
    public abstract class ItemData : ScriptableObject
    {
        // Стабильный числовой ID для сохранений. Вручную не задаётся и в игровой
        // логике не сравнивается (для этого есть ссылка на ItemData): в Editor он
        // выводится из GUID ассета, поэтому не меняется при переименовании/перемещении
        // и у скопированного ассета автоматически становится новым.
        [SerializeField, HideInInspector] private int id;

        public int Id => id;

        public string itemName;
        [TextArea] public string description;

        [Tooltip("Вес предмета — учитывается EquipmentModifierService при подсчёте нагрузки экипировки.")]
        public float weight = 0f;

        [Tooltip("Максимальный размер стака в инвентаре. 1 — предмет не стакуется (оружие, броня и т.д.).")]
        public int maxStackSize = 1;

        /// <summary>Наследники, которым нужен свой OnValidate, переопределяют этот метод и вызывают base.OnValidate().</summary>
        protected virtual void OnValidate()
        {
#if UNITY_EDITOR
            string path = UnityEditor.AssetDatabase.GetAssetPath(this);
            if (string.IsNullOrEmpty(path)) return;

            string guid = UnityEditor.AssetDatabase.AssetPathToGUID(path);
            if (guid.Length < 8) return;

            int expected = System.Convert.ToInt32(guid.Substring(0, 8), 16);
            if (expected == 0) expected = 1;

            if (id != expected)
            {
                id = expected;
                UnityEditor.EditorUtility.SetDirty(this);
            }
#endif
        }
    }
}
