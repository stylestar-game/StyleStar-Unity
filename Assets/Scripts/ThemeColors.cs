using UnityEngine;

namespace StyleStar
{
    public static class ThemeColors
    {
        public static Color FolderBGColor = new Color32(136, 0, 21, 255);
        public static Color FolderBGColorSub = new Color32(221, 0, 33, 255);

        public static Color InactiveSelectionColor = new Color32(64, 64, 64, 255);

        public static Color Purple = new Color32(170, 71, 186, 255);
        public static Color Pink = new Color32(249, 79, 142, 255);
        public static Color Yellow = new Color32(234, 237, 53, 255);
        public static Color Green = new Color32(28, 206, 40, 255);
        public static Color Blue = new Color32(97, 224, 225, 255);
        public static Color BrightGreen = new Color32(117, 252, 49, 255);

        public static Color NullColor = new Color32(0, 0, 0, 0);

        public static Color Stylish = new Color32(255, 252, 104, 255);
        public static Color Good = new Color32(39, 234, 175, 255);
        public static Color Bad = new Color32(183, 120, 255, 255);
        public static Color Miss = new Color32(255, 70, 70, 255);

        public static Color ClearedFont = new Color32(159, 243, 255, 255);
        public static Color ClearedStroke = new Color32(0, 58, 138, 255);

        public static Color FailedFont = new Color32(255, 55, 164, 255);
        public static Color FailedStroke = new Color32(118, 19, 79, 255);

        public static Color FullComboFont = new Color32(207, 255, 138, 255);
        public static Color FullComboStroke = new Color32(0, 112, 19, 255);

        private static Color[] colorArray = new Color[] { Purple, Pink, Yellow, Green, Blue };

        public static Color GetColor(int index)
        {
            return colorArray[index % colorArray.Length];
        }
    }
}
