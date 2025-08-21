using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectEmission : MonoBehaviour
{
    private Material mat; 
    private bool isHovered = false;
    private float glowStrength = 0f;

    [SerializeField] private float fadeSpeed = 5f;     // скорость появления/исчезания
    [ColorUsage(true, true)]
    [SerializeField] private Color glowColor = new Color(0f, 45f, 120f, 1f); // HDR разрешён

    void Start()
    {
        // Создаём индивидуальный материал, чтобы не править общую ссылку
        mat = GetComponent<Renderer>().material;
        Debug.Log(mat);
        mat.SetColor("GlowColor", glowColor);
        mat.SetFloat("GlowStrength", 0f);
        // _BaseGlow, _PulseSpeed, _PulseAmplitude можешь регулировать прямо в материале
    }

    void OnMouseEnter() => isHovered = true;
    void OnMouseExit() => isHovered = false;

    void Update()
    {
        float target = isHovered ? 1f : 0f;
        glowStrength = Mathf.Lerp(glowStrength, target, Time.deltaTime * fadeSpeed);
        Debug.Log(glowStrength);
        mat.SetFloat("GlowStrength", glowStrength);
    }
}
