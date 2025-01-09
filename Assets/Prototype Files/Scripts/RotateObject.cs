using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateObject : MonoBehaviour
{
    // Vitesse de rotation sur l'axe Y
    public float rotationSpeed = 100f;

    void Update()
    {
        // Applique une rotation constante sur l'axe Y, dépendant du temps et de la vitesse définie
        transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f);
    }
}
