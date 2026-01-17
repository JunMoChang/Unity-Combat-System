using UnityEngine;

namespace Player.Function
{
    public class SightControl
    {
        private readonly Transform lookTarget;
        private float mouseX, mouseY;

        public SightControl(Transform lookTarget)
        {
            this.lookTarget = lookTarget;
        }

        public Vector2 Onlook(string actionName)
        {
            return InputManager.Instance.GetVector2(actionName);
        }

        public void HandleLook(float mouseSensitivity, Vector2 lookInput)
        {
            if (lookInput != Vector2.zero)
            {
                mouseX += lookInput.x * mouseSensitivity * Time.deltaTime;
                mouseY -= lookInput.y * mouseSensitivity * Time.deltaTime;

                lookTarget.localRotation = Quaternion.Euler(mouseY, mouseX, 0);
            }
        }
    }
}