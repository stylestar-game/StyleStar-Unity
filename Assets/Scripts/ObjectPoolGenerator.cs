using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Pools
{
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
}

public class ObjectPoolGenerator : MonoBehaviour
{
    public GameObject LeftStepBase;
    public GameObject RightStepBase;
    public GameObject LeftShuffleBase;
    public GameObject RightShuffleBase;
    public GameObject LeftHoldBase;
    public GameObject RightHoldBase;
    public GameObject LeftSlideBase;
    public GameObject RightSlideBase;
    public GameObject MotionDownBase;
    public GameObject MotionUpBase;
    public GameObject BeatMarkerBase;
    public int AmountToPool;
    public bool CanExpand;

    private void Start()
    {
        Pools.LeftStep = ScriptableObject.CreateInstance<ObjectPooler>();
        Pools.LeftStep.SetPool(LeftStepBase, AmountToPool, CanExpand);

        Pools.RightStep = ScriptableObject.CreateInstance<ObjectPooler>();
        Pools.RightStep.SetPool(RightStepBase, AmountToPool, CanExpand);

        Pools.LeftShuffle = ScriptableObject.CreateInstance<ObjectPooler>();
        Pools.LeftShuffle.SetPool(LeftShuffleBase, AmountToPool, CanExpand);

        Pools.RightShuffle = ScriptableObject.CreateInstance<ObjectPooler>();
        Pools.RightShuffle.SetPool(RightShuffleBase, AmountToPool, CanExpand);

        Pools.LeftHold = ScriptableObject.CreateInstance<ObjectPooler>();
        Pools.LeftHold.SetPool(LeftHoldBase, AmountToPool, CanExpand);

        Pools.RightHold = ScriptableObject.CreateInstance<ObjectPooler>();
        Pools.RightHold.SetPool(RightHoldBase, AmountToPool, CanExpand);

        Pools.LeftSlide = ScriptableObject.CreateInstance<ObjectPooler>();
        Pools.LeftSlide.SetPool(LeftSlideBase, AmountToPool, CanExpand);

        Pools.RightSlide = ScriptableObject.CreateInstance<ObjectPooler>();
        Pools.RightSlide.SetPool(RightSlideBase, AmountToPool, CanExpand);

        Pools.MotionDown = ScriptableObject.CreateInstance<ObjectPooler>();
        Pools.MotionDown.SetPool(MotionDownBase, AmountToPool, CanExpand);

        Pools.MotionUp = ScriptableObject.CreateInstance<ObjectPooler>();
        Pools.MotionUp.SetPool(MotionUpBase, AmountToPool, CanExpand);

        Pools.BeatMarker = ScriptableObject.CreateInstance<ObjectPooler>();
        Pools.BeatMarker.SetPool(BeatMarkerBase, AmountToPool, CanExpand);
    }
}
