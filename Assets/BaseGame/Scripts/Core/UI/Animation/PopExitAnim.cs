using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TW.UGUI.Shared;
using UnityEngine;

public class PopExitAnim : TransitionAnimationBehaviour
{
    public CanvasGroup canvasGroup;
    public override void Setup()
    {
       Debug.Log("set up");
    }

    public override async UniTask PlayAsync(IProgress<float> progress = null)
    {
        canvasGroup.alpha = 1;
        await canvasGroup.DOFade(0, 0.15f).AsyncWaitForCompletion();
    }

    public override void Play(IProgress<float> progress = null)
    {
        Debug.Log("play anmimation");
    }

    public override void Stop()
    {
        Debug.Log("stop anmimation");
    }

    public override float TotalDuration { get; }
}
