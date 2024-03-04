using System.Collections;
using VibePack.Utility;
using DG.Tweening;
using UnityEngine;

namespace VibePack.Transitions
{
    /// <summary>
    /// Controller for all screen transitions.
    /// </summary>
    public class TransitionController : MonoBehaviour, IAwaitable
    {
        /// <summary>
        /// Material used for camera blitz.
        /// </summary>
        [SerializeField] Material material;
        /// <summary>
        /// Textures used for transitions.
        /// </summary>
        [SerializeField] Texture2D[] textures;
        /// <summary>
        /// Default transition to be used if necesary.
        /// </summary>
        [SerializeField] TransitionSettings defaultTransition;
        /// <summary>
        /// Determines wether the transition controller fades out at start.
        /// </summary>
        [SerializeField] bool fadeOnStart;

        /// <summary>
        /// Private instance for singleton.
        /// </summary>
        static TransitionController instance;

        /// <summary>
        /// Current transition type (Texture/Alpha).
        /// </summary>
        TransitionType transitionType;
        /// <summary>
        /// Stores how long the transition will last.
        /// </summary>
        float transitionDuration;
        /// <summary>
        /// Active when a transition is currently active.
        /// </summary>
        bool transitionActive;

        /// <summary>
        /// Fades out on Start if fadeOnStart is active.
        /// </summary>
        /// <returns></returns>
        private IEnumerator Start()
        {
            if (!fadeOnStart || instance != this)
                yield break;

            yield return null;
            yield return WaitForTransition();
            FadeOut();
        }

        /// <summary>
        /// Makes sure the transition is inactive on editor.
        /// </summary>
        private void OnApplicationQuit() => material.SetFloat("_Cutoff", 0);

        /// <summary>
        /// Transitions between open and close.
        /// </summary>
        /// <param name="startValue">Starting transition value.</param>
        /// <param name="endValue">Ending transition value.</param>
        /// <returns>YieldInstruction for the transition.</returns>
        private YieldInstruction Transition(float startValue, float endValue)
        {
            if (transitionActive)
                return null;

            transitionActive = true;
            string property = transitionType == TransitionType.Texture ? "_Cutoff" : "_Fade";
            Tweener tweener = DOTween.To(v => material.SetFloat(property, v), startValue, endValue, transitionDuration);
            return tweener.OnComplete(() => transitionActive = false).WaitForCompletion();
        }

        /// <summary>
        /// Changes the settings for transitions.
        /// </summary>
        /// <param name="settings">Settings to be used.</param>
        private void SetSettings(TransitionSettings settings)
        {
            if (settings.transitionType == TransitionType.Texture)
                material.SetTexture("_TransitionTex", textures[Mathf.Clamp((int)settings.textureId, 0, textures.Length)]);

            if (settings.changeValues)
            {
                int value = settings.transitionType == TransitionType.Texture ? 1 : 0;
                material.SetFloat("_Cutoff", 1);
                material.SetFloat("_Fade", value);
            }

            transitionDuration = settings.transitionTime;
            transitionType = settings.transitionType;
            material.SetColor("_Color", settings.color);
        }

        /// <summary>
        /// Initializes the manager.
        /// </summary>
        public void Initialize()
        {
            if (instance != null)
                return;

            instance = this;

            if (!fadeOnStart)
                return;

            SetSettings(defaultTransition);
        }

        /// <summary>
        /// Used for Awaiter.
        /// </summary>
        /// <returns></returns>
        public bool ShouldWait() => transitionActive;

        /// <summary>
        /// Used for Awaiter.
        /// </summary>
        /// <returns></returns>
        public CustomYieldInstruction Await() => new Awaiter(this);

        /// <summary>
        /// Fades in.
        /// </summary>
        /// <returns>YieldInstruction for the transition.</returns>
        public static YieldInstruction FadeIn()
        {
            if (instance == null)
                return null;

            return instance.Transition(0, 1);
        }

        /// <summary>
        /// Fades out.
        /// </summary>
        /// <returns>YieldInstruction for the transition.</returns>
        public static YieldInstruction FadeOut()
        {
            if (instance == null)
                return null;

            return instance.Transition(1, 0);
        }

        /// <summary>
        /// Changes the settings for transitions.
        /// </summary>
        /// <param name="settings">Settings to be used.</param>
        public static void SetTransitionSettings(TransitionSettings settings)
        {
            if (instance == null)
                return;

            instance.SetSettings(settings);
        }

        /// <summary>
        /// Sets the default transition settings as active.
        /// </summary>
        public static void SetDefaultTransitionSettings()
        {
            if (instance == null)
                return;

            instance.SetSettings(instance.defaultTransition);
        }

        /// <summary>
        /// Returns a YieldInstruction that waits until the controller is ready to transiton again.
        /// </summary>
        /// <returns>YieldInstruction that waits until the controller is ready to transiton again.</returns>
        public static CustomYieldInstruction WaitForTransition()
        {
            if(instance == null || !instance.transitionActive)
                return null;

            return instance.Await();
        }
    }
}