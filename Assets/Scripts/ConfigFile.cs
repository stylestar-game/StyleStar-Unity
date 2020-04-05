using Nett;
using StyleStar;
using System;
using System.Collections.Generic;
using System.IO;

public static class ConfigFile
{
    public static string FilePath { get; set; } = Defines.ConfigFile;
    public static bool IsLoaded { get; private set; } = false;

    private static Dictionary<string, object> configTable;

    private static void CreateConfigFile()
    {
        // TODO: Make magic numbers for configs into constants defined in this class
        TomlTable tomlConfigTable = Toml.Create();

        Dictionary<string, object> keyConfigDict = new Dictionary<string, object>();
        keyConfigDict.Add("Left", 37);
        keyConfigDict.Add("Right", 39);
        keyConfigDict.Add("Up", 38);
        keyConfigDict.Add("Down", 40);
        keyConfigDict.Add("Exit", 115);
        keyConfigDict.Add("Back", 27);
        keyConfigDict.Add("Back2", 8);
        keyConfigDict.Add("Auto", 113);
        keyConfigDict.Add("Select", 13);
        keyConfigDict.Add("GameOptions", 112);
        tomlConfigTable.Add("KeyConfig", keyConfigDict);

        Dictionary<string, object> touchConfigDict = new Dictionary<string, object>();
        touchConfigDict.Add("WidthAxis", 1);
        touchConfigDict.Add("CalMinX", 0);
        touchConfigDict.Add("CalMaxX", 1280);
        touchConfigDict.Add("CalMinY", 0);
        touchConfigDict.Add("CalMaxY", 720);
        touchConfigDict.Add("AbsX", 1024);
        touchConfigDict.Add("AbsY", 1024);
        tomlConfigTable.Add("TouchConfig", touchConfigDict);

        Dictionary<string, object> gameConfigDict = new Dictionary<string, object>();
        gameConfigDict.Add("Language", 0);
        gameConfigDict.Add("Resolution", 0);
        gameConfigDict.Add("TouchScreenOrientation", 1);
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

        //if (configTable.ContainsKey(Defines.KeyConfig))
        //    InputMonitor.SetKeys((Dictionary<string, object>)configTable[Defines.KeyConfig]);
        if (configTable.ContainsKey(Defines.TouchConfig))
            TouchSettings.SetConfig((Dictionary<string, object>)configTable[Defines.TouchConfig]);
        //if (configTable.ContainsKey(Defines.GameConfig))
        //    GameSettingsScreen.SetConfig((Dictionary<string, object>)configTable[Defines.GameConfig]);

        IsLoaded = true;
        return true;
    }

    public static void Update()
    {
        configTable = new Dictionary<string, object>()
            {
                //{Defines.KeyConfig, InputMonitor.GetConfig() },
                {Defines.TouchConfig, TouchSettings.GetConfig() },
                //{Defines.GameConfig, GameSettingsScreen.GetConfig() }
            };
    }

    public static void Save(string filepath = "")
    {
        if (!String.IsNullOrEmpty(filepath))
            FilePath = filepath;

        var data = new Dictionary<string, object>()
            {
                //{Defines.KeyConfig, InputMonitor.GetConfig() },
                {Defines.TouchConfig, TouchSettings.GetConfig() },
                //{Defines.GameConfig, GameSettingsScreen.GetConfig() }
            };
        Toml.WriteFile(data, Defines.ConfigFile);

        // Should this go here? I don't see why we would save if we didn't want to apply it immediately
        //UpdateGlobals();
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