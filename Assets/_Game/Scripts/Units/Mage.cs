using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mage : MonoBehaviour
{
    float speed;
    private void Update()
    {
        transform.position += transform.forward * Time.deltaTime * speed;

    }
    public virtual void Initialize(float speed)
    {
        this.speed = speed;
    }

}
