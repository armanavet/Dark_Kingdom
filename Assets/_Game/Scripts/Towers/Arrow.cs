using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    float speed;
    float progress = 0;
    float damage;
    Vector3 startPoint;
    Vector3 endPoint;
    public LayerMask EnemyMask;
    private void Update()
    {
        transform.position = Vector3.Lerp(startPoint, endPoint, progress);
        progress += speed * Time.deltaTime;
        if (progress >= 1)
        {
            Explode();
        }
    }
    public void Initialize(float speed,Vector3 position,Vector3 target, float damage)
    {
        this.speed = speed;
        startPoint = position;
        endPoint = target;
        this.damage= damage;
    }
    void Explode()
    {
        Collider[] targets = Physics.OverlapSphere(transform.position, EnemyMask);
        if (targets.Length > 0)
        {
            for (int i = 0; i < targets.Length; i++)
            {
                targets[i].GetComponent<TargetPoint>().Enemy.ApplyDamage(damage);
            }
        }
        Destroy(gameObject);
    }
    //private void OnCollisionEnter(Collision collision)
    //{
    //    if(collision.gameObject.layer!=6)
    //     Destroy(gameObject);
    //}
}