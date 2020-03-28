using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayPooler : MonoBehaviour
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
    public GameObject FootMarkerBase;
    public GameObject FootBgBase;
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

        Pools.FootMarker = ScriptableObject.CreateInstance<ObjectPooler>();
        Pools.FootMarker.SetPool(FootMarkerBase, AmountToPool, CanExpand);

        Pools.FootBg = ScriptableObject.CreateInstance<ObjectPooler>();
        Pools.FootBg.SetPool(FootBgBase, AmountToPool, CanExpand);
    }
}