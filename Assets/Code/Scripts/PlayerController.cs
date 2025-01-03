using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Player Settings")]
    [SerializeField]
    private float playerSpeed = 4.5f;
    [SerializeField]
    private float sprintingSpeed = 7.3f;
    [SerializeField]
    private float jumpHeight = 1.54f;
    [SerializeField]
    private float gravityValue = -9.81f;

    private CharacterController characterController;
    private Vector3 playerVelocity;
    private bool groundedPlayer;
    private InputManager inputManager;
    private Transform cameraTransform;

    private void Start()
    {
        characterController = gameObject.GetComponent<CharacterController>();
        inputManager = InputManager.Instance;
        cameraTransform = Camera.main.transform;
    }

    private void Update()
    {
        groundedPlayer = characterController.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }


        Vector2 movement = inputManager.GetPlayerMovement();

        // Handle sprinting logic
        float currentSpeed = inputManager.PlayerIsSprinting() ? sprintingSpeed : playerSpeed;
        Debug.Log(inputManager.PlayerIsSprinting());
        // We need to convert our vector 2 to a vector 3D.
        Vector3 move = new Vector3(movement.x, 0f, movement.y);

        move = cameraTransform.forward * move.z + cameraTransform.right * move.x;
        move.y = 0f; // Ensure the player doesn't move vertically based on camera tilt
        characterController.Move(move * Time.deltaTime * currentSpeed);

        // Align player's rotation with the camera's rotation
        Vector3 cameraForward = cameraTransform.forward;
        cameraForward.y = 0f; // Ignore camera's vertical tilt for player rotation
        if (cameraForward.sqrMagnitude > 0f) // Prevent jitter when looking straight down/up
        {
            Quaternion targetRotation = Quaternion.LookRotation(cameraForward);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10f * Time.deltaTime); // Smooth rotation
        }

        // Handle jumping
        if (inputManager.PlayerJumpedThisFrame() && groundedPlayer)
        {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
        }

        playerVelocity.y += gravityValue * Time.deltaTime;
        characterController.Move(playerVelocity * Time.deltaTime); 

    }
}