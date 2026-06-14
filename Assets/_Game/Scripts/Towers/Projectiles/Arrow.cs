using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    float speed;
    
    private void Update()
    {
        if (StateManager.Instance.State == GameState.Paused) return;

        transform.position += transform.forward * Time.deltaTime * speed;
    }
    public void Initialize(float speed)
    {
        this.speed = speed;
    }
}