using System;
using System.Collections;
using Cinemachine;
using Player.Function;
using Player.StateManager;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        public static PlayerController Instance{get; private set;}
        
        #region 组件

        private CharacterController characterController;
        private CinemachineVirtualCamera playerCamera;
        public Animator animator;
        public Transform playerModel;

        #endregion
        
        #region 依赖

        private Movement movement;
        [SerializeField] public AttackSetting attackSetting;
        private SightControl sightControl;
        public WeaponHandel weaponHandel;
        public PlayerFsm playerFsm;
        public PlayerAnimationParameter animationParameter;

        #endregion

        #region 参数
        
        [Header("视觉旋转")]
        public Transform lookTarget;
        public float mouseSensitivity = 5f;
        [Header("移动速度")]
        public float walkSpeed = 2f;
        public float runSpeed = 5f;
        public float sprintSpeed = 8f;
        public float gravity = 9.81f;
        
        private LayerMask enemyLayer;
        #endregion

        #region 控制条件

        private Vector2 moveInput;
        private Vector2 lookInput;
        public Vector2 MoveInput => moveInput;
        private float exitCombatTimer;
        private readonly float exitCombatTime = 5f;

        #endregion
        
        public Action<string, InputControl> attackAction;
        
        void Awake()
        {
            Instance = this;
            TryGetComponent(out characterController);
            TryGetComponent(out weaponHandel);
            TryGetComponent(out animator);
            playerCamera = GameObject.FindWithTag("PlayerCamera").GetComponent<CinemachineVirtualCamera>();
        }
        
        void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            
            enemyLayer = LayerMask.GetMask("Enemy");
            
            movement = new Movement(characterController, playerModel, playerCamera, gravity);
            sightControl = new SightControl(lookTarget);
            attackSetting = new AttackSetting(transform, weaponHandel, enemyLayer);
            playerFsm =  new PlayerFsm(attackSetting.attackMapping);
            animationParameter =  new PlayerAnimationParameter();
            
            playerFsm.Start();
        }

        void Update()
        {
            playerFsm.CurrentState.Update();
            
            if(playerFsm.isMoving) HandleMove();
            transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);
            HandleLook();
            HandelCombatState();
        }
        private void HandleMove()
        {
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                movement.HandleMove(moveInput, sprintSpeed);
            }
            else if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
            {
                movement.HandleMove(moveInput, walkSpeed);
            }
            else
            {
                movement.HandleMove(moveInput, runSpeed);
            }
        }
        private void HandleLook()
        {
            sightControl.HandleLook(mouseSensitivity, lookInput);
        }
        private void HandelCombatState()
        {
            if (playerFsm.isCombat)
            {
                exitCombatTimer +=  Time.deltaTime;
            }
            if (exitCombatTimer >= exitCombatTime)
            {
                playerFsm.isCombat = false;
                exitCombatTimer = 0;
            }
        }
        
        public void OnMove()
        {
            moveInput = movement.OnMove("Move");
        }
        public void OnLook()
        {
            lookInput = sightControl.Onlook("Look");
        }
        public void OnAttack(string actionName, InputControl inputControl)
        {
            if(actionName != "Attack")return;
            
            attackSetting.InputKeySetting(actionName, inputControl, attackAction);
            exitCombatTimer = 0;
        }
        public void OnInteraction()
        {
            
        }

        public void OnEnableAttack()
        {
            attackSetting.OnEnableAttack();
        }
        public bool CheckHasWeapon()
        {
            return weaponHandel.HasWeapon;
        }
        public void BeginCoroutine(IEnumerator coroutine)
        {
            StartCoroutine(coroutine);
        }

        private void OnEnable()
        {
            InputManager.Instance.OnInputPressedWithControl += OnAttack;
        }

        private void OnDisable()
        {
            InputManager.Instance.OnInputPressedWithControl -= OnAttack;
        }
        
    }
    
}
