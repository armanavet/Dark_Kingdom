using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    float speed;
    float progress = 0;
    float damage;
    Vector3 startPoint;
    Enemy target;
    public LayerMask EnemyMask;
    private void Update()
    {
        transform.position = Vector3.Lerp(startPoint, target.transform.position, progress);
        progress += speed * Time.deltaTime;
        if (progress >= 1)
        {
            //target.ApplyDamage(damage);
            Destroy(gameObject);

        }
    }
    public void Initialize(float speed,Vector3 startPoint,Enemy target, float damage)
    {
        this.speed = speed;
        this.startPoint = startPoint;
        this.target = target;
        this.damage= damage;
    }
    
}