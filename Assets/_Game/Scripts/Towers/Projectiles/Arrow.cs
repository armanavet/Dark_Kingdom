using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Arrow : MonoBehaviour
{
    float speed;

    private void Update()
    {
        transform.position += transform.forward * Time.deltaTime * speed;
    }
    public void Initialize(float speed)
    {
        this.speed = speed;
    }
}