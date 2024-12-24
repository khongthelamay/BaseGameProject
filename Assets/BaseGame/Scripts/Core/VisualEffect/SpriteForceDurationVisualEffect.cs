using System.Threading;
using Core;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

public class SpriteForceDurationVisualEffect : VisualEffect
{
    [System.Serializable]
    public class SpriteLayer
    {
        [field: SerializeField] public SpriteRenderer SpriteRenderer { get; private set; }
        [field: SerializeField] public Sprite[] StarSpriteList { get; private set; }
        [field: SerializeField] public Sprite[] EndSpriteList { get; private set; }
        public int StartSpriteCount => StarSpriteList.Length;
        public int EndSpriteCount => EndSpriteList.Length;

        public void UpdateStartSprite(int index)
        {
            if (index >= StartSpriteCount) return;
            SpriteRenderer.sprite = StarSpriteList[index];
        }
        public void UpdateEndSprite(int index)
        {
            if (index >= EndSpriteCount) return;
            SpriteRenderer.sprite = EndSpriteList[index];
        }
        public void ResetSprite()
        {
            SpriteRenderer.sprite = null;
        }
    }
    [field: SerializeField] public float Duration {get; private set;}
    [field: SerializeField] public float Speed { get; private set; } = 1;
    [field: SerializeField] public int Sample { get; private set; } = 30;
    [field: SerializeField] public AxisDetection<SpriteLayer[]> Sprite {get; private set;}
    private CancellationTokenSource CancellationTokenSource { get; set; }
    public override VisualEffect WithDuration(float duration)
    {
        Duration = duration;
        return this;
    }
    public override VisualEffect WithSpeed(float speed)
    {
        Speed = speed;
        return base.WithSpeed(speed);
    }

    public override VisualEffect WithAxis(int axis)
    {
        Sprite.Axis = axis;
        return base.WithAxis(axis);
    }

    public override void OnDespawn()
    {
        ResetSprite();
        base.OnDespawn();
    }


    public override VisualEffect Play()
    {
        StartUpdateSprite().Forget();
        return base.Play();
    }

    public override VisualEffect Stop()
    {
        CancellationTokenSource?.Cancel();
        CancellationTokenSource?.Dispose();
        CancellationTokenSource = null;
        return base.Stop();
    }

    private async UniTask StartUpdateSprite()
    {
        CancellationTokenSource?.Cancel();
        CancellationTokenSource?.Dispose();
        CancellationTokenSource = null;
        CancellationTokenSource = new CancellationTokenSource();
        
        SpriteLayer[] currentSpriteLayers = Sprite.Current;
        
        int waitTime = (int)(1000 / Speed / Sample);
        int startSpriteCount = GetStartSpriteCount(currentSpriteLayers);
        int endSpriteCount = GetEndSpriteCount(currentSpriteLayers);
        int holdDuration = (int)(Duration * 1000 / Speed - startSpriteCount * waitTime - endSpriteCount * waitTime);
        for (int i = 0; i < startSpriteCount; i++)
        {
            foreach (SpriteLayer spriteLayer in currentSpriteLayers)
            {
                spriteLayer.UpdateStartSprite(i);
            }
            await UniTask.Delay(waitTime, cancellationToken: CancellationTokenSource.Token);
        }

        if (holdDuration > 0)
        {
            await UniTask.Delay(holdDuration, cancellationToken: CancellationTokenSource.Token);
        }
        
        for (int i = 0; i < endSpriteCount; i++)
        {
            foreach (SpriteLayer spriteLayer in currentSpriteLayers)
            {
                spriteLayer.UpdateEndSprite(i);
            }
            await UniTask.Delay(waitTime, cancellationToken: CancellationTokenSource.Token);
        }
        OnDespawn();
    }

    private int GetStartSpriteCount(SpriteLayer[] currentSpriteLayers)
    {
        int maxSpriteCount = 0;
        foreach (SpriteLayer spriteLayer in currentSpriteLayers)
        {
            maxSpriteCount = Mathf.Max(maxSpriteCount, spriteLayer.StartSpriteCount);
        }

        return maxSpriteCount;
    }
    private int GetEndSpriteCount(SpriteLayer[] currentSpriteLayers)
    {
        int maxSpriteCount = 0;
        foreach (SpriteLayer spriteLayer in currentSpriteLayers)
        {
            maxSpriteCount = Mathf.Max(maxSpriteCount, spriteLayer.EndSpriteCount);
        }
        return maxSpriteCount;
    }
    private void ResetSprite()
    {
        foreach (SpriteLayer spriteLayer in Sprite.Horizontal)
        {
            spriteLayer.ResetSprite();
        }
        foreach (SpriteLayer spriteLayer in Sprite.Vertical)
        {
            spriteLayer.ResetSprite();
        }
        foreach (SpriteLayer spriteLayer in Sprite.NoneAxis)
        {
            spriteLayer.ResetSprite();
        }
    }
}