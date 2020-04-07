using System;
using System.Collections.Generic;
using UnityEngine;

namespace StyleStar
{
    public static class GameSettingsScreen
    {
        //private static List<Label> fixedLabels = new List<Label>();
        private static List<SettingOption> options;
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

        private static bool isInitialized = false;

        public static void Initialize(GameObject selectObj, GameObject confirmReject)
        {
            // These are destroyed each time the scene is unloaded
            selectionObj = selectObj;
            confirmRejectLabel = confirmReject;

            // Only needs to be loaded once
            if (!isInitialized)
            {
                options = new List<SettingOption>() {
                    new SettingOption("LANGUAGE", new string[] { "English", "Open language select..." }),
                    new SettingOption("SCREEN RESOLUTION", new string[] { "640x360", "1280x720", "1920x1080" }),
                    new SettingOption("TOUCH SCREEN ORIENTATION", new string[] { "Horizontal (0°)", "Vertical (90°)", "Horizontal (180°)", "Vertical (270°)" }),
                    new SettingOption("ENABLE FREE PLAY", new string[] { "ON", "OFF" }),
                    new SettingOption("AUTO MODE", new string[] { "OFF", "ON", "DOWN ONLY" }),
                    new SettingOption("", new string[] { "Save and Exit", "Leave without Saving" })
                };
                isInitialized = true;
            }
            else
            {
                // We need to do this because the pool gets deleted every time we exit from this menu.
                foreach (var option in options)
                    option.InitializeOptionObj();
            }
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
                if (options[5].SelectedOption == 0)
                    confirmRejectLabel.SetText("Press SELECT again to confirm changes.");
                else
                    confirmRejectLabel.SetText("Press SELECT again to discard changes.");

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
                return options[5].SelectedOption == 0 ? DialogResult.Confirm : DialogResult.Cancel;
            }
            else
                confirmSelection = true;

            return DialogResult.NoAction;
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
                        options[1].SelectedOption = Convert.ToInt32(entry.Value);
                        break;
                    case ConfigKeys.TouchScreenOrientation:
                        options[2].SelectedOption = Convert.ToInt32(entry.Value);
                        break;
                    case ConfigKeys.EnableFreePlay:
                        options[3].SelectedOption = Convert.ToBoolean(entry.Value) ? 0 : 1;
                        break;
                    case ConfigKeys.AutoMode:
                        options[4].SelectedOption = Convert.ToInt32(entry.Value);
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
                {ConfigKeys.Language, (int)selectedLanguage},
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

        public enum Language
        {
            English,
            Japanese
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
