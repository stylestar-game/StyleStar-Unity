using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings
{
    public Language UILangauge { get; set; }
    public Resolution ScreenResolution { get; set; }
    public AutoMode AutoSetting { get; set; }


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
}
