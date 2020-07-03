using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StyleStar
{
    public static class LanguageSelect
    {
        private static SettingSelector settingSelector = null;
        private static bool confirmSelection = false;
        private static GameObject confirmObjectLabel;

        public static Mode NextGameMode { get; set; } = Mode.SongSelect;

        public static void Initialize(GameObject inConfirmLabel)
        {
            if (settingSelector == null)
            {
                settingSelector = new SettingSelector(GameObject.Find("ScrollSelector").gameObject,
                    new List<string>(){
                    ConfigFile.GetLocalizedString("Lang_Name", Language.English),
                    ConfigFile.GetLocalizedString("Lang_Name", Language.Spanish),
                    ConfigFile.GetLocalizedString("Lang_Name", Language.Japanese),
                    ConfigFile.GetLocalizedString("Lang_Name", Language.French),
                }, "SelectorOption");
            }
            else
                settingSelector.SetNewSelector(GameObject.Find("ScrollSelector").gameObject);
            confirmObjectLabel = inConfirmLabel;
        }

        public static void Draw()
        {
            settingSelector.Draw();

            if (confirmSelection)
            {
                confirmObjectLabel.SetText(ConfigFile.GetLocalizedString("Select_Confirm"));
                confirmObjectLabel.SetActive(true);
            }
            else
                confirmObjectLabel.SetActive(false);
        }

        public static void ScrollUp()
        {
            if (!confirmSelection)
                settingSelector.ScrollUp();
        }

        public static void ScrollDown()
        {
            if (!confirmSelection)
                settingSelector.ScrollDown();
        }

        public static void Cancel()
        {
            confirmSelection = false;
        }

        public static DialogResult Select()
        {
            DialogResult outResult = DialogResult.NoAction;
            if (confirmSelection)
                outResult = DialogResult.Confirm;

            confirmSelection ^= true;
            
            return outResult;
        }

        public static Language GetLanguage()
        {
            Language outLang = Language.None;
            switch (settingSelector.Select())
            {
                case 0: outLang = Language.English; break;
                case 1: outLang = Language.Spanish; break;
                case 2: outLang = Language.Japanese; break;
                case 3: outLang = Language.French; break;
            }

            return outLang;
        }
    }
}