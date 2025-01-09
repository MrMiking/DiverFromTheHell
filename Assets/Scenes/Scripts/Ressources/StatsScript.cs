using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

// Déclare un événement personnalisé appelé UIEvent qui peut accepter trois paramètres de type float
[System.Serializable]
public class UIEvent : UnityEvent<float, float, float> { }

// Classe principale qui gère les statistiques du joueur, notamment la stamina (endurance) et les HP (points de vie)
public class StatsScript : MonoBehaviour
{
    // Références aux événements Unity
    [Header("Références")]
    public UnityEvent OnStaminaExhausted; // Déclenché lorsque la stamina est épuisée
    public UnityEvent OnStaminaReplinished; // Déclenché lorsque la stamina est complètement rechargée
    public UIEvent OnStaminaChanged; // Déclenché lorsque la valeur de la stamina change (pour la mise à jour de l'UI)
    public UIEvent OnHPChanged; // Déclenché lorsque la valeur des HP change (pour la mise à jour de l'UI)


    // Variables pour la gestion de la stamina (endurance)
    [Header("Statistiques de Stamina")]
    public float maxStamina; // Valeur maximale de stamina
    public float minStamina; // Valeur minimale de stamina (par défaut, cela pourrait être 0)
    public float currentStamina; // Valeur actuelle de stamina

    public float staminaLoss; // Vitesse à laquelle la stamina se réduit (lors de la course)
    public float staminaGain; // Vitesse à laquelle la stamina se régénère (quand on ne court pas)
    public float staminaLossJump;

    // Variables pour la gestion des HP (points de vie)
    [Header("Statistiques de HP")]
    public float maxHP; // Valeur maximale de HP
    public float minHP; // Valeur minimale de HP
    public float currentHP; // Valeur actuelle de HP

    // Variables pour suivre l'état du joueur
    [Header("Conditions")]
    public bool isRunning; // Indique si le joueur est en train de courir

    // Méthode appelée au démarrage du jeu (ou lors de l'activation du script)
    void Start()
    {
        // Initialiser la stamina et les HP au maximum
        currentStamina = maxStamina;
        currentHP = maxHP;

        // Déclenche l'événement pour informer l'UI que la stamina a changé (au début du jeu)
        OnHPChanged.Invoke(currentHP, minHP, maxHP);
        OnStaminaChanged.Invoke(currentStamina, minStamina, maxStamina);
    }

    // Méthode appelée à chaque frame pour gérer la logique de mise à jour des stats
    void Update()
    {
        HandleStamina(); // Appelle la méthode qui gère la stamina

        if(Input.GetKeyDown(KeyCode.P)) 
        {
            HealthLoss(30);
        }

    }

    // Méthode qui gère la perte et la récupération de stamina
    public void HandleStamina()
    {
        // Si le joueur ne court pas
        if (!isRunning)
        {
            // Augmenter la stamina au fil du temps jusqu'à la valeur maximale
            currentStamina += staminaGain * Time.deltaTime;
            if (currentStamina >= maxStamina)
            {
                currentStamina = maxStamina; // Limite la stamina à la valeur maximale
            }
        }

        // Si le joueur court
        if (isRunning)
        {
            // Réduire la stamina au fil du temps
            currentStamina -= staminaLoss * Time.deltaTime;
            if (currentStamina <= minStamina)
            {
                // Si la stamina atteint la valeur minimale
                currentStamina = minStamina; // Limite la stamina à la valeur minimale
                OnStaminaExhausted.Invoke(); // Déclenche l'événement indiquant que la stamina est épuisée
                StartCoroutine(WaitUntilStaminaIsReplinished()); // Démarre une coroutine pour attendre que la stamina soit rechargée
            }
        }

        // Déclenche l'événement pour mettre à jour l'UI avec la nouvelle valeur de stamina
        OnStaminaChanged.Invoke(currentStamina, minStamina, maxStamina);
    }

    // Méthode appelée quand le joueur commence à courir
    public void OnStartedRunning()
    {
        isRunning = true; // Indique que le joueur court
    }

    // Méthode appelée quand le joueur arrête de courir
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


    // Coroutine qui attend que la stamina se recharge complètement
    IEnumerator WaitUntilStaminaIsReplinished()
    {
        // Attend un certain nombre de secondes, calculé en fonction du taux de régénération de la stamina
        yield return new WaitForSeconds(maxStamina / staminaGain);

        // Déclenche l'événement indiquant que la stamina est complètement rechargée
        OnStaminaReplinished.Invoke();
        yield break; // Termine la coroutine
    }
}
