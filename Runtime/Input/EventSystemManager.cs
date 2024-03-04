using UnityEngine.InputSystem.UI;
using UnityEngine.EventSystems;
using System.Collections;
using VibePack.Utility;
using VibePack.Input;
using UnityEngine;

namespace VibePack
{
    [RequireComponent(typeof(EventSystem))]
    [RequireComponent(typeof(InputSystemUIInputModule))]
    public class EventSystemManager : MonoBehaviour
    {
        [SerializeField] bool hideMouseOnPress;
        [SerializeField] float mouseMovementTreshold;
        [SerializeField] InputEvent clickEvent;
        [SerializeField] InputEvent mouseDragEvent;

        GameObject lastSelectedGameObject;
        UnityEngine.InputSystem.InputAction clickAction;
        EventSystem eventSystem;
        bool mouseActive = true;

        private void Start()
        {
            clickAction = GetComponent<InputSystemUIInputModule>().actionsAsset.FindAction(clickEvent.ToString());
            eventSystem = GetComponent<EventSystem>();
        }

        private void DeactivateMouse()
        {
            InputManager.On(mouseDragEvent).Bind(MouseDrag);
            Cursor.visible = mouseActive = false;
            Cursor.lockState = CursorLockMode.Locked;
            clickAction.Disable();

            if (lastSelectedGameObject == null)
                return;

            IEnumerator ReselectButton()
            {
                yield return new WaitForFrames(1);
                eventSystem.SetSelectedGameObject(lastSelectedGameObject);
            }

            StartCoroutine(ReselectButton());
        }

        private void ActivateMouse()
        {
            InputManager.On(mouseDragEvent).Unbind(MouseDrag);
            Cursor.visible = mouseActive = true;
            Cursor.lockState = CursorLockMode.None;
            clickAction.Enable();
        }

        private void MouseDrag(InputValue value)
        {
            if (value.Get<Vector2>().magnitude <= mouseMovementTreshold)
                return;

            if (lastSelectedGameObject == null)
                lastSelectedGameObject = eventSystem.currentSelectedGameObject;

            ActivateMouse();
        }

        private void OnNavigate(InputValue value)
        {
            if (value.Get<Vector2>().magnitude <= Mathf.Epsilon)
                return;

            if(mouseActive)
                DeactivateMouse();

            if(eventSystem.currentSelectedGameObject != null)
                lastSelectedGameObject = eventSystem.currentSelectedGameObject;
        }
    }
}
