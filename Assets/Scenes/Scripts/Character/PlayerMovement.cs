using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

[System.Serializable]
public class AnimationEvent : UnityEvent<float> { }

public class PlayerMovement : MonoBehaviour
{
    [Header("Références")]
    [SerializeField] Rigidbody rb;
    [SerializeField] Transform cameraTransform;
    public UnityEvent OnStartedRunning;
    public UnityEvent OnStoppedRunning;
    public UnityEvent OnJumped;
    public AnimationEvent OnSpeedUpdated;

    [Header("Variables de déplacement du personnage")]
    public float walkingSpeed;
    public float runningSpeed;
    private float currentSpeed;
    public float speed;
    public bool canRun;
    private bool isMoving;

    [Header("Rotation lissée du personnage")]
    public float turnSmoothTime;
    float turnSmoothVelocity;

    [Header("Physique")]
    public float gravityForce;
    public LayerMask groundMask;
    public bool canJump;
    public float jumpForceY; // Contrôle la force verticale du saut
    public float jumpForceForwardMultiplier; // Contrôle la force horizontale du saut

    Vector2 input;
    Vector3 moveDirection; // Ajout d'une variable pour stocker la direction du mouvement

    void Update()
    {
        GetInput();
        SpeedCalculation();
    }

    private void FixedUpdate()
    {
        Movement();
    }

    // Méthode qui gère le mouvement
    public void Movement()
    {
        // On crée un vecteur direction basé sur les entrées de l'utilisateur (axes horizontal et vertical)
        Vector3 direction = new Vector3(input.x, 0, input.y).normalized;

        // Récupère les vecteurs avant et droit de la caméra
        Vector3 cameraForward = cameraTransform.forward;
        Vector3 cameraRight = cameraTransform.right;

        // On ignore la composante Y (verticale) pour que le mouvement ne soit pas affecté par l'inclinaison de la caméra
        cameraForward.y = 0f;
        cameraRight.y = 0f;
        cameraForward.Normalize(); // Normalise le vecteur avant
        cameraRight.Normalize(); // Normalise le vecteur droit

        // Calcule la direction du mouvement en fonction de la caméra et des entrées de l'utilisateur
        moveDirection = cameraForward * input.y + cameraRight * input.x; // On stocke cette direction dans moveDirection

        // On commence par la vitesse actuelle du Rigidbody pour la modifier
        Vector3 velocity = rb.velocity;

        // Si le joueur est au sol
        if (IsGrounded())
        {
            // Applique la gravité et réinitialise la vitesse Y pour éviter que le joueur ne monte en l'air
            velocity.y = -gravityForce * Time.fixedDeltaTime;
            canJump = true;
        }
        else
        {
            // Si le joueur est en l'air, continue d'appliquer la gravité
            velocity.y += -gravityForce * Time.fixedDeltaTime;
            JumpMechanic();
        }

        // Si la direction du mouvement est significative (supérieure à un certain seuil)
        if (moveDirection.magnitude >= 0.1f && IsGrounded())
        {
            isMoving = true;

            // Calcule l'angle vers lequel le joueur doit se tourner en fonction de la direction du mouvement
            float targetAngle = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg;
            // Lisse la rotation du personnage pour qu'elle ne soit pas instantanée
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            // Applique la rotation calculée au joueur
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            // Applique la vitesse de mouvement sur les axes X et Z (horizontal et avant/arrière)
            velocity.x = moveDirection.x * currentSpeed * Time.fixedDeltaTime;
            velocity.z = moveDirection.z * currentSpeed * Time.fixedDeltaTime;
        }
        else isMoving = false;

        // Applique la vitesse finale au Rigidbody, incluant la gravité et le mouvement
        rb.velocity = velocity;

        speed = rb.velocity.magnitude;

        // A rendre plus complexe pour le saut
        OnSpeedUpdated.Invoke(speed);
    }

    // Méthode modifiée pour inclure une force directionnelle pendant le saut
    public void JumpMechanic()
    {
        if (canJump)
        {

            // Ajout d'une force en Y (verticale) pour le saut
            Vector3 jumpForce = new Vector3(0f, jumpForceY, 0f);

            // Ajout de la force directionnelle selon la direction actuelle du joueur
            Vector3 forwardJumpForce = moveDirection * jumpForceForwardMultiplier;

            // Combine la force verticale et la force directionnelle
            rb.AddForce(jumpForce + forwardJumpForce, ForceMode.Impulse);

            canJump = false; // Désactive la possibilité de sauter de nouveau jusqu'à ce que le joueur touche le sol
            Debug.Log("Tried To Jump with forward force");

            OnJumped.Invoke();
        }
    }

    // Méthode pour récupérer les entrées de l'utilisateur
    public void GetInput()
    {
        // On récupère les valeurs des axes d'entrée "Horizontal" (gauche/droite) et "Vertical" (avant/arrière)
        input = new Vector2(
            Input.GetAxis("Horizontal"),
            Input.GetAxis("Vertical")
        );
    }

    // Méthode pour calculer la vitesse du joueur (marche ou course)
    public void SpeedCalculation()
    {
        // Si le bouton pour courir est pressé (par défaut "Fire3" est Shift gauche) et que le joueur peut courir
        if (Input.GetButton("Fire3") && canRun && isMoving)
        {
            // On passe à la vitesse de course
            currentSpeed = runningSpeed;
            OnStartedRunning.Invoke(); // Appelle l'événement lorsque le joueur commence à courir
        }
        else
        {
            // Sinon, on reste à la vitesse de marche
            currentSpeed = walkingSpeed;
            OnStoppedRunning.Invoke(); // Appelle l'événement lorsque le joueur arrête de courir
        }
    }

    // Méthode appelée lorsque l'endurance du joueur est épuisée
    public void OnStaminaExhausted()
    {
        canRun = false; // Désactive la possibilité de courir
    }

    // Méthode appelée lorsque l'endurance du joueur est rechargée
    public void OnStaminaReplinished()
    {
        canRun = true; // Réactive la possibilité de courir
    }

    // Méthode pour savoir si le joueur est au sol
    bool IsGrounded()
    {
        // Effectue un Raycast vers le bas pour détecter s'il y a un sol sous le joueur
        return Physics.Raycast(transform.position, Vector3.down, 2f, groundMask);
    }
}
