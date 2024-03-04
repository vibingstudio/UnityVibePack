using VibePack.Utility;
using UnityEngine;
using TMPro;

namespace VibePack.UI
{
    public class DialogueButton : PageButton
    {
        [Title("Dialogue Button", 1)]
        [SerializeField] TextMeshProUGUI text;

        public void SetText(string option) => text.text = option;
    }
}
