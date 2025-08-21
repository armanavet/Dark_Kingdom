using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutlineSelection : MonoBehaviour
{
    private Outline outlineComp;

    private void Start()
    {
        outlineComp = GetComponent<Outline>();
    }

    private void OnMouseEnter()
    {
        outlineComp.enabled = true;
    }

    private void OnMouseExit()
    {
        outlineComp.enabled = false;
    }
}
