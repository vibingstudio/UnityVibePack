using UnityEngine.Events;
using System.Collections;
using VibePack.Utility;
using VibePack.Input;
using UnityEngine;

namespace VibePack.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class ScreenStick : MonoBehaviour
    {
        [Title("Screen Stick")]
        [SerializeField] Optional<RectTransform> touchArea;
        [SerializeField] Transform handle;
        [SerializeField] InputEvent clickEvent;
        [SerializeField] InputEvent moveEvent;
        [SerializeField] float handleDistance = 1;
        [SerializeField] float smartClickTime = 0.2f;
        [SerializeField] float timeToFade = 0.2f;
        [Space(10)]
        [SerializeField] UnityEvent<Vector2> onMove;
        [SerializeField] UnityEvent onSmartClick;

        CanvasGroup canvasGroup;
        Coroutine fadeCoroutine;
        Camera cam;
        Rect touchAreaRect;
        Vector2 mousePos;
        Vector2 pressPos;
        float clickTimestamp;
        bool isInsideArea;
        bool isPressed;

        private void Start()
        {
            cam = Camera.main;
            canvasGroup = GetComponent<CanvasGroup>();
            canvasGroup.alpha = 0;

            if (!touchArea || touchArea.Value == null)
                return;

            Vector3[] corners = new Vector3[4];
            touchArea.Value.GetWorldCorners(corners);
            Vector2 min = new Vector2(Mathf.Infinity, Mathf.Infinity);
            Vector2 max = new Vector2(-Mathf.Infinity, -Mathf.Infinity);

            for (int i = 0; i < corners.Length; i++)
            {
                Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(Camera.main, corners[i]);
                min = Vector2.Min(min, screenPoint);
                max = Vector2.Max(max, screenPoint);
            }

            touchAreaRect = new Rect(min.x, min.y, max.x - min.x, max.y - min.y);
        }

        private void OnEnable()
        {
            StartCoroutine(Await());

            IEnumerator Await()
            {
                if (!InputManager.InputAvailable())
                    yield return InputManager.WaitForInitialization();

                InputManager.On(clickEvent).Bind(OnClick);
                InputManager.On(moveEvent).Bind(OnMove);
            }
        }

        private void OnDisable()
        {
            InputManager.On(clickEvent).Unbind(OnClick);
            InputManager.On(moveEvent).Unbind(OnMove);
        }

        private IEnumerator Fade(float target)
        {
            for (float time = 0, startingFade = canvasGroup.alpha; time < timeToFade; time += Time.deltaTime)
            {
                canvasGroup.alpha = Mathf.Lerp(startingFade, target, time / timeToFade);
                yield return null;
            }

            canvasGroup.alpha = target;
            fadeCoroutine = null;
        }

        private void OnClick(InputValue data)
        {
            if (fadeCoroutine != null)
                StopCoroutine(fadeCoroutine);

            if (!(isPressed = data.isPressed) || !isInsideArea)
            {
                if (Time.time - clickTimestamp <= smartClickTime)
                    onSmartClick?.Invoke();

                fadeCoroutine = StartCoroutine(Fade(0));
                return;
            }

            clickTimestamp = Time.time;
            fadeCoroutine = StartCoroutine(Fade(1));
            handle.position = transform.position = pressPos = mousePos;
        }

        private void OnMove(InputValue data)
        {
            Vector2 input = data.Get<Vector2>();
            isInsideArea = !touchArea || touchArea.Value == null || touchAreaRect.Contains(input);
            mousePos = cam.ScreenToWorldPoint(input);

            if (!isPressed)
                return;

            Vector2 handlePosition = mousePos - pressPos;
            onMove?.Invoke(handlePosition.normalized);

            if (handlePosition.magnitude > handleDistance)
                handlePosition = Vector3.ClampMagnitude(handlePosition, handleDistance);

            handle.position = (Vector2)transform.position + handlePosition;
        }

        public void AddClickCallback(UnityAction action) => onSmartClick.AddListener(action);

        public void RemoveClickCallback(UnityAction action) => onSmartClick.RemoveListener(action);

        public void AddMoveCallback(UnityAction<Vector2> action) => onMove.AddListener(action);

        public void RemoveMoveCallback(UnityAction<Vector2> action) => onMove.RemoveListener(action);
    }
}
