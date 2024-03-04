using System;
using UnityEngine;
using VibePack.Utility;

namespace VibePack.UI
{
    [Serializable]
    public struct Dialogue
    {
        public string text;
        public Optional<string[]> choices;

        public Dialogue(string text, params string[] choices)
        {
            this.text = text;
            this.choices = choices;
        }
    }

    [CreateAssetMenu(menuName = "VibePack/DialogueSequence", fileName = "DialogueSequence")]
    public class DialogueSequence : ScriptableObject
    {
        [Title("Dialogue")]
        [SerializeField] Dialogue[] dialogues;

        public static implicit operator Dialogue[](DialogueSequence self) => self.dialogues;
    }
}
