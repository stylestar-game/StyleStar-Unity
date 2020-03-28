using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Pools
{
    // Gameplay Screen
    public static ObjectPooler LeftStep;
    public static ObjectPooler RightStep;
    public static ObjectPooler LeftShuffle;
    public static ObjectPooler RightShuffle;
    public static ObjectPooler LeftHold;
    public static ObjectPooler RightHold;
    public static ObjectPooler LeftSlide;
    public static ObjectPooler RightSlide;
    public static ObjectPooler MotionDown;
    public static ObjectPooler MotionUp;

    public static ObjectPooler BeatMarker;

    public static ObjectPooler FootMarker;
    public static ObjectPooler FootBg;

    // Song Selection
    public static ObjectPooler SongCards;
    public static ObjectPooler FolderCards;

    // Game Settings
    public static ObjectPooler GameSettingOptions;
}
