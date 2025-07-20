using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mage : MonoBehaviour
{
    Tower target;
    float speed;
    float progress = 0;
    float damage;
    Vector3 startPoint;
    Vector3 endPoint;
    public LayerMask TowerMask;
    bool cxival;

    private void Update()
    {
        transform.position = Vector3.Lerp(startPoint, endPoint, progress);
        progress += speed * Time.deltaTime;
        if (progress >= 1 && !cxival)
        {
            cxival = true;
            if (target != null)
                target.ApplyDamage(damage);
            //Destroy(gameObject);
        }
    }
    public virtual void Initialize(float speed, Vector3 startPoint, Vector3 endPoint, Tower target, float damage)
    {
        this.speed = speed;
        this.startPoint = startPoint;
        this.endPoint = target.transform.position;
        this.target = target;
        this.damage = damage;
    }
    //private void OnCollisionEnter(Collision collision)
    //{
    //    if(collision.gameObject.layer!=6)
    //     Destroy(gameObject);
    //}

}
