using UnityEngine;

namespace PlayerController
{
    public class FpController : MonoBehaviour
    {
        [Header("Parameters")] 
        [SerializeField] private float walkSpeed = 3.0f;
        [SerializeField] private float sprintMultiplier = 2.0f;
        [SerializeField] private float jumpForce = 5.0f;
        [SerializeField] private float gravityMultiplier = 1.0f;
        [SerializeField] private float mouseSensitivity = 0.1f;
        [SerializeField] private float upDownLookRange = 80f;
        [SerializeField] private CharacterController characterController;
        [SerializeField] private Camera mainCamera;
        [SerializeField] private PlayerInput playerInputHandler;
        
        private Vector3 _currentMovement;
        private float _verticalRotation;
        private float CurrentSpeed => walkSpeed * (playerInputHandler.SprintInput ? sprintMultiplier : 1);
        
        // Start is called before the first frame update
        void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        
        // Update is called once per frame
        void Update()
        {
            HandleMovement();
            HandleRotation();
        }
        
        private Vector3 CalculateWorldDirection()
        {
            Vector3 inputDirection =
                new Vector3(playerInputHandler.MovementInput.x, 0f, playerInputHandler.MovementInput.y);
            Vector3 worldDirection = transform.TransformDirection(inputDirection);
            return worldDirection.normalized;
        }
        
        private void HandleJumping()
        {
            if (characterController.isGrounded)
            {
                _currentMovement.y = -0.5f;
                if (playerInputHandler.JumpInput)
                {
                    _currentMovement.y = jumpForce;
                }
            }
            else
            {
                _currentMovement.y += Physics.gravity.y * gravityMultiplier * Time.deltaTime;
            }
        }
        
        private void HandleMovement()
        {
            Vector3 worldDirection = CalculateWorldDirection();
            _currentMovement.x = worldDirection.x * CurrentSpeed;
            _currentMovement.z = worldDirection.z * CurrentSpeed;
            HandleJumping();
            characterController.Move(_currentMovement * Time.deltaTime);
        }
        
        private void ApplyHorizontalRotation(float rotationAmount)
        {
            transform.Rotate(0, rotationAmount, 0);
        }
        
        private void ApplyVerticalRotation(float rotationAmount)
        {
            _verticalRotation = Mathf.Clamp(_verticalRotation - rotationAmount, -upDownLookRange, upDownLookRange);
            mainCamera.transform.localRotation = Quaternion.Euler(_verticalRotation, 0, 0);
        }

        private void HandleRotation()
        {
            float mouseXRotation = playerInputHandler.RotationInput.x * mouseSensitivity;
            float mouseYRotation = playerInputHandler.RotationInput.y * mouseSensitivity;
            ApplyHorizontalRotation(mouseXRotation);
            ApplyVerticalRotation(mouseYRotation);
        }
    }
}