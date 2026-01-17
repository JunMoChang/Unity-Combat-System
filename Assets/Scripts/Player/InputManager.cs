using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class InputManager : MonoBehaviour
    {
        public enum PlayerInputActionName
        {
            Move,
            Walk,
            Sprint,
            Look,
            Attack,
            Interaction,
        }

        public enum UIActionName
        {
            Back,
            OpenInventory,
            OpenEquipment
        }
        public static InputManager Instance { get; private set; }
    
        [Header("Input Actions")]
        public PlayerInput playerInput;
        
        private InputActionMap playerActionMap;
        private InputActionMap uiActionMap;
        
        private readonly Dictionary<string, bool> inputStates = new();
        private readonly Dictionary<string, InputAction> inputActions = new();
        
        public event Action<string> OnInputPressed;
        public event Action<string> OnInputReleased;
        public event Action<string, InputControl> OnInputPressedWithControl;
    
        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeInputSystem();
            }
            else Destroy(gameObject);
            
        }
    
        private void InitializeInputSystem()
        {
            if (playerInput == null) playerInput = GetComponent<PlayerInput>();
            
            playerActionMap = playerInput.actions.FindActionMap("Player");
            uiActionMap =  playerInput.actions.FindActionMap("UI");

            RegisterInputActions();
        }
    
        private void RegisterInputActions()
        {
            string[] actionNames = Enum.GetNames(typeof(PlayerInputActionName));
            foreach (string n in actionNames)
            {
                RegisterAction(playerActionMap, n);
            }
            if(!playerActionMap.enabled) playerActionMap.Enable();
            
            if(!uiActionMap.enabled) uiActionMap.Enable();
            actionNames = Enum.GetNames(typeof(UIActionName));
            foreach (string n in actionNames)
            {
                RegisterAction(uiActionMap, n);
            }
            
        }
    
        private void RegisterAction(InputActionMap actionMap, string actionName)
        {
            InputAction action = null;
            
            if (actionMap == playerActionMap)
            {
                action= playerActionMap.FindAction(actionName);
            }
            else if (actionMap == uiActionMap)
            {
                action = uiActionMap.FindAction(actionName);
            }

            if (action == null) return;
            
            inputActions[actionName] = action;
            inputStates[actionName] = false;
                
            action.started += ctx => OnInputStarted(actionName, ctx);
            action.canceled += ctx => OnInputCanceled(actionName, ctx);
            action.performed += ctx => OnInputPerformed(actionName, ctx);
        }
        
        private void OnInputStarted(string actionName, InputAction.CallbackContext context)
        { 
            inputStates[actionName] = true;
            OnInputPressed?.Invoke(actionName);
            OnInputPressedWithControl?.Invoke(actionName, context.control);
        }
    
        private void OnInputCanceled(string actionName, InputAction.CallbackContext context)
        {
            inputStates[actionName] = false;
            OnInputReleased?.Invoke(actionName);
        }
    
        private void OnInputPerformed(string actionName, InputAction.CallbackContext context)
        {
        }
    
    
        public bool GetButton(string actionName)
        {
            return inputStates.ContainsKey(actionName) && inputStates[actionName];
        }
        
        public Vector2 GetVector2(string actionName)
        {
            if (inputActions.TryGetValue(actionName, out InputAction inputAction))
                return inputAction.ReadValue<Vector2>();
            return Vector2.zero;
        }
    
        public float GetFloat(string actionName)
        {
            if (inputActions.TryGetValue(actionName, out InputAction inputAction))
                return inputAction.ReadValue<float>();
            return 0f;
        }
        
        public void StartRebinding(string actionName, Action<bool> callback)
        {
            InputAction action = playerActionMap.FindAction(actionName);
            if (action != null)
            {
                var rebindingOperation = action.PerformInteractiveRebinding()
                    .OnComplete(operation =>
                    {
                        callback?.Invoke(true);
                        operation.Dispose();
                    })
                    .OnCancel(operation =>
                    {
                        callback?.Invoke(false);
                        operation.Dispose();
                    })
                    .Start();
            }
        }
    
        private void OnDestroy()
        {
            foreach (InputAction action in inputActions.Values)
            {
                action.Dispose();
            }
        }
    }
}