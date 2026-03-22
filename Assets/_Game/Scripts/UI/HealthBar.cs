using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Slider healthbarSlider;
    [SerializeField] private Gradient gradient;
    [SerializeField] private Image fillColor;
    public void SetMaxHealth(float health)
    {
        healthbarSlider.maxValue = health;
        healthbarSlider.value = health;
        fillColor.color = gradient.Evaluate(1f);
    }

    public void SetHealth(float health)
    {
        healthbarSlider.value = health;
        fillColor.color = gradient.Evaluate(healthbarSlider.normalizedValue);
    }
}
