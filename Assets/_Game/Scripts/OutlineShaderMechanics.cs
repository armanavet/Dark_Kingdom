using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class OutlineShaderMechanics : MonoBehaviour
{
    private void Start()
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag("a");
        foreach (GameObject obj in objects)
        {
            if (obj.GetComponent<Outline>() == null)
            {
                obj.AddComponent<Outline>();
                obj.GetComponent<Outline>().enabled = false;
                obj.GetComponent<Outline>().OutlineColor = Color.white;
                obj.GetComponent<Outline>().OutlineWidth = 8f;
            }
            if(obj.GetComponent<OutlineSelection>() == null)
            {
                obj.AddComponent<OutlineSelection>();
            }
            if(obj.GetComponent<BoxCollider>() == null)
            {
                obj.AddComponent<BoxCollider>();
            }
        }
    }
    
}
