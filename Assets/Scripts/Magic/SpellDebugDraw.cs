using UnityEngine;

namespace Game.Magic
{
    /// <summary>
    /// Отладочная отрисовка для магии: сфера из трёх окружностей через Debug.DrawLine.
    /// В отличие от Gizmos, линии живут заданное время и после уничтожения объекта
    /// (например, снаряда после взрыва). Виден в Scene всегда, в Game — при включённых Gizmos.
    /// Conditional: в билде вызовы метода вырезаются компилятором целиком.
    /// </summary>
    public static class SpellDebugDraw
    {
        private const int Segments = 24;

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void Sphere(Vector3 center, float radius, Color color, float duration)
        {
            Circle(center, Vector3.right, Vector3.up, radius, color, duration);
            Circle(center, Vector3.right, Vector3.forward, radius, color, duration);
            Circle(center, Vector3.up, Vector3.forward, radius, color, duration);
        }

        private static void Circle(Vector3 center, Vector3 axisA, Vector3 axisB, float radius, Color color, float duration)
        {
            Vector3 previous = center + axisA * radius;

            for (int i = 1; i <= Segments; i++)
            {
                float angle = i * Mathf.PI * 2f / Segments;
                Vector3 next = center + (axisA * Mathf.Cos(angle) + axisB * Mathf.Sin(angle)) * radius;
                Debug.DrawLine(previous, next, color, duration);
                previous = next;
            }
        }
    }
}
