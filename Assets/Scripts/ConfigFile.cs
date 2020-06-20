using Nett;
using StyleStar;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class ConfigFile
{
    public static string FilePath { get; set; } = Defines.ConfigFile;
    public static bool IsLoaded { get; private set; } = false;
    public static Language LocalizedLanguage { get; set; } = Language.English;

    private static Dictionary<string, object> configTable;

    private static Dictionary<string, object> localizationTable = null;

    private static string GetLocalizedLanguageString()
    {
        string localizedLang = "";
        switch (LocalizedLanguage)
        {
            case Language.French: localizedLang = "French"; break;
            case Language.Japanese: localizedLang = "Japanese"; break;
            case Language.Spanish: localizedLang = "Spanish"; break;
            case Language.English:
            default: localizedLang = "English"; break;
        }
        return localizedLang;
    }

    private static void SetLocalizedLanguage(int localLangKey)
    {
        switch (localLangKey)
        {
            case 3: LocalizedLanguage = Language.French; break;
            case 2: LocalizedLanguage = Language.Japanese; break;
            case 1: LocalizedLanguage = Language.Spanish; break;
            case 0:
            default: LocalizedLanguage = Language.English; break;
        }
    }

    private static void CreateConfigFile()
    {
        // TODO: Make magic numbers for configs into constants defined in this class
        TomlTable tomlConfigTable = Toml.Create();

        Rect screenRect = Util.GetScreenRect();
        Boolean bEmptyRect = screenRect.width == 0 || screenRect.height == 0;

        Dictionary<string, object> touchConfigDict = new Dictionary<string, object>();
        touchConfigDict.Add("WidthAxis", 0/*along positive x-axis*/);
        touchConfigDict.Add("CalMinX", !bEmptyRect ? screenRect.xMin : 0);
        touchConfigDict.Add("CalMaxX", !bEmptyRect ? screenRect.xMax : 1280);
        touchConfigDict.Add("CalMinY", !bEmptyRect ? screenRect.yMin : 0);
        touchConfigDict.Add("CalMaxY", !bEmptyRect ? screenRect.yMax : 720);
        touchConfigDict.Add("AbsX", !bEmptyRect ? screenRect.width : 1024);
        touchConfigDict.Add("AbsY", !bEmptyRect ? screenRect.height : 1024);
        tomlConfigTable.Add("TouchConfig", touchConfigDict);

        Dictionary<string, object> gameConfigDict = new Dictionary<string, object>();
        gameConfigDict.Add("Language", 0);
        gameConfigDict.Add("Resolution", 0);
        gameConfigDict.Add("TouchScreenOrientation", 0);
        gameConfigDict.Add("EnableFreePlay", true);
        gameConfigDict.Add("AutoMode", 0);
        tomlConfigTable.Add("GameConfig", gameConfigDict);

        Toml.WriteFile(tomlConfigTable, Defines.ConfigFile);
    }

    public static bool Load(string filepath = "")
    {
        if (!String.IsNullOrEmpty(filepath))
            FilePath = filepath;

        // build defaults
        if (!File.Exists(Defines.ConfigFile))
            CreateConfigFile();

        configTable = Toml.ReadFile(Defines.ConfigFile).ToDictionary();

        if (File.Exists(Defines.LanguageConfig))
            localizationTable = Toml.ReadFile(Defines.LanguageConfig).ToDictionary();

        // Set the localized language for the rest of the run, unless changed in options
        if (configTable.ContainsKey(Defines.GameConfig))
        {
            Dictionary<string, object> gameOpts = (Dictionary<string, object>)configTable["GameConfig"];
            if (gameOpts.ContainsKey("Language"))
                SetLocalizedLanguage(Convert.ToInt32(gameOpts["Language"]));
        }

        if (configTable.ContainsKey(Defines.TouchConfig))
            TouchSettings.SetConfig((Dictionary<string, object>)configTable[Defines.TouchConfig]);

        IsLoaded = true;
        return true;
    }

    public static void Update()
    {
        configTable = new Dictionary<string, object>()
            {
                {Defines.TouchConfig, TouchSettings.GetConfig() },
                {Defines.GameConfig, GameSettingsScreen.GetConfig() }
            };
    }

    public static void Save(string filepath = "")
    {
        if (!String.IsNullOrEmpty(filepath))
            FilePath = filepath;

        File.Delete(Defines.ConfigFile); // delete file before writing another TOML file
        Toml.WriteFile(configTable, Defines.ConfigFile);
    }

    //public Dictionary<string, object> this[string key]
    //{
    //    get { return (Dictionary<string, object>)configTable[key]; }
    //    set { configTable[key] = value; }
    //}

    public static Dictionary<string, object> GetTable(string key)
    {
        return (Dictionary<string, object>)configTable[key];
    }

    public static object GetKey(string key)
    {
        foreach (var dict in configTable)
        {
            if (((Dictionary<string, object>)dict.Value).ContainsKey(key))
                return ((Dictionary<string, object>)dict.Value)[key];
        }
        return null;
    }

    public static string GetLocalizedString(string stringKey)
    {
        string outStr = "";
        if (localizationTable != null)
        {
            string localizedLang = GetLocalizedLanguageString();
            if (localizationTable.ContainsKey(localizedLang))
                outStr = ((string)((Dictionary<string, object>)localizationTable[localizedLang])[stringKey]);
        }
        return outStr;
    }

    //public void UpdateGlobals()
    //{
    //    int mode = Convert.ToInt32(GetKey(GameSettingsScreen.ConfigKeys.AutoMode));
    //    Globals.AutoMode = (GameSettingsScreen.AutoMode)mode;
    //}

    //public void ResetGameSettings()
    //{
    //    configTable = Toml.ReadFile(Defines.ConfigFile).ToDictionary();
    //    if (configTable.ContainsKey(Defines.GameConfig))
    //        GameSettingsScreen.SetConfig((Dictionary<string, object>)configTable[Defines.GameConfig]);
    //}
}