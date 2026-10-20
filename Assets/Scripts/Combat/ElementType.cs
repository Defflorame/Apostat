using UnityEngine;

namespace Game.Combat
{
    /// <summary>
    /// Стихия как ассет, а не enum: новые стихии добавляются созданием ассета
    /// (Create → Game/Combat/Element Type), переименовываются в инспекторе и
    /// не требуют правки кода. Сравнение — по ссылке на ассет, поэтому
    /// переименование displayName ничего не ломает.
    /// null в поле "стихия" = нет стихии (нейтральный урон).
    /// </summary>
    [CreateAssetMenu(fileName = "NewElementType", menuName = "Game/Combat/Element Type")]
    public class ElementType : ScriptableObject
    {
        [Tooltip("Название для UI и логов. На логику не влияет.")]
        public string displayName;
    }
}
