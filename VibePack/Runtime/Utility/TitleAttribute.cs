using UnityEngine;

namespace VibePack.Utility
{
    public class TitleAttribute : PropertyAttribute
    {
        public Color color;
        public string text;
        public bool spacingTop;
        public bool useColor;

        public TitleAttribute(string text)
        {
            this.text = text;
            color = ColorExtension.HtmlToColor("#208ed4");
        }

        public TitleAttribute(string text, bool spacingTop)
        {
            this.text = text;
            this.spacingTop = spacingTop;
            color = ColorExtension.HtmlToColor("#208ed4");
        }

        public TitleAttribute(string text, bool spacingTop, int colorIndex) : this(text, colorIndex) => this.spacingTop = spacingTop;

        public TitleAttribute(string text, int colorIndex)
        {
            if (colorIndex > 5)
                colorIndex = 5;

            Color[] colors;
            colors = new Color[]
            {
                    ColorExtension.HtmlToColor("208ed4"), // Blue
                    ColorExtension.HtmlToColor("8ac926"), // Green
                    ColorExtension.HtmlToColor("ebbc3d"), // Yellow
                    ColorExtension.HtmlToColor("e36a68"), // Red
                    ColorExtension.HtmlToColor("3bceac"), // Cyan
                    ColorExtension.HtmlToColor("bc87de"), // Purple
            };

            this.text = text;
            color = colors[colorIndex];
            useColor = true;
        }
    }
}
