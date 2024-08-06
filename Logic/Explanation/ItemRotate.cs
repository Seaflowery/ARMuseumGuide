using System;
using UnityEngine;


public class ItemRotate : MonoBehaviour
{
    private float lastTime = 0, speed = 50f;

    void Start()
    {
        lastTime = Time.time;
    }

    void Update()
    {
        float nowTime = Time.time;
        float dt = nowTime - lastTime;
        lastTime = nowTime;
        transform.Rotate(Vector3.up * speed * dt, Space.Self);
    }
}