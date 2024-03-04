using UnityEngine;

namespace VibePack
{
    public class ColorExtension
    {
        public static Color HtmlToColor(string hex)
        {
            if (hex.StartsWith("#"))
                hex = hex[1..];

            if (hex.Length != 6)
            {
                Debug.LogError("The hexadecimal color format is incorrect. It must have 6 characters.");
                return Color.white;
            }

            byte r = (byte)int.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
            byte g = (byte)int.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
            byte b = (byte)int.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);

            return new Color32(r, g, b, 255);
        }
    }
}
