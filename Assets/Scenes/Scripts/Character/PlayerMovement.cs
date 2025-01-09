using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

[System.Serializable]
public class AnimationEvent : UnityEvent<float> { }

public class PlayerMovement : MonoBehaviour
{
    [Header("R�f�rences")]
    [SerializeField] Rigidbody rb;
    [SerializeField] Transform cameraTransform;
    public UnityEvent OnStartedRunning;
    public UnityEvent OnStoppedRunning;
    public UnityEvent OnJumped;
    public AnimationEvent OnSpeedUpdated;

    [Header("Variables de d�placement du personnage")]
    public float walkingSpeed;
    public float runningSpeed;
    private float currentSpeed;
    public float speed;
    public bool canRun;
    private bool isMoving;

    [Header("Rotation liss�e du personnage")]
    public float turnSmoothTime;
    float turnSmoothVelocity;

    [Header("Physique")]
    public float gravityForce;
    public LayerMask groundMask;
    public bool canJump;
    public float jumpForceY; // Contr�le la force verticale du saut
    public float jumpForceForwardMultiplier; // Contr�le la force horizontale du saut

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

    // M�thode qui g�re le mouvement
    public void Movement()
    {
        // On cr�e un vecteur direction bas� sur les entr�es de l'utilisateur (axes horizontal et vertical)
        Vector3 direction = new Vector3(input.x, 0, input.y).normalized;

        // R�cup�re les vecteurs avant et droit de la cam�ra
        Vector3 cameraForward = cameraTransform.forward;
        Vector3 cameraRight = cameraTransform.right;

        // On ignore la composante Y (verticale) pour que le mouvement ne soit pas affect� par l'inclinaison de la cam�ra
        cameraForward.y = 0f;
        cameraRight.y = 0f;
        cameraForward.Normalize(); // Normalise le vecteur avant
        cameraRight.Normalize(); // Normalise le vecteur droit

        // Calcule la direction du mouvement en fonction de la cam�ra et des entr�es de l'utilisateur
        moveDirection = cameraForward * input.y + cameraRight * input.x; // On stocke cette direction dans moveDirection

        // On commence par la vitesse actuelle du Rigidbody pour la modifier
        Vector3 velocity = rb.linearVelocity;

        // Si le joueur est au sol
        if (IsGrounded())
        {
            // Applique la gravit� et r�initialise la vitesse Y pour �viter que le joueur ne monte en l'air
            velocity.y = -gravityForce * Time.fixedDeltaTime;
            canJump = true;
        }
        else
        {
            // Si le joueur est en l'air, continue d'appliquer la gravit�
            velocity.y += -gravityForce * Time.fixedDeltaTime;
            JumpMechanic();
        }

        // Si la direction du mouvement est significative (sup�rieure � un certain seuil)
        if (moveDirection.magnitude >= 0.1f && IsGrounded())
        {
            isMoving = true;

            // Calcule l'angle vers lequel le joueur doit se tourner en fonction de la direction du mouvement
            float targetAngle = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg;
            // Lisse la rotation du personnage pour qu'elle ne soit pas instantan�e
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            // Applique la rotation calcul�e au joueur
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            // Applique la vitesse de mouvement sur les axes X et Z (horizontal et avant/arri�re)
            velocity.x = moveDirection.x * currentSpeed * Time.fixedDeltaTime;
            velocity.z = moveDirection.z * currentSpeed * Time.fixedDeltaTime;
        }
        else isMoving = false;

        // Applique la vitesse finale au Rigidbody, incluant la gravit� et le mouvement
        rb.linearVelocity = velocity;

        speed = rb.linearVelocity.magnitude;

        // A rendre plus complexe pour le saut
        OnSpeedUpdated.Invoke(speed);
    }

    // M�thode modifi�e pour inclure une force directionnelle pendant le saut
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

            canJump = false; // D�sactive la possibilit� de sauter de nouveau jusqu'� ce que le joueur touche le sol
            Debug.Log("Tried To Jump with forward force");

            OnJumped.Invoke();
        }
    }

    // M�thode pour r�cup�rer les entr�es de l'utilisateur
    public void GetInput()
    {
        // On r�cup�re les valeurs des axes d'entr�e "Horizontal" (gauche/droite) et "Vertical" (avant/arri�re)
        input = new Vector2(
            Input.GetAxis("Horizontal"),
            Input.GetAxis("Vertical")
        );
    }

    // M�thode pour calculer la vitesse du joueur (marche ou course)
    public void SpeedCalculation()
    {
        // Si le bouton pour courir est press� (par d�faut "Fire3" est Shift gauche) et que le joueur peut courir
        if (Input.GetButton("Fire3") && canRun && isMoving)
        {
            // On passe � la vitesse de course
            currentSpeed = runningSpeed;
            OnStartedRunning.Invoke(); // Appelle l'�v�nement lorsque le joueur commence � courir
        }
        else
        {
            // Sinon, on reste � la vitesse de marche
            currentSpeed = walkingSpeed;
            OnStoppedRunning.Invoke(); // Appelle l'�v�nement lorsque le joueur arr�te de courir
        }
    }

    // M�thode appel�e lorsque l'endurance du joueur est �puis�e
    public void OnStaminaExhausted()
    {
        canRun = false; // D�sactive la possibilit� de courir
    }

    // M�thode appel�e lorsque l'endurance du joueur est recharg�e
    public void OnStaminaReplinished()
    {
        canRun = true; // R�active la possibilit� de courir
    }

    // M�thode pour savoir si le joueur est au sol
    bool IsGrounded()
    {
        // Effectue un Raycast vers le bas pour d�tecter s'il y a un sol sous le joueur
        return Physics.Raycast(transform.position, Vector3.down, 2f, groundMask);
    }
}
