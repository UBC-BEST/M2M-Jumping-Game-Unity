using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : Platformer
{
    public float moveSpeed = 0.5f;
    public float range = 1f;
    
    private float startX;
    
    void Start()
    {
        startX = transform.position.x;
    }
    
    void Update()
    {
        float newX = startX + Mathf.PingPong(Time.time * moveSpeed, range * 2) - range;
        transform.position = new Vector3(newX, transform.position.y, transform.position.z);
    }
}
