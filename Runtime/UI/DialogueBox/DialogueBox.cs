using System.Collections.Generic;
using VibePack.Extensions;
using UnityEngine.Events;
using System.Collections;
using VibePack.Utility;
using VibePack.Input;
using UnityEngine;
using DG.Tweening;

namespace VibePack.UI
{
    [System.Serializable]
    public class IntUnityEvent : UnityEvent<int> { }

    [RequireComponent(typeof(CanvasGroup))]
    public class DialogueBox : Page
    {
        [Title("Dialogue Box", 1)]
        [SerializeField] TMP_Animated text;
        [SerializeField] Transform buttonArea;
        [SerializeField] Transform buttonPrefab;
        [SerializeField] InputEvent clickEvent;

        public IntUnityEvent onOptionChosen = new IntUnityEvent();

        Dialogue[] dialogues;
        bool mousePressed;
        bool working;

        private YieldInstruction HideText() => text.DOFade(0, 0.2f).WaitForCompletion();

        private YieldInstruction ShowText() => text.DOFade(1, 0.2f).WaitForCompletion();

        private IEnumerator DisplayDialogues()
        {
            yield return Await();
            InputManager.On(clickEvent).Bind(OnPress);

            foreach (Dialogue dialogue in dialogues)
            {
                yield return HideText();
                text.StopEffects();
                text.text = "";
                yield return ShowText();

                text.ReadText(dialogue.text);
                yield return text.Await();

                if (dialogue.choices && dialogue.choices.Value.Length > 0)
                {
                    yield return DisplayOptions(dialogue);
                    continue;
                }

                mousePressed = false;
                yield return new WaitUntil(() => mousePressed);
            }

            working = false;
            InputManager.On(clickEvent).Unbind(OnPress);
            Close();
            yield return Await();
        }

        private void OnPress(InputValue input)
        {
            if (!working || !input.isPressed)
                return;

            if (mousePressed = input.isPressed)
                mousePressed = true;
        }

        private IEnumerator DisplayOptions(Dialogue dialogue)
        {
            int optionChosen = -1;
            string[] choices = dialogue.choices.Value;

            DialogueButton[] buttons = new DialogueButton[choices.Length];
            for (int i = 0; i < choices.Length; i++)
            {
                var choice = choices[i];
                DialogueButton button = Instantiate(buttonPrefab, buttonArea).GetComponent<DialogueButton>();
                button.SetText(choice);
                button.onPointerClick.AddListener(x => optionChosen = i);
                buttons[i] = button;
            }

            yield return null;
            List<CustomYieldInstruction> instructions = new List<CustomYieldInstruction>();

            foreach (DialogueButton button in buttons)
            {
                button.Open();
                instructions.Add(button);
            }

            yield return instructions.Await();
            yield return new WaitUntil(() => optionChosen >= 0);

            onOptionChosen?.Invoke(optionChosen);
            instructions.Clear();

            foreach (DialogueButton button in buttons)
            {
                button.Close();
                button.onPointerClick.RemoveAllListeners();
                instructions.Add(button);
            }

            yield return instructions.Await();
            instructions.Clear();

            foreach (Transform child in buttonArea)
                Destroy(child.gameObject);

            yield return null;
        }

        public Coroutine DisplayText(params Dialogue[] dialogues)
        {
            if (working || ShouldWait())
                return null;

            this.dialogues = dialogues;
            working = true;
            Open();
            return StartCoroutine(DisplayDialogues());
        }
    }
}
