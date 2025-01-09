using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

// D�clare un �v�nement personnalis� appel� UIEvent qui peut accepter trois param�tres de type float
[System.Serializable]
public class UIEvent : UnityEvent<float, float, float> { }

// Classe principale qui g�re les statistiques du joueur, notamment la stamina (endurance) et les HP (points de vie)
public class StatsScript : MonoBehaviour
{
    // R�f�rences aux �v�nements Unity
    [Header("R�f�rences")]
    public UnityEvent OnStaminaExhausted; // D�clench� lorsque la stamina est �puis�e
    public UnityEvent OnStaminaReplinished; // D�clench� lorsque la stamina est compl�tement recharg�e
    public UIEvent OnStaminaChanged; // D�clench� lorsque la valeur de la stamina change (pour la mise � jour de l'UI)
    public UIEvent OnHPChanged; // D�clench� lorsque la valeur des HP change (pour la mise � jour de l'UI)


    // Variables pour la gestion de la stamina (endurance)
    [Header("Statistiques de Stamina")]
    public float maxStamina; // Valeur maximale de stamina
    public float minStamina; // Valeur minimale de stamina (par d�faut, cela pourrait �tre 0)
    public float currentStamina; // Valeur actuelle de stamina

    public float staminaLoss; // Vitesse � laquelle la stamina se r�duit (lors de la course)
    public float staminaGain; // Vitesse � laquelle la stamina se r�g�n�re (quand on ne court pas)
    public float staminaLossJump;

    // Variables pour la gestion des HP (points de vie)
    [Header("Statistiques de HP")]
    public float maxHP; // Valeur maximale de HP
    public float minHP; // Valeur minimale de HP
    public float currentHP; // Valeur actuelle de HP

    // Variables pour suivre l'�tat du joueur
    [Header("Conditions")]
    public bool isRunning; // Indique si le joueur est en train de courir

    // M�thode appel�e au d�marrage du jeu (ou lors de l'activation du script)
    void Start()
    {
        // Initialiser la stamina et les HP au maximum
        currentStamina = maxStamina;
        currentHP = maxHP;

        // D�clenche l'�v�nement pour informer l'UI que la stamina a chang� (au d�but du jeu)
        OnHPChanged.Invoke(currentHP, minHP, maxHP);
        OnStaminaChanged.Invoke(currentStamina, minStamina, maxStamina);
    }

    // M�thode appel�e � chaque frame pour g�rer la logique de mise � jour des stats
    void Update()
    {
        HandleStamina(); // Appelle la m�thode qui g�re la stamina

        if(Input.GetKeyDown(KeyCode.P)) 
        {
            HealthLoss(30);
        }

    }

    // M�thode qui g�re la perte et la r�cup�ration de stamina
    public void HandleStamina()
    {
        // Si le joueur ne court pas
        if (!isRunning)
        {
            // Augmenter la stamina au fil du temps jusqu'� la valeur maximale
            currentStamina += staminaGain * Time.deltaTime;
            if (currentStamina >= maxStamina)
            {
                currentStamina = maxStamina; // Limite la stamina � la valeur maximale
            }
        }

        // Si le joueur court
        if (isRunning)
        {
            // R�duire la stamina au fil du temps
            currentStamina -= staminaLoss * Time.deltaTime;
            if (currentStamina <= minStamina)
            {
                // Si la stamina atteint la valeur minimale
                currentStamina = minStamina; // Limite la stamina � la valeur minimale
                OnStaminaExhausted.Invoke(); // D�clenche l'�v�nement indiquant que la stamina est �puis�e
                StartCoroutine(WaitUntilStaminaIsReplinished()); // D�marre une coroutine pour attendre que la stamina soit recharg�e
            }
        }

        // D�clenche l'�v�nement pour mettre � jour l'UI avec la nouvelle valeur de stamina
        OnStaminaChanged.Invoke(currentStamina, minStamina, maxStamina);
    }

    // M�thode appel�e quand le joueur commence � courir
    public void OnStartedRunning()
    {
        isRunning = true; // Indique que le joueur court
    }

    // M�thode appel�e quand le joueur arr�te de courir
    public void OnStoppedRunning()
    {
        isRunning = false; // Indique que le joueur ne court plus
    }

    public void HealthLoss(float loss)
    {
        currentHP -= loss;
        if(currentHP >= maxHP) { currentHP = maxHP; }
        if (currentHP <= minHP)
        {
            Debug.Log("Player should be dead");
        }
        OnHPChanged.Invoke(currentHP, minHP, maxHP);
    }

    public void OnJumped()
    {
        currentStamina -= staminaLossJump;
    }


    // Coroutine qui attend que la stamina se recharge compl�tement
    IEnumerator WaitUntilStaminaIsReplinished()
    {
        // Attend un certain nombre de secondes, calcul� en fonction du taux de r�g�n�ration de la stamina
        yield return new WaitForSeconds(maxStamina / staminaGain);

        // D�clenche l'�v�nement indiquant que la stamina est compl�tement recharg�e
        OnStaminaReplinished.Invoke();
        yield break; // Termine la coroutine
    }
}
