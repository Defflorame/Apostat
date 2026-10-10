using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using Game.Player.ViewModel;

namespace Game.EditorTools
{
    /// <summary>
    /// Создаёт RightHand_Base.controller / LeftHand_Base.controller и заготовки клипов.
    /// Запускать один раз. Существующие ассеты не перезаписывает.
    /// Если перед запуском выделить в сцене RightHand/LeftHand, клипы получат
    /// ключи с её текущей позой — рука не прыгает в (0,0,0). Без выделения клипы пустые.
    /// </summary>
    public static class HandAnimatorBuilder
    {
        private const string RootFolder = "Assets/Animations/ViewModel";

        [MenuItem("Game/Animations/Create Right Hand Controller")]
        public static void BuildRight()
        {
            string controllerPath = $"{RootFolder}/RightHand_Base.controller";
            if (!CanCreate(controllerPath)) return;

            string folder = EnsureFolder("Right");
            Transform pose = Selection.activeTransform;

            AnimationClip idle = GetOrCreateClip(folder, PlayerAnimatorIds.IdleClip, 1f, true, pose);
            AnimationClip light = GetOrCreateClip(folder, PlayerAnimatorIds.LightAttackClip, 0.5f, false, pose);
            AnimationClip charge = GetOrCreateClip(folder, PlayerAnimatorIds.ChargeHoldClip, 1f, true, pose);
            AnimationClip heavy = GetOrCreateClip(folder, PlayerAnimatorIds.HeavyAttackClip, 0.7f, false, pose);
            AnimationClip dodge = GetOrCreateClip(folder, PlayerAnimatorIds.DodgeClip, 0.3f, false, pose);
            AnimationClip parry = GetOrCreateClip(folder, PlayerAnimatorIds.ParryClip, 0.5f, false, pose);

            AnimatorController controller = AnimatorController.CreateAnimatorControllerAtPath(controllerPath);

            controller.AddParameter(PlayerAnimatorIds.IsLightAttackingName, AnimatorControllerParameterType.Bool);
            controller.AddParameter(PlayerAnimatorIds.IsHeavyAttackingName, AnimatorControllerParameterType.Bool);
            controller.AddParameter(PlayerAnimatorIds.IsChargingName, AnimatorControllerParameterType.Bool);
            controller.AddParameter(PlayerAnimatorIds.IsDodgingName, AnimatorControllerParameterType.Bool);
            controller.AddParameter(PlayerAnimatorIds.IsParryingName, AnimatorControllerParameterType.Bool);
            controller.AddParameter(new AnimatorControllerParameter
            {
                name = PlayerAnimatorIds.AttackSpeedName,
                type = AnimatorControllerParameterType.Float,
                defaultFloat = 1f
            });

            AnimatorStateMachine sm = controller.layers[0].stateMachine;

            AnimatorState idleState = AddState(sm, "Idle", idle);
            sm.defaultState = idleState;
            AnimatorState lightState = AddState(sm, "LightAttack", light);
            AnimatorState chargeState = AddState(sm, "ChargeHold", charge);
            AnimatorState heavyState = AddState(sm, "HeavyAttack", heavy);
            AnimatorState dodgeState = AddState(sm, "Dodge", dodge);
            AnimatorState parryState = AddState(sm, "Parry", parry);

            // Скорость атак подстраивается под AttackData через параметр.
            lightState.speedParameterActive = true;
            lightState.speedParameter = PlayerAnimatorIds.AttackSpeedName;
            heavyState.speedParameterActive = true;
            heavyState.speedParameter = PlayerAnimatorIds.AttackSpeedName;

            // Light
            Link(idleState, lightState, 0f, (PlayerAnimatorIds.IsLightAttackingName, true));
            Link(lightState, idleState, 0.05f, (PlayerAnimatorIds.IsLightAttackingName, false));

            // Charge → Heavy. Порядок переходов из ChargeHold важен: сначала Heavy.
            Link(idleState, chargeState, 0f, (PlayerAnimatorIds.IsChargingName, true));
            Link(chargeState, heavyState, 0f, (PlayerAnimatorIds.IsHeavyAttackingName, true));
            Link(chargeState, idleState, 0.05f,
                (PlayerAnimatorIds.IsChargingName, false),
                (PlayerAnimatorIds.IsHeavyAttackingName, false));
            Link(idleState, heavyState, 0f, (PlayerAnimatorIds.IsHeavyAttackingName, true));
            Link(heavyState, idleState, 0.05f, (PlayerAnimatorIds.IsHeavyAttackingName, false));

            // Dodge / Parry
            Link(idleState, dodgeState, 0f, (PlayerAnimatorIds.IsDodgingName, true));
            Link(dodgeState, idleState, 0.05f, (PlayerAnimatorIds.IsDodgingName, false));
            Link(idleState, parryState, 0f, (PlayerAnimatorIds.IsParryingName, true));
            Link(parryState, idleState, 0.05f, (PlayerAnimatorIds.IsParryingName, false));

            Finish(controller);
        }

        [MenuItem("Game/Animations/Create Left Hand Controller")]
        public static void BuildLeft()
        {
            string controllerPath = $"{RootFolder}/LeftHand_Base.controller";
            if (!CanCreate(controllerPath)) return;

            string folder = EnsureFolder("Left");
            Transform pose = Selection.activeTransform;

            AnimationClip idle = GetOrCreateClip(folder, PlayerAnimatorIds.IdleClip, 1f, true, pose);
            AnimationClip block = GetOrCreateClip(folder, PlayerAnimatorIds.BlockRaiseClip, 0.2f, false, pose);

            AnimatorController controller = AnimatorController.CreateAnimatorControllerAtPath(controllerPath);
            controller.AddParameter(PlayerAnimatorIds.IsBlockingName, AnimatorControllerParameterType.Bool);

            AnimatorStateMachine sm = controller.layers[0].stateMachine;
            AnimatorState idleState = AddState(sm, "Idle", idle);
            sm.defaultState = idleState;
            AnimatorState blockState = AddState(sm, "BlockRaise", block);

            // BlockRaise не зациклен: последний кадр держит щит поднятым, пока IsBlocking = true.
            Link(idleState, blockState, 0.05f, (PlayerAnimatorIds.IsBlockingName, true));
            Link(blockState, idleState, 0.08f, (PlayerAnimatorIds.IsBlockingName, false));

            Finish(controller);
        }

        private static AnimatorState AddState(AnimatorStateMachine sm, string stateName, AnimationClip clip)
        {
            AnimatorState state = sm.AddState(stateName);
            state.motion = clip;
            return state;
        }

        private static void Link(AnimatorState from, AnimatorState to, float duration, params (string parameter, bool value)[] conditions)
        {
            AnimatorStateTransition transition = from.AddTransition(to);
            transition.hasExitTime = false;
            transition.hasFixedDuration = true;
            transition.duration = duration;

            foreach ((string parameter, bool value) in conditions)
            {
                transition.AddCondition(value ? AnimatorConditionMode.If : AnimatorConditionMode.IfNot, 0f, parameter);
            }
        }

        private static AnimationClip GetOrCreateClip(string folder, string clipName, float length, bool loop, Transform pose)
        {
            string path = $"{folder}/{clipName}.anim";
            AnimationClip existing = AssetDatabase.LoadAssetAtPath<AnimationClip>(path);
            if (existing != null) return existing;

            var clip = new AnimationClip { name = clipName };

            if (pose != null)
            {
                Vector3 position = pose.localPosition;
                Vector3 euler = pose.localEulerAngles;
                SetConstant(clip, "m_LocalPosition.x", position.x, length);
                SetConstant(clip, "m_LocalPosition.y", position.y, length);
                SetConstant(clip, "m_LocalPosition.z", position.z, length);
                SetConstant(clip, "localEulerAnglesRaw.x", euler.x, length);
                SetConstant(clip, "localEulerAnglesRaw.y", euler.y, length);
                SetConstant(clip, "localEulerAnglesRaw.z", euler.z, length);
            }

            AnimationClipSettings settings = AnimationUtility.GetAnimationClipSettings(clip);
            settings.loopTime = loop;
            AnimationUtility.SetAnimationClipSettings(clip, settings);

            AssetDatabase.CreateAsset(clip, path);
            return clip;
        }

        private static void SetConstant(AnimationClip clip, string property, float value, float length)
        {
            EditorCurveBinding binding = EditorCurveBinding.FloatCurve("", typeof(Transform), property);
            AnimationUtility.SetEditorCurve(clip, binding, AnimationCurve.Constant(0f, length, value));
        }

        private static bool CanCreate(string controllerPath)
        {
            if (AssetDatabase.LoadAssetAtPath<AnimatorController>(controllerPath) == null) return true;

            Debug.LogWarning($"HandAnimatorBuilder: {controllerPath} уже существует — не перезаписываю. Удалите его, если нужно создать заново.");
            return false;
        }

        private static string EnsureFolder(string sub)
        {
            if (!AssetDatabase.IsValidFolder("Assets/Animations")) AssetDatabase.CreateFolder("Assets", "Animations");
            if (!AssetDatabase.IsValidFolder(RootFolder)) AssetDatabase.CreateFolder("Assets/Animations", "ViewModel");

            string subPath = $"{RootFolder}/{sub}";
            if (!AssetDatabase.IsValidFolder(subPath)) AssetDatabase.CreateFolder(RootFolder, sub);
            return subPath;
        }

        private static void Finish(AnimatorController controller)
        {
            EditorUtility.SetDirty(controller);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Selection.activeObject = controller;
            Debug.Log($"HandAnimatorBuilder: создан {AssetDatabase.GetAssetPath(controller)}");
        }
    }
}