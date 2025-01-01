using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TW.UGUI.Core.Screens;
using TW.UGUI.Shared;
using UnityEngine;

public class PushEnterAnim : TransitionAnimationBehaviour
{
    public CanvasGroup canvasGroup;
    public override void Setup()
    {
    }

    public override UniTask PlayAsync(IProgress<float> progress = null)
    {
        canvasGroup.alpha = 0;
        canvasGroup.DOFade(1, 0.15f);
        return default;
    }

    public override void Play(IProgress<float> progress = null)
    {
    }

    public override void Stop()
    {
    }

    public override float TotalDuration { get; }
}
