using LitMotion;
using R3;
using TW.Reactive.CustomComponent;
using TW.Utility.CustomComponent;
using UnityEngine;

public class Enemy : ACachedMonoBehaviour
{
    [field: SerializeField] private float MovementSpeed { get; set; }
    [field: SerializeField] public Transform[] MovePoint { get; private set; }
    [field: SerializeField] public int CurrentPoint { get; private set; }
    [field: SerializeField] public ReactiveValue<float> PlaybackSpeed {get; private set;}

    private MotionHandle _movementMotionHandle;

    public Enemy SetupMovePoint(Transform[] movePoint)
    {
        MovePoint = movePoint;
        CurrentPoint = 0;
        Transform.position = MovePoint[CurrentPoint].position;
        PlaybackSpeed.Value = 1;
        

        return this;
    }

    public void StartMoveToNextPoint()
    {
        Vector3 currentPosition = MovePoint[CurrentPoint].position;
        CurrentPoint = (CurrentPoint + 1) % MovePoint.Length;
        Vector3 targetPosition = MovePoint[CurrentPoint].position;
        float distance = Vector3.Distance(currentPosition, targetPosition);
        float duration = distance / MovementSpeed;
        
        _movementMotionHandle = LMotion.Create(currentPosition, targetPosition, duration)
            .WithEase(Ease.Linear)
            .WithOnComplete(StartMoveToNextPoint)
            .Bind(OnUpdate);
        
        _movementMotionHandle.PlaybackSpeed = PlaybackSpeed.Value;
        PlaybackSpeed.ReactiveProperty.Subscribe(OnPlaybackSpeedChanged).AddTo(this);
    }

    private void OnUpdate(Vector3 aa)
    {
        Transform.position = aa;
    }
    private void OnPlaybackSpeedChanged(float speed)
    {
        // if (!_movementMotionHandle.IsActive()) return;
        _movementMotionHandle.PlaybackSpeed = speed;
    }
}