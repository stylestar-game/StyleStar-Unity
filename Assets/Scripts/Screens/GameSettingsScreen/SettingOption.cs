using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace StyleStar
{
    public class SettingOption
    {
        public static Color SelectionColor { get; set; } = ThemeColors.Pink;
        public static Color InactiveColor { get; set; } = new Color(172, 172, 172);

        public string Title { get; private set; }
        public List<string> Options { get; set; } = new List<string>();
        public int SelectedOption { get; set; } = 0;

        private string separator = "·";
        private List<Vector2> optionOffset = new List<Vector2>();

        private static Vector3 titlePoint = new Vector3(-174, 320);
        private static Vector3 titleOffset = new Vector3(-47, -81);

        private GameObject optionObj;

        public SettingOption(string title, string[] options)
        {
            optionObj = Pools.GameSettingOptions.GetPooledObject();

            Title = title;
            var titleObj = optionObj.GetComponent<TextMeshProUGUI>();
            titleObj.text = Title;
            Options.AddRange(options);

            optionObj.SetActive(true);
        }

        public void Draw(int index)
        {
            string text = "";
            for (int i = 0; i < Options.Count; i++)
            {
                if (SelectedOption == i)
                    text += string.Format("<color=#{0}>{1}</color>", ColorUtility.ToHtmlStringRGB(SelectionColor), Options[i]);
                else
                    text += Options[i];

                if (i < (Options.Count - 1))
                    text += " " + separator + " ";
            }

            var subOptionsObj = optionObj.transform.Find("OptionText").GetComponent<TextMeshProUGUI>();
            subOptionsObj.text = text;

            optionObj.transform.localPosition = new Vector3(titlePoint.x, titlePoint.y) + index * titleOffset;

        }

        public void ScrollRight()
        {
            SelectedOption++;
            if (SelectedOption >= Options.Count)
                SelectedOption = 0;
        }

        public void ScrollLeft()
        {
            SelectedOption--;
            if (SelectedOption < 0)
                SelectedOption = (Options.Count - 1);
        }
    }
}
