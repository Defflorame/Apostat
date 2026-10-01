using System.Collections.Generic;
using UnityEngine;
using Game.Player;

namespace Game.Debugging
{
    /// <summary>
    /// ВРЕМЕННЫЙ инструмент. Не часть архитектуры — удалить, когда появится
    /// реальный debug/dev overlay или он станет не нужен.
    ///
    /// Только читает PlayerActionStateMachine.CurrentState (публичный
    /// geттер уже существует) — ничего не меняет в боевой логике.
    /// Показывает в правом верхнем углу текущее состояние и последние
    /// несколько переходов между состояниями с таймингом между ними.
    /// </summary>
    public class PlayerStateDebugOverlay : MonoBehaviour
    {
        [SerializeField] private PlayerActionStateMachine stateMachine;

        [Tooltip("Сколько последних переходов хранить и показывать.")]
        [SerializeField] private int historySize = 8;

        private readonly List<string> _history = new List<string>();
        private string _lastStateName;
        private float _lastChangeTime;

        private void Update()
        {
            if (stateMachine == null) return;

            string currentName = stateMachine.CurrentState?.GetType().Name ?? "None";
            if (currentName == _lastStateName) return;

            if (_lastStateName != null)
            {
                float elapsed = Time.time - _lastChangeTime;
                _history.Add($"[{Time.time:F2}] {_lastStateName} → {currentName}  ({elapsed:F2}s)");

                if (_history.Count > historySize)
                {
                    _history.RemoveAt(0);
                }
            }

            _lastStateName = currentName;
            _lastChangeTime = Time.time;
        }

        private void OnGUI()
        {
            if (stateMachine == null) return;

            const int width = 320;
            const int padding = 10;

            GUIStyle currentStyle = new GUIStyle(GUI.skin.label) { fontSize = 18, fontStyle = FontStyle.Bold };
            currentStyle.normal.textColor = Color.white;

            GUIStyle historyStyle = new GUIStyle(GUI.skin.label) { fontSize = 13 };
            historyStyle.normal.textColor = new Color(0.8f, 0.8f, 0.8f);

            int height = 40 + _history.Count * 18 + 20;

            GUI.Box(new Rect(Screen.width - width - padding, padding, width, height), "");
            GUILayout.BeginArea(new Rect(Screen.width - width - padding + 10, padding + 8, width - 20, height - 16));

            GUILayout.Label($"State: {_lastStateName ?? "None"}", currentStyle);
            GUILayout.Space(6);

            for (int i = _history.Count - 1; i >= 0; i--)
            {
                GUILayout.Label(_history[i], historyStyle);
            }

            GUILayout.EndArea();
        }
    }
}