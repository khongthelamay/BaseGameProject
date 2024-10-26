using TW.Utility.DesignPattern;
using UnityEngine;
using UnityEngine.Pool;

namespace Core.SimplePool
{
    // simple pool for mono behaviour will be implemented in the future
    public interface IMonoPoolingObject<out T> : IPoolAble<T> where T : Component 
    {
        T Spawn(Vector3 position, Quaternion rotation)
        {
            return TW.Utility.DesignPattern.SimplePool.Spawn(this as T, position, rotation);
        }
        T Spawn(Transform parent)
        {
            return TW.Utility.DesignPattern.SimplePool.Spawn(this as T, parent);
        }
        T Spawn()
        {
            return TW.Utility.DesignPattern.SimplePool.Spawn(this as T);
        }

        T Spawn(Vector3 position, Quaternion rotation, Transform parent)
        {
            return TW.Utility.DesignPattern.SimplePool.Spawn(this as T, position, rotation, parent);
        }
        
        void Despawn()
        {
            TW.Utility.DesignPattern.SimplePool.DeSpawn(((T)this).gameObject);
        }
    }

    public interface IPoolAble<out T>
    {
        T Spawn(Vector3 position, Quaternion rotation);
        T Spawn(Transform parent);
        T Spawn();
        T Spawn(Vector3 position, Quaternion rotation, Transform parent);
        void Despawn();
    }
    public class NewBehaviourScript : MonoBehaviour, IMonoPoolingObject<NewBehaviourScript>
    {
        public void Init()
        {
            Debug.Log("Init");
        }

        public NewBehaviourScript Spawn(Vector3 position, Quaternion rotation)
        {
            throw new System.NotImplementedException();
        }

        public NewBehaviourScript Spawn(Transform parent)
        {
            throw new System.NotImplementedException();
        }

        public NewBehaviourScript Spawn()
        {
            throw new System.NotImplementedException();
        }

        public NewBehaviourScript Spawn(Vector3 position, Quaternion rotation, Transform parent)
        {
            throw new System.NotImplementedException();
        }

        public void Despawn()
        {
            throw new System.NotImplementedException();
        }
    }
    public class SimplePool : MonoBehaviour
    {
        private void Start()
        {
            NewBehaviourScript newBehaviourScript = new NewBehaviourScript();
            newBehaviourScript.Spawn();
        }
    }
}