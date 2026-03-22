using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    [SerializeField] GameObject canvas;
    Camera main;
    private void Start()
    {
        main = Camera.main;
    }
    private void Update()
    {
        canvas.transform.rotation = Quaternion.LookRotation(main.transform.forward);
    }
}
