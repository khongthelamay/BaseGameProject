using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class TestMove : MonoBehaviour
{
    [field: SerializeField] private Transform Point1 {get; set;}
    [field: SerializeField] private Transform Point2 {get; set;}
    [field: SerializeField] public float Alpha {get; private set;}
    [field: SerializeField] private float JumpHeight {get; set;}
    private void Start()
    {

    }

    private void Update()
    {
        Alpha = Mathf.Repeat(Time.time, 1f);
        float x = Mathf.Lerp(Point1.position.x, Point2.position.x, EaseLinear(Alpha));
        float height = Mathf.Max(Point1.position.y, Point2.position.y) + JumpHeight;
        float y = Alpha < 0.5f ? 
            Mathf.Lerp(Point1.position.y, height, EaseOutQuad(Alpha * 2)) : 
            Mathf.Lerp(height, Point2.position.y, EaseInQuad((Alpha - 0.5f) * 2));

        transform.position = new Vector2(x, y);
    }
    
    private float EaseOutQuad(float x)
    {
        return 1f - (1f - x) * (1f - x);
    }

    private float EaseInQuad(float x)
    {
        return x * x;
    }
    private float EaseLinear(float x)
    {
        return x;
    }
}
