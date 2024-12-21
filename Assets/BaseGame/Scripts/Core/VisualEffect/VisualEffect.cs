using Core.SimplePool;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using TW.Utility.CustomComponent;
using TW.Utility.Extension;
using UnityEngine;

namespace Core
{
    public class VisualEffect : ACachedMonoBehaviour, IPoolAble<VisualEffect>
    {
        public virtual VisualEffect Play()
        {
            return this;
        }
        public virtual VisualEffect WithDuration(float duration)
        {
            return this;
        }
        public virtual VisualEffect WithLocalScale(float scale)
        {
            Transform.localScale = Vector3.one * scale;
            return this;
        }
        public virtual VisualEffect WithSpeed(float speed)
        {
            
            return this;
        }
        public virtual VisualEffect WithAxis(int axis)
        {
            return this;
        }
        public virtual VisualEffect OnSpawn()
        {
            return this;
        }

        public virtual void OnDespawn()
        {

        }


    }
}