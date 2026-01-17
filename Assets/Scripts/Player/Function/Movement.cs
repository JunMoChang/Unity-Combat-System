using UnityEngine;
using UnityEngine.InputSystem;

namespace Player.Function
{
    public class Movement
    {
        private readonly CharacterController controller;
        private readonly Transform playerModel;
        private readonly Cinemachine.CinemachineVirtualCamera playerCamera;
        private readonly float gravity;
        public Movement(CharacterController controller, Transform playerModel,
            Cinemachine.CinemachineVirtualCamera playerCamera, float gravity)
        {
            this.controller = controller;
            this.playerModel = playerModel;
            this.playerCamera = playerCamera;
            this.gravity = gravity;
        }
        public Vector2 OnMove(string actioName)
        {
            return InputManager.Instance.GetVector2(actioName);
        }

        public void HandleMove(Vector2 moveInput, float speed)  
        {
            Vector3 vertical = Vector3.zero;
            if (controller.isGrounded)
            {
                vertical.y = 0;
            }
            else
            {
                vertical.y -= gravity;
            }
            controller.Move(vertical * Time.deltaTime);
            
            if (moveInput == Vector2.zero) return;
            
            Vector3 forward = playerCamera.transform.forward;
            Vector3 right = playerCamera.transform.right;
            forward.y = 0;
            right.y = 0;
            forward.Normalize();
            right.Normalize();
            
            Vector3 moveDirection = (forward * moveInput.y + right * moveInput.x).normalized;

            HandleRotation(moveDirection);
            
            controller.Move(speed * Time.deltaTime * moveDirection);
        }
        
        private void HandleRotation(Vector3 targetDirection)
        {
            float angle = Mathf.Atan2(targetDirection.x, targetDirection.z) * Mathf.Rad2Deg;
            playerModel.rotation = Quaternion.Euler(playerModel.rotation.eulerAngles.x, angle,
                playerModel.rotation.eulerAngles.z);
        }
    }
}