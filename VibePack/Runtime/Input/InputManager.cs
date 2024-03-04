using Gyroscope = UnityEngine.InputSystem.Gyroscope;
using static UnityEngine.InputSystem.InputAction;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using System.Collections;
using VibePack.Utility;
using UnityEngine;
using System;

namespace VibePack.Input
{
    /// <summary>
    /// Struct for handling input. Mimics Unity's CallbackContext.
    /// </summary>
    public struct InputValue
    {
        CallbackContext context;

        public InputValue(CallbackContext context) => this.context = context;

        public object Get() => context.ReadValueAsObject();

        public T Get<T>() where T : struct => context.ReadValue<T>();

        public bool isPressed => Get<float>() >= 0.5f;
    }

    /// <summary>
    /// 
    /// </summary>
    [CreateAssetMenu(menuName = "VibePack/Input Manager", fileName = "InputManager")]
    public class InputManager : ScriptableObject
    {
        [Title("Input Manager")]
        [SerializeField] InputActionAsset actionAsset;

        readonly static Dictionary<string, Action<CallbackContext>> callbacks = new Dictionary<string, Action<CallbackContext>>();
        
        static InputAction currentInputAction;
        static InputManager instance;
        static InputActionPhase phase;
        static InputMap currentMap;

        public void Initialize() => instance = this;

        public static bool InputAvailable() => instance != null;

        public static IEnumerator WaitForInitialization()
        {
            yield return new WaitUntil(() => instance != null);
        }

        private static InputActionMap GetMap() => instance.actionAsset.FindActionMap(currentMap.ToString());

        private static string GetKey(Action<InputValue> action) => $"{currentMap}.{currentInputAction.name}.{phase}{{{action.GetHashCode()}}}";

        public static void ChangeMap(InputMap newMap)
        {
            if (instance == null || (newMap == currentMap && GetMap().enabled))
                return;

            GetMap()?.Disable();
            currentMap = newMap;
            GetMap().Enable();
        }

        public static InputManager On(InputEvent key)
        {
            if (instance == null)
                return null;

            currentInputAction = GetMap()?.FindAction(key.ToString());
            phase = InputActionPhase.Performed;
            return instance;
        }

        public InputManager Started()
        {
            phase = InputActionPhase.Started;
            return instance;
        }

        public InputManager Canceled()
        {
            phase = InputActionPhase.Canceled;
            return instance;
        }

        public void Bind(Action<InputValue> action)
        {
            if (currentInputAction == null || instance == null)
                return;

            Action<CallbackContext> callback = ctx => action?.Invoke(new InputValue(ctx));
            switch (phase)
            {
                case InputActionPhase.Started:
                    currentInputAction.started += callback;
                    break;
                case InputActionPhase.Performed:
                    currentInputAction.performed += callback;
                    break;
                case InputActionPhase.Canceled:
                    currentInputAction.canceled += callback;
                    break;
            }

            callbacks[GetKey(action)] = callback;
            currentInputAction = null;
            return;
        }

        public void Unbind(Action<InputValue> action)
        {
            if (!callbacks.TryGetValue(GetKey(action), out Action<CallbackContext> callback))
                return;

            switch (phase)
            {
                case InputActionPhase.Started:
                    currentInputAction.started -= callback;
                    break;
                case InputActionPhase.Performed:
                    currentInputAction.performed -= callback;
                    break;
                case InputActionPhase.Canceled:
                    currentInputAction.canceled -= callback;
                    break;
            }
        }

        public static void TurnOnAccelerometer()
        {
#if !UNITY_EDITOR
            if (Accelerometer.current != null)
                InputSystem.EnableDevice(Accelerometer.current);
#endif
        }

        public static void TurnOffAccelerometer()
        {
#if !UNITY_EDITOR
            if (Accelerometer.current != null)
                InputSystem.EnableDevice(Accelerometer.current);
#endif
        }

        public static void TurnOnGyroscope()
        {
#if !UNITY_EDITOR
            if (Gyroscope.current != null)
                InputSystem.EnableDevice(Gyroscope.current);
#endif
        }

        public static void TurnOffGyroscope()
        {
#if !UNITY_EDITOR
            if (Gyroscope.current != null)
                InputSystem.EnableDevice(Gyroscope.current);
#endif
        }
    }
}
