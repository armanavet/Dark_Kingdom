using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    float speed;
    float progress = 0;
    Vector3 startPoint;
    Vector3 endPoint;
    private void Update()
    {
        transform.position = Vector3.Lerp(startPoint, endPoint, progress);
        progress += speed * Time.deltaTime;
        if (progress >= 1)
            Destroy(gameObject);
    }
    public void Initialize(float speed,Vector3 position,Vector3 target)
    {
        this.speed = speed;
        startPoint = position;
        endPoint = target;
    }
    //private void OnCollisionEnter(Collision collision)
    //{
    //    if(collision.gameObject.layer!=6)
    //     Destroy(gameObject);
    //}
}