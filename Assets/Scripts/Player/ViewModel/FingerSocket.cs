using UnityEngine;

namespace Game.Player.ViewModel
{
    public enum FingerType
    {
        Thumb,
        Index,
        Middle,
        Ring,
        Pinky
    }

    /// <summary>
    /// Метка пальца на префабе руки (голой или в перчатке). Только данные:
    /// положение и оси, в которых ставится кольцо. Ставьте на пустой
    /// дочерний объект пальца в том месте, где должно сидеть кольцо.
    /// </summary>
    public class FingerSocket : MonoBehaviour
    {
        [SerializeField] private FingerType finger = FingerType.Index;

        public FingerType Finger => finger;

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, 0.01f);
        }
    }
}