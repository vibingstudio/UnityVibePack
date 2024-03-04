using UnityEngine.EventSystems;
using UnityEngine;
using VibePack.UI;

namespace VibePack.Utility
{
    public class SpriteButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler,
        IMoveHandler, ISubmitHandler
    {
        [Title("Sprite Button", 5)]
        [SerializeField] Optional<Navigation> navigation;

        [Space(10)]
        public BaseDataUnityEvent onSubmit = new BaseDataUnityEvent();
        public BaseDataUnityEvent onPointerClick = new BaseDataUnityEvent();
        public PointerDataUnityEvent onPointerEnter = new PointerDataUnityEvent();
        public PointerDataUnityEvent onPointerExit = new PointerDataUnityEvent();
        public PointerDataUnityEvent onPointerUp = new PointerDataUnityEvent();
        public PointerDataUnityEvent onPointerDown = new PointerDataUnityEvent();

        protected bool isEnabled = true;

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!isEnabled)
                return;

            onPointerEnter?.Invoke(eventData);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!isEnabled)
                return;

            onPointerExit?.Invoke(eventData);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!isEnabled)
                return;

            onPointerDown?.Invoke(eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!isEnabled)
                return;

            onPointerUp?.Invoke(eventData);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!isEnabled)
                return;

            onPointerClick?.Invoke(eventData);
        }

        public void OnMove(AxisEventData eventData)
        {
            if (!isEnabled)
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

        public void OnSubmit(BaseEventData eventData)
        {
            if (!isEnabled)
                return;

            onSubmit?.Invoke(eventData);
        }

        public void SetEnabled(bool enabled) => isEnabled = enabled;
    }
}
