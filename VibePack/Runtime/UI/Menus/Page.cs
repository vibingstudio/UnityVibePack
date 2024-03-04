using UnityEngine.Events;
using System.Collections;
using VibePack.Utility;
using VibePack.Input;
using UnityEngine;

namespace VibePack.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class Page : MonoBehaviour, IAwaitable
    {
        [Title("Proto Page", 3)]
        [SerializeField] Optional<PageManager> manager;
        [SerializeField] Optional<TransitionSettings> transitionSettings;
        [SerializeField] Optional<InputEvent> canCancel;

        [Space(20)]
        public UnityEvent onOpen;
        public UnityEvent onClose;
        public UnityEvent onOpened;
        public UnityEvent onClosed;

        CanvasGroup canvasGroup;
        bool isTransitioning;
        bool isOpen;

        protected virtual void Start()
        {
            if (canvasGroup == null)
                canvasGroup = GetComponent<CanvasGroup>();

            isOpen = gameObject.activeSelf;
        }

        private void OnOpened()
        {
            onOpened.RemoveAllListeners();
            isTransitioning = false;
            isOpen = true;

            if (manager && manager.Value != null)
                manager.Value.OpenFirst();

            if(canCancel)
                InputManager.On(canCancel).Bind(OnCancel);
        }

        private void OnCancel(InputValue input) => Close();

        private void OnClosed()
        {
            isTransitioning = false;
            onClosed.RemoveAllListeners();
            gameObject.SetActive(false);
            isOpen = false;
        }

        private IEnumerator CloseSequence()
        {
            isTransitioning = true;
            if(canCancel)
                InputManager.On(canCancel).Unbind(OnCancel);

            if (manager && manager.Value is not null and PageManager managerReference)
            {
                managerReference.CloseAll();
                yield return managerReference.Await();
            } 

            onClose?.Invoke();
            onClose.RemoveAllListeners();
            onClosed.AddListener(OnClosed);

            if (!transitionSettings)
            {
                onClosed?.Invoke();
                yield break;
            }

            TransitionDirector director = new TransitionDirector(false, transitionSettings, canvasGroup, transform);
            director.OnComplete(() => onClosed?.Invoke());
            director.Build();
        }

        public void Open()
        {
            if (isTransitioning || isOpen)
                return;

            if (canvasGroup == null)
                canvasGroup = GetComponent<CanvasGroup>();

            canvasGroup.alpha = 0;
            gameObject.SetActive(true);
            isTransitioning = true;
            onOpen?.Invoke();
            onOpen.RemoveAllListeners();
            onOpened.AddListener(OnOpened);

            if (!transitionSettings)
            {
                onOpened?.Invoke();
                return;
            }

            TransitionDirector director = new TransitionDirector(true, transitionSettings, canvasGroup, transform);
            director.OnComplete(() => onOpened?.Invoke());
            director.Build();
        }

        public void Close()
        {
            if (isTransitioning || !isOpen)
                return;

            StartCoroutine(CloseSequence());
        }

        public bool IsOpen() => isOpen;

        public virtual bool ShouldWait() => isTransitioning;

        public CustomYieldInstruction Await() => new Awaiter(this);

        public static implicit operator Awaiter(Page protoable) => new Awaiter(protoable);
    }
}
