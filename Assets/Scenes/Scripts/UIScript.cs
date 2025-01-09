using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIScript : MonoBehaviour
{
    public Slider staminaSlider;
    public Slider hpSlider;
    public float smoothSpeed = 5f; // Vitesse d'interpolation pour rendre la transition fluide

    private float targetStaminaValue;
    private float targetHPValue;

    private void Start()
    {
    }

    void Update()
    {
        MathLerpStats();
    }

    public void MathLerpStats()
    {
        // Applique une interpolation entre la valeur actuelle et la valeur cible des sliders
        staminaSlider.value = Mathf.Lerp(staminaSlider.value, targetStaminaValue, Time.deltaTime * smoothSpeed);
        hpSlider.value = Mathf.Lerp(hpSlider.value, targetHPValue, Time.deltaTime * smoothSpeed);

    }

    public void OnStaminaChanged(float currentStamina, float minStamina, float maxStamina)
    {
        staminaSlider.maxValue = maxStamina;
        staminaSlider.minValue = minStamina;

        // On ne change plus directement la valeur du slider, mais on fixe une cible
        targetStaminaValue = currentStamina;
    }

    public void OnHPChanged(float currentHP, float minHp, float maxHp)
    {
        hpSlider.maxValue = maxHp;
        hpSlider.minValue = minHp;

        // De la même manière, on fixe une cible pour la vie
        targetHPValue = currentHP;
    }
}
