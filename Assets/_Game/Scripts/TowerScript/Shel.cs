using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shel : MonoBehaviour
{
    Vector3 launchPoint, targetPoint, launchVelocity;
    float age,blastRadius,Damage;
    // Start is called before the first frame update
    public LayerMask EnemyMask;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        age += Time.deltaTime;
        Vector3 p=launchPoint+launchVelocity*age;
        p.y -= 0.5f * 9.81f * age * age;
        transform.position = p;
        Vector3 d = launchVelocity;
        d.y -= 9.81f * age;
        transform.localRotation= Quaternion.LookRotation(d);
        if (transform.position.y < 0f)
        {
           Explode();
        }
    }
    public void Initialize(Vector3 launchpoint,Vector3 targetpoint,Vector3 launchvelocity,float blastradius,float damage)
    {
        launchPoint= launchpoint;
        targetPoint = targetpoint;
        launchVelocity = launchvelocity;
        blastRadius= blastradius;
        Damage = damage;
    }
    void Explode()
    {
        Collider[] targets=Physics.OverlapSphere(transform.position, blastRadius,EnemyMask);
        if (targets.Length > 0 )
        {
            for (int i = 0; i < targets.Length; i++)
            {
                
            }
        }
        Destroy(gameObject);
    }
}
