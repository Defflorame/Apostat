using UnityEngine;

namespace Game.Player.ViewModel
{
    public enum GripType
    {
        /// <summary>Точка, которой несущая рука держит предмет. Совмещается с ItemSocket руки.</summary>
        Primary,

        /// <summary>Точка хвата второй руки у двуручного оружия. На неё встаёт модель левой руки.</summary>
        Secondary
    }

    /// <summary>
    /// Метка хвата на префабе оружия/щита. Это только данные о позиции и
    /// осях хвата, без логики. Ставьте метки прямыми детьми корня префаба.
    /// </summary>
    public class ItemGrip : MonoBehaviour
    {
        [SerializeField] private GripType type = GripType.Primary;

        public GripType Type => type;

        private void OnDrawGizmos()
        {
            Gizmos.color = type == GripType.Primary ? Color.green : Color.cyan;
            Gizmos.DrawWireSphere(transform.position, 0.03f);

            const float axisLength = 0.1f;
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position + transform.right * axisLength);
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, transform.position + transform.up * axisLength);
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, transform.position + transform.forward * axisLength);
        }
    }
}