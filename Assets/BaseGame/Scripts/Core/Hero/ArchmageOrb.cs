using Core.SimplePool;
using LitMotion;
using TW.ACacheEverything;
using TW.Utility.CustomComponent;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Core
{
    public partial class ArchmageOrb : ACachedMonoBehaviour, IPoolAble<ArchmageOrb>
    {
        [field: SerializeField] public Hero Hero {get; private set;}
        [field: SerializeField] public SpriteRenderer MainGraphic {get; private set;}
        [field: SerializeField] public int State {get; private set;}
        [field: SerializeField] public AnimationCurve XCurve {get; private set;}
        [field: SerializeField] public AnimationCurve YCurve {get; private set;}
        
        private float Noise { get; set; }
        private Vector3 CurrentPosition { get; set; }
        private MotionHandle MoveAroundHandle  { get; set; }
        public ArchmageOrb Setup(Hero hero)
        {
            Hero = hero;
            State = 0;
            MainGraphic.sortingOrder = 20;
            Noise = Random.Range(-0.1f, 0.1f);
            return this;
        }

        public ArchmageOrb MovingAround()
        {
            MoveAroundHandle = LMotion.Create(0f, 1f, 2f)
                .WithEase(Ease.Linear)
                .WithLoops(-1)
                .Bind(OnMovingAroundUpdateCache);
            
            return this;
        }
        public ArchmageOrb Consumed()
        {
            CurrentPosition = Transform.localPosition;
            MoveAroundHandle.TryCancel();
            MoveAroundHandle = LMotion.Create(0f, 1f, Random.Range(0.1f, 0.3f))
                .WithEase(Ease.Linear)
                .WithOnComplete(OnConsumedCompleteCache)
                .Bind(OnConsumedUpdateCache);
            return this;
        }
        [ACacheMethod]
        private void OnMovingAroundUpdate(float value)
        {
            Transform.localPosition = new Vector3(XCurve.Evaluate(value), YCurve.Evaluate(value) + Noise);
            if (value > 0.5f && State == 0)
            {
                SetState(1);
            }
            else if (value < 0.5f && State == 1)
            {
                SetState(0);
            }
        }
        [ACacheMethod]
        private void OnConsumedUpdate(float value)
        {
            Transform.localPosition = Vector3.Lerp(CurrentPosition, Vector3.zero, value);
        }
        [ACacheMethod]
        private void OnConsumedComplete()
        {
            this.Despawn();
        }
        private void SetState(int value)
        {
            State = value;
            MainGraphic.sortingOrder = State == 0 ? 20 : -20;
        }
        public ArchmageOrb OnSpawn()
        {
            return this;
        }
        public void OnDespawn()
        {
            MoveAroundHandle.TryCancel();
        }
    }
}