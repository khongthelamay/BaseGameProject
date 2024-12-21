using System.Linq;
using Core;
using Core.SimplePool;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

public class SpriteVisualEffect : VisualEffect
{
    [System.Serializable]
    public class SpriteLayer
    {
        [field: SerializeField] public SpriteRenderer SpriteRenderer { get; private set; }
        [field: SerializeField] public Sprite[] SpriteList { get; private set; }
        public int SpriteCount => SpriteList.Length;
        
        public void UpdateSprite(int index)
        {
            if (index >= SpriteCount) return;
            SpriteRenderer.sprite = SpriteList[index];
        }
        public void ResetSprite()
        {
            SpriteRenderer.sprite = null;
        }
    }
    
    [field: SerializeField] public float Speed {get; private set;} = 1;
    [field: SerializeField] public int Sample { get; private set; } = 30;
    [field: SerializeField] public AxisDetection<SpriteLayer[]> Sprite {get; private set;}
    public override VisualEffect Play()
    {
        StartUpdateSprite().Forget();
        return base.Play();
    }

    public override void OnDespawn()
    {
        ResetSprite();
        base.OnDespawn();
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

    private async UniTask StartUpdateSprite()
    {
        SpriteLayer[] currentSpriteLayers = Sprite.Current;
        int waitTime = (int)(1000 / Speed / Sample);
        int spriteCount = GetSpriteCount(currentSpriteLayers);
        for (int i = 0; i < spriteCount; i++)
        {
            foreach (SpriteLayer spriteLayer in currentSpriteLayers)
            {
                spriteLayer.UpdateSprite(i);
            }
            await UniTask.Delay(waitTime, cancellationToken: this.GetCancellationTokenOnDestroy());
        }
        this.Despawn();
    }

    private int GetSpriteCount(SpriteLayer[] currentSpriteLayers)
    {
        int maxSpriteCount = 0;
        foreach (SpriteLayer spriteLayer in currentSpriteLayers)
        {
            maxSpriteCount = Mathf.Max(maxSpriteCount, spriteLayer.SpriteCount);
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