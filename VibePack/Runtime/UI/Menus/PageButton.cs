using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine.Events;
using VibePack.Utility;
using UnityEngine.UI;
using UnityEngine;
using System;

namespace VibePack.UI
{
    [Serializable]
    public class PointerDataUnityEvent : UnityEvent<PointerEventData> { }

    [Serializable]
    public class BaseDataUnityEvent : UnityEvent<BaseEventData> { }

    [Serializable]
    public struct Navigation
    {
        [SerializeField] public GameObject up;
        [SerializeField] public GameObject down;
        [SerializeField] public GameObject right;
        [SerializeField] public GameObject left;
    }

    [RequireComponent(typeof(CanvasGroup))]
    public class PageButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler,
        IMoveHandler, ISelectHandler, IDeselectHandler, ISubmitHandler, IAwaitable
    {
        [Title("Page Button", 5)]
        [SerializeField] Optional<TransitionSettings> transitionSettings;
        [SerializeField] protected bool startOpen;
        [SerializeField] bool selectOnHover;
        [SerializeField] float transitionTime = 0.15f;
        [SerializeField] Optional<float> hoverSize;
        [SerializeField] Optional<Color> hoverColor;

        [Space(10)]
        [SerializeField] Optional<CanvasGroup> outline;
        [SerializeField] Optional<Navigation> navigation;

        [Space(10)]
        public BaseDataUnityEvent onSubmit = new BaseDataUnityEvent();
        public BaseDataUnityEvent onPointerClick = new BaseDataUnityEvent();
        public PointerDataUnityEvent onPointerEnter = new PointerDataUnityEvent();
        public PointerDataUnityEvent onPointerExit = new PointerDataUnityEvent();
        public PointerDataUnityEvent onPointerUp = new PointerDataUnityEvent();
        public PointerDataUnityEvent onPointerDown = new PointerDataUnityEvent();

        [Space(20)]
        public UnityEvent onOpen;
        public UnityEvent onClose;
        public UnityEvent onOpened;
        public UnityEvent onClosed;

        protected bool isTransitioning;

        Color currentColor = Color.white;
        Coroutine outlineCoroutine;
        Coroutine colorCoroutine;
        Coroutine sizeCoroutine;
        CanvasGroup canvasGroup;
        Image[] images;

        bool isOpen;

        protected virtual void Start()
        {
            gameObject.SetActive(isOpen = startOpen);
            canvasGroup = GetComponent<CanvasGroup>();

            if (outline)
                outline.Value.alpha = 0;
        }

        private IEnumerator ChangeOutlineColor(float targetAlpha)
        {
            if (!outline)
                yield break;

            float startingAlha = outline.Value.alpha;
            for (float time = 0; time < transitionTime; time += Time.deltaTime)
            {
                outline.Value.alpha = Mathf.Lerp(startingAlha, targetAlpha, time / transitionTime);
                yield return null;
            }

            outline.Value.alpha = targetAlpha;
        }

        private IEnumerator ChangeColor(Color targetColor)
        {
            images ??= GetComponentsInChildren<Image>();

            if (images.Length <= 0)
                yield break;

            Color startingColor = currentColor;

            for (float time = 0; time < transitionTime; time += Time.deltaTime)
            {
                currentColor = Color.Lerp(startingColor, targetColor, Mathf.Abs(time / transitionTime));

                foreach (Image img in images)
                {
                    if (img == outline)
                        continue;

                    img.color = currentColor;
                }

                yield return null;
            }

            currentColor = targetColor;
        }

        private IEnumerator ChangeSize(float targetScale)
        {
            float startingScale = transform.localScale.x;

            for (float time = 0; time < transitionTime; time += Time.deltaTime)
            {
                transform.localScale = Vector3.one * Mathf.Lerp(startingScale, targetScale, time / transitionTime);
                yield return null;
            }

            transform.localScale = Vector3.one * targetScale;
        }

        private void OnOpened()
        {
            onOpened.RemoveAllListeners();
            isTransitioning = false;
            isOpen = true;
        }

        private void OnClosed()
        {
            isTransitioning = false;
            onClosed.RemoveAllListeners();
            gameObject.SetActive(false);
            isOpen = false;
        }

        public void Open()
        {
            if (isTransitioning || isOpen)
                return;

            isTransitioning = true;
            gameObject.SetActive(true);
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

            isTransitioning = true;
            onClose?.Invoke();
            onClose.RemoveAllListeners();
            onClosed.AddListener(OnClosed);

            if (!transitionSettings)
            {
                onClosed?.Invoke();
                return;
            }

            TransitionDirector director = new TransitionDirector(false, transitionSettings, canvasGroup, transform);
            director.OnComplete(() => onClosed?.Invoke());
            director.Build();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!isOpen)
                return;

            if (selectOnHover)
                EventSystem.current.SetSelectedGameObject(gameObject);

            onPointerEnter?.Invoke(eventData);

            if (!hoverSize)
                return;

            if (sizeCoroutine != null)
                StopCoroutine(sizeCoroutine);

            sizeCoroutine = StartCoroutine(ChangeSize(hoverSize));
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!isOpen)
                return;

            onPointerExit?.Invoke(eventData);

            if (!hoverSize)
                return;

            if (sizeCoroutine != null)
                StopCoroutine(sizeCoroutine);

            sizeCoroutine = StartCoroutine(ChangeSize(1));
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!isOpen)
                return;

            onPointerDown?.Invoke(eventData);

            if (!hoverColor)
                return;

            if (colorCoroutine != null)
                StopCoroutine(colorCoroutine);

            colorCoroutine = StartCoroutine(ChangeColor(hoverColor));
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!isOpen)
                return;

            onPointerUp?.Invoke(eventData);

            if (!hoverColor)
                return;

            if (colorCoroutine != null)
                StopCoroutine(colorCoroutine);

            colorCoroutine = StartCoroutine(ChangeColor(Color.white));
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!isOpen)
                return;

            if (!selectOnHover)
                EventSystem.current.SetSelectedGameObject(gameObject);

            onPointerClick?.Invoke(eventData);
        }

        public void OnMove(AxisEventData eventData)
        {
            if (!isOpen)
                return;

            switch (eventData.moveDir)
            {
                case MoveDirection.Up:
                    if (navigation.Value.up != null)
                        EventSystem.current.SetSelectedGameObject(navigation.Value.up);
                    break;
                case MoveDirection.Down:
                    if (navigation.Value.down != null)
                        EventSystem.current.SetSelectedGameObject(navigation.Value.down);
                    break;
                case MoveDirection.Right:
                    if (navigation.Value.right != null)
                        EventSystem.current.SetSelectedGameObject(navigation.Value.right);
                    break;
                case MoveDirection.Left:
                    if (navigation.Value.left != null)
                        EventSystem.current.SetSelectedGameObject(navigation.Value.left);
                    break;
            }
        }

        public void OnSelect(BaseEventData eventData)
        {
            if (!isOpen || !outline)
                return;

            if (outlineCoroutine != null)
                StopCoroutine(outlineCoroutine);

            outlineCoroutine = StartCoroutine(ChangeOutlineColor(1));
        }

        public void OnDeselect(BaseEventData eventData)
        {
            if (!isOpen || !outline)
                return;

            if (outlineCoroutine != null)
                StopCoroutine(outlineCoroutine);

            outlineCoroutine = StartCoroutine(ChangeOutlineColor(0));
        }

        public void OnSubmit(BaseEventData eventData)
        {
            if (!isOpen)
                return;

            onSubmit?.Invoke(eventData);
        }

        public virtual bool ShouldWait() => isTransitioning;

        public CustomYieldInstruction Await() => new Awaiter(this);

        public static implicit operator Awaiter(PageButton pageButton) => new Awaiter(pageButton);
    }
}
