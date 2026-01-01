using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider healthSlider;
    public float damageAmount;
    public void SetHealth(float healthNormalized)
    {
        healthSlider.value = healthNormalized;
    }
    public void Damage(float damageAmount)
    {
        healthSlider.value -= damageAmount;
    }
}
