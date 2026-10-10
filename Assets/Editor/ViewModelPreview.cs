using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Game.Player.ViewModel;

namespace Game.EditorTools
{
    /// <summary>
    /// Только для анимирования в Edit Mode. Ставит в сцене временные копии
    /// оружия/руки, выровненные так же, как в игре. ОБЯЗАТЕЛЬНО удалить через
    /// Remove All Previews перед запуском Play Mode, иначе в игре будет два меча.
    /// 
    /// Превью. Выделите RightHand в Hierarchy, затем с Ctrl добавьте в выделение
    ///  из Project префаб одноручного меча и префаб голой руки. Выберите Game → Animations → Add Preview To Selected Hand.
    ///  Рука и меч встанут в сцену в той же позе, что и в игре.
    /// </summary>
    public static class ViewModelPreview
    {
        private const string PreviewPrefix = "__Preview_";

        [MenuItem("Game/Animations/Add Preview To Selected Hand")]
        public static void AddPreview()
        {
            // Выделить: руку в Hierarchy (с HandVisual) + префаб(ы) оружия/руки в Project (Ctrl+клик).
            HandVisual hand = null;
            var prefabs = new List<GameObject>();

            foreach (Object obj in Selection.objects)
            {
                if (!(obj is GameObject go)) continue;

                if (go.scene.IsValid())
                {
                    HandVisual found = go.GetComponent<HandVisual>();
                    if (found != null) hand = found;
                }
                else
                {
                    prefabs.Add(go);
                }
            }

            if (hand == null || prefabs.Count == 0)
            {
                Debug.LogWarning("ViewModelPreview: выделите руку в сцене (с HandVisual) и префаб оружия/руки в Project (Ctrl+клик).");
                return;
            }

            HeldItemSocket socket = hand.GetComponentInChildren<HeldItemSocket>(true);
            var handSerialized = new SerializedObject(hand);
            Transform modelParent = handSerialized.FindProperty("modelParent").objectReferenceValue as Transform;

            foreach (GameObject prefab in prefabs)
            {
                bool isWeapon = FindPrimaryGrip(prefab) != null;

                if (isWeapon && socket != null)
                {
                    GameObject instance = Spawn(prefab, socket.transform);
                    ItemGrip primary = FindPrimaryGrip(instance);
                    HeldItemSocket.AlignToGrip(instance.transform, primary.transform);
                }
                else if (!isWeapon && modelParent != null)
                {
                    GameObject instance = Spawn(prefab, modelParent);
                    instance.transform.localPosition = Vector3.zero;
                    instance.transform.localRotation = Quaternion.identity;
                }
                else
                {
                    Debug.LogWarning($"ViewModelPreview: не удалось поставить превью '{prefab.name}' (нет сокета или modelParent).");
                }
            }
        }

        [MenuItem("Game/Animations/Remove All Previews")]
        public static void RemoveAll()
        {
            var toDelete = new List<GameObject>();

            foreach (Transform t in Object.FindObjectsByType<Transform>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            {
                if (t.name.StartsWith(PreviewPrefix)) toDelete.Add(t.gameObject);
            }

            foreach (GameObject go in toDelete)
            {
                Undo.DestroyObjectImmediate(go);
            }

            Debug.Log($"ViewModelPreview: удалено превью: {toDelete.Count}");
        }

        private static GameObject Spawn(GameObject prefab, Transform parent)
        {
            var instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab, parent);
            Undo.RegisterCreatedObjectUndo(instance, "Add Preview");
            instance.name = PreviewPrefix + prefab.name;
            return instance;
        }

        private static ItemGrip FindPrimaryGrip(GameObject root)
        {
            foreach (ItemGrip grip in root.GetComponentsInChildren<ItemGrip>(true))
            {
                if (grip.Type == GripType.Primary) return grip;
            }

            return null;
        }
    }
}