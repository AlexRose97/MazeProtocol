using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PlayerController
{
    public class PlayerInput : MonoBehaviour
    {
        [Header("Input Action Assets")] [SerializeField]
        private InputActionAsset playerControllerAsset;

        [SerializeField] private string actionMapName = "Player";
        [SerializeField] private string movement = "Movement";
        [SerializeField] private string rotation = "Rotation";
        [SerializeField] private string jump = "Jump";
        [SerializeField] private string sprint = "Sprint";

        private InputAction _movementAction;
        private InputAction _rotationAction;
        private InputAction _jumpAction;
        private InputAction _sprintAction;

        public Vector2 MovementInput { get; private set; }
        public Vector2 RotationInput { get; private set; }
        public bool JumpInput { get; private set; }
        public bool SprintInput { get; private set; }

        private void Awake()
        {
            InputActionMap map = playerControllerAsset.FindActionMap(actionMapName);
            _movementAction = map.FindAction(movement);
            _rotationAction = map.FindAction(rotation);
            _jumpAction = map.FindAction(jump);
            _sprintAction = map.FindAction(sprint);
            
            SubscribeActivationValueEvents();
        }

        private void SubscribeActivationValueEvents()
        {
            _movementAction.performed += inputInfo => MovementInput = inputInfo.ReadValue<Vector2>();
            _movementAction.canceled += inputInfo => MovementInput = Vector2.zero;

            _rotationAction.performed += inputInfo => RotationInput = inputInfo.ReadValue<Vector2>();
            _rotationAction.canceled += inputInfo => RotationInput = Vector2.zero;

            _jumpAction.performed += inputInfo => JumpInput = true;
            _jumpAction.canceled += inputInfo => JumpInput = false;

            _sprintAction.performed += inputInfo => SprintInput = true;
            _sprintAction.canceled += inputInfo => SprintInput = false;
        }

        private void OnEnable()
        {
            playerControllerAsset.FindActionMap(actionMapName).Enable();
        }
        
        private void OnDisable()
        {
            playerControllerAsset.FindActionMap(actionMapName).Disable();
        }

    }
}