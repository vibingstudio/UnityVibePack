using VibePack.Transitions;
using VibePack.Utility;
using VibePack.Input;
using UnityEngine;

namespace VibePack
{
    /// <summary>
    /// Initializes VibePacks Primary Systems.
    /// </summary>
    public class PersistentObject : MonoBehaviour
    {
        [SerializeField] TransitionController transitionController;
        [SerializeField] InputManager inputManager;
        [SerializeField] Oracle oracle;

        /// <summary>
        /// Sinleton Instance
        /// </summary>
        protected static PersistentObject instance;

        /// <summary>
        /// Start manager initialization. Override to stop execution.
        /// </summary>
        private void Awake() => Initialize();

        /// <summary>
        /// Initializes all managers. Returns a bool for singleton purposes.
        /// </summary>
        protected virtual bool Initialize()
        {
            if (instance != null)
            {
                Destroy(gameObject);
                return false;
            }

            DontDestroyOnLoad(gameObject);
            instance = this;
            Color color = Color.yellow;
            string colorHex = $"<color=#{(int)(color.r * 255):X2}{(int)(color.g * 255):X2}{(int)(color.b * 255):X2}>";

            if (oracle == null)
                Debug.Log($"{colorHex}==> Oracle Reference Not Set.</color>");
            else
                oracle.Initialize();

            if (inputManager == null)
                Debug.Log($"{colorHex}==> Input Manager</size> <size=15>Reference Not Set.</color>");
            else
                inputManager.Initialize();

            if (transitionController == null)
                Debug.Log($"{colorHex}==> Transition Controller Reference Not Set.</color>");
            else
                transitionController.Initialize();

            return true;
        }
    }
}
