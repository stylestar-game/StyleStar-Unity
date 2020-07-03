using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace StyleStar
{
    // controls separate GUI which allows scrolling upwards/downwards. We could use this again, so this has it's own file for now.
    public class SettingSelector
    {
        private GameObject selectorObj;
        private List<string> selectionOptions;
        private int currSelectionOption = 0;
        private string innerText;

        public SettingSelector(GameObject inSelectorObj, List<string> inSelectionOptions, string inInnerText)
        {
            selectorObj = inSelectorObj;
            selectionOptions = inSelectionOptions;
            innerText = inInnerText;
            selectorObj.SetActive(true);
        }

        public void SetNewSelector(GameObject inSelectorObj)
        {
            selectorObj = inSelectorObj;
        }

        public void SetNewOptions(List<string> inNewSelectionOptions)
        {
            selectionOptions.Clear();
            selectionOptions = inNewSelectionOptions;
        }

        public void Draw()
        {
            var selectorText = selectorObj.transform.Find(innerText).GetComponent<TextMeshProUGUI>();
            selectorText.SetText(selectionOptions[currSelectionOption]);
        }

        public void ScrollUp()
        {
            currSelectionOption++;
            if (currSelectionOption >= selectionOptions.Count)
                currSelectionOption = 0;
        }

        public void ScrollDown()
        {
            currSelectionOption--;
            if (currSelectionOption < 0)
                currSelectionOption = selectionOptions.Count - 1;
        }

        public int Select() // Return current selected option. Client will do something based on this response.
        {
            return currSelectionOption;
        }
    }
}
