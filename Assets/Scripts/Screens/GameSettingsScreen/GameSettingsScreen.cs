using System;
using System.Collections.Generic;
using UnityEngine;

namespace StyleStar
{
    public static class GameSettingsScreen
    {
        //private static List<Label> fixedLabels = new List<Label>();
        private static List<SettingOption> options = null;
        //private static Label confirmLabel;
        //private static Label rejectLabel;

        
        //private static Vector2 optionsPoint = new Vector2(390, 56);

        private static float fontTitleHeight = 25.0f;
        private static float fontOptionHeight = 18.0f;

        private static GameObject selectionObj;
        private static GameObject confirmRejectLabel;
        private static Vector3 selectionPoint = new Vector3(127, 309);
        private static Vector3 selectionOffset = new Vector3(-47, -81);

        private static int selectedCategory = 0;

        private static bool confirmSelection = false;

        private static Language selectedLanguage = Language.English;

        private const int LANGUAGE_SETTING = 0;
        private const int RESOLUTION_SETTING = 1;
        private const int ORIENTATION_SETTING = 2;
        private const int FREE_PLAY_SETTING = 3;
        private const int AUTO_MODE_SETTING = 4;
        private const int CONFIRM_EXIT = 5;

        private const int OPEN_LANGUAGE_DIALOG = 1;

        public static void Initialize(GameObject selectObj, GameObject confirmReject)
        {
            // These are destroyed each time the scene is unloaded
            selectionObj = selectObj;
            confirmRejectLabel = confirmReject;

            if (options != null)
                options.Clear(); // clear data if there's anything in here from last time

            options = new List<SettingOption>() {
                new SettingOption(ConfigFile.GetLocalizedString("Lang_Title"), new string[] { ConfigFile.GetLocalizedString("Lang_Name"),
                    ConfigFile.GetLocalizedString("Lang_Open") }),
                new SettingOption(ConfigFile.GetLocalizedString("Screen_Res"), new string[] { "640x360", "1280x720", "1920x1080" }),
                new SettingOption(ConfigFile.GetLocalizedString("Touch_Res"), new string[] {
                    ConfigFile.GetLocalizedString("Hor_0"), ConfigFile.GetLocalizedString("Ver_90"),
                    ConfigFile.GetLocalizedString("Hor_180"), ConfigFile.GetLocalizedString("Ver_270")
                }),
                new SettingOption(ConfigFile.GetLocalizedString("En_Free"), new string[] { ConfigFile.GetLocalizedString("On"), ConfigFile.GetLocalizedString("Off") }),
                new SettingOption(ConfigFile.GetLocalizedString("Auto_Mode_2"), new string[] { ConfigFile.GetLocalizedString("Off"),
                    ConfigFile.GetLocalizedString("On"), ConfigFile.GetLocalizedString("Down_Only") }),
                new SettingOption("", new string[] { ConfigFile.GetLocalizedString("Save_Exit"), ConfigFile.GetLocalizedString("Leave_No_Save") })
           };
        }

        public static void Draw()
        {
            for (int i = 0; i < options.Count; i++)
            {
                options[i].Draw(i);
            }

            selectionObj.transform.localPosition = selectionPoint + selectedCategory * selectionOffset;

            if (confirmSelection)
            {
                if (options[CONFIRM_EXIT].SelectedOption == 0)
                    confirmRejectLabel.SetText(ConfigFile.GetLocalizedString("Select_Confirm"));
                else
                    confirmRejectLabel.SetText(ConfigFile.GetLocalizedString("Select_Discard"));

                confirmRejectLabel.SetActive(true);
            }
            else
                confirmRejectLabel.SetActive(false);
        }

        public static void ScrollUp()
        {
            if (confirmSelection)
                return;

            selectedCategory--;
            if (selectedCategory < 0)
                selectedCategory = options.Count - 1;
        }

        public static void ScrollDown()
        {
            if (confirmSelection)
                return;

            selectedCategory++;
            if (selectedCategory >= options.Count)
                selectedCategory = 0;
        }

        public static void ScrollRight()
        {
            if (confirmSelection)
                return;

            options[selectedCategory].ScrollRight();
        }

        public static void ScrollLeft()
        {
            if (confirmSelection)
            {
                confirmSelection = false;
                return;
            }

            options[selectedCategory].ScrollLeft();
        }

        public static DialogResult Select()
        {
            if (confirmSelection)
            {
                confirmSelection = false;
                // Config is saved in the main thread
                return options[CONFIRM_EXIT].SelectedOption == 0 ? DialogResult.Confirm : DialogResult.Cancel;
            }
            else if (selectedCategory == CONFIRM_EXIT)
                confirmSelection = true;
            else if (IsLanguageDialogSelected())
            {
                LanguageSelect.NextGameMode = Mode.GameSettings;
                return DialogResult.Confirm;
            }

            return DialogResult.NoAction;
        }

        public static bool IsLanguageDialogSelected()
        {
            return selectedCategory == LANGUAGE_SETTING && options[LANGUAGE_SETTING].SelectedOption == OPEN_LANGUAGE_DIALOG;
        }

        public static void SetConfig(Dictionary<string, object> config)
        {
            foreach (var entry in config)
            {
                switch (entry.Key)
                {
                    case ConfigKeys.Language:
                        selectedLanguage = (Language)Convert.ToInt32(entry.Value);
                        break;
                    case ConfigKeys.Resolution:
                        options[RESOLUTION_SETTING].SelectedOption = Convert.ToInt32(entry.Value);
                        break;
                    case ConfigKeys.TouchScreenOrientation:
                        options[ORIENTATION_SETTING].SelectedOption = Convert.ToInt32(entry.Value);
                        break;
                    case ConfigKeys.EnableFreePlay:
                        options[FREE_PLAY_SETTING].SelectedOption = Convert.ToBoolean(entry.Value) ? 0 : 1;
                        break;
                    case ConfigKeys.AutoMode:
                        options[AUTO_MODE_SETTING].SelectedOption = Convert.ToInt32(entry.Value);
                        break;
                    default:
                        break;
                }
            }
        }

        public static Dictionary<string, object> GetConfig()
        {
            return new Dictionary<string, object>()
            {
                {ConfigKeys.Language, (int)ConfigFile.LocalizedLanguage},
                {ConfigKeys.Resolution,options[1].SelectedOption },
                {ConfigKeys.TouchScreenOrientation, options[2].SelectedOption },
                {ConfigKeys.EnableFreePlay, options[3].SelectedOption == 0 ? true : false },
                {ConfigKeys.AutoMode, options[4].SelectedOption }
            };
        }

        public enum LabelField
        {
            StylishCount,
            CoolCount,
            GoodCount,
            MissCount,
            Score,
            Result,
            Title,
            Artist,
            Level
        }

        public enum Resolution
        {
            _640x360,
            _1280x720,
            _1920x1080
        }

        public enum AutoMode
        {
            Off,
            Auto,
            AutoDown
        }

        public static class ConfigKeys
        {
            public const string Language = "Language";
            public const string Resolution = "Resolution";
            public const string TouchScreenOrientation = "TouchScreenOrientation";
            public const string EnableFreePlay = "EnableFreePlay";
            public const string StageNumber = "StageNumber";
            public const string AutoMode = "AutoMode";
        }
    }
}
