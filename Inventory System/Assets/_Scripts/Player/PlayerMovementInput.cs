using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

[RequireComponent(typeof(CharacterController))]

public class PlayerMovementInput : MonoBehaviour
{
    #region Player Movement
    [Header("Player")]
    public float MoveSpeed = 2.0f;
    public float SprintSpeed = 5.335f;

    [Range(0.0f, 0.3f)]
    public float RotationSmoothTime = 0.12f;
    public float SpeedChangeRate = 10.0f;
    private GameObject _mainCamera;

    //Player fields
    private float _speed;
    private float _targetRotation = 0.0f;
    private float _rotationVelocity;
    private float _verticalVelocity;
    public float _sensitivity = 1f;

    private bool _rotateOnMove = true;
    #endregion

    #region Cinemachine
    [Header("Cinemachine")]
    public GameObject CinemachineCameraTarget;
    public float TopClamp = 70.0f;
    public float BottomClamp = -30.0f;
    public float CameraAngleOverride = 0.0f;


    public bool LockCameraPosition = false;

    // cinemachine
    private float _cinemachineTargetYaw;
    private float _cinemachineTargetPitch;
    private const float _threshold = 0.01f;
    #endregion

    #region Player Grounded
    [Header("Player Grounded")]
    public bool Grounded = true;
    public float GroundedOffset = -0.14f;
    public float GroundedRadius = 0.28f;

    public LayerMask GroundLayers;
    #endregion

    private InputsManager _input;
    private CharacterController _controller;

    #region Player Anim
    [Header("Player Animation")]
    [Space(10)]

    private Animator _animator;

    private string WALK_ANIM_PARAM = "speed";
    private string GROUND_ANIM_PARAM = "grounded";
    private string JUMP_ANIM_PARAM = "jump";
    private string FREEFALL_ANIM_PARAM = "freefall";

    // animation IDs
    [HideInInspector] public int _animIDSpeed;
    [HideInInspector] public int _animIDGrounded;
    [HideInInspector] public int _animIDJump;
    [HideInInspector] public int _animIDFreeFall;
    [HideInInspector] public int _animIDMotionSpeed;
    private float _animationBlend;

    public bool _hasAnimator;
    #endregion
    private PlayerInput _playerInput;

    private PlayerJump _playerJump;


    private void Awake()
    {
        // get a reference to our main camera
        if (_mainCamera == null)
        {
            _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        }
        AssignAnimationIDs();
    }



    void Start()
    {
        _cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;
        _input = InputsManager.instance;
        _controller = GetComponent<CharacterController>();
        _playerInput = InputsManager.instance.gameObject.GetComponent<PlayerInput>();
        _playerJump = GetComponent<PlayerJump>();
        if (_playerJump != null)
            _playerJump.OnAddingGravity += AddingGravity;
        else
            _verticalVelocity = -15.0f;

        if (_animator != null)
        {
            _hasAnimator = true;
        }
        else
        {
            _hasAnimator = false;
        }
    }

    private void AssignAnimationIDs()
    {
        _animIDSpeed = Animator.StringToHash(WALK_ANIM_PARAM);
        _animIDGrounded = Animator.StringToHash(GROUND_ANIM_PARAM);
        _animIDJump = Animator.StringToHash(JUMP_ANIM_PARAM);
        _animIDFreeFall = Animator.StringToHash(FREEFALL_ANIM_PARAM);
        
    }
    private void AddingGravity(float jumpFloat)
    {
        _verticalVelocity = jumpFloat;
    }

    private bool IsCurrentDeviceMouse
    {
        get
        {
            if (_playerInput != null)
                return _playerInput.currentControlScheme == "KeyboardMouse";
            else
                return false;
        }
    }

    void Update()
    {
        GroundedCheck();
        Move();
    }

    private void GroundedCheck()
    {
        // set sphere position, with offset
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset,
            transform.position.z);
        Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers,
            QueryTriggerInteraction.Ignore);

        // update the player animator using character
        if (_hasAnimator)
        {
            _animator.SetBool(_animIDGrounded, Grounded);
        }
    }
    private void LateUpdate()
    {
        CameraRotation();
    }
    private void CameraRotation()
    {
        // if there is an input and camera position is not fixed
        if (_input.Look.sqrMagnitude >= _threshold && !LockCameraPosition)
        {
            _cinemachineTargetYaw += _input.Look.x * _sensitivity;
            _cinemachineTargetPitch += _input.Look.y * _sensitivity;
        }

        // clamp our rotations so our values are limited 360 degrees
        _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
        _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);


        // Cinemachine will follow this target
        CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride,
            _cinemachineTargetYaw, 0.0f);
    }

    private void Move()
    {

        float targetSpeed = _input.IsSprint ? SprintSpeed : MoveSpeed;

        if (_input.Move == Vector2.zero) targetSpeed = 0.0f;

        // a reference to the players current horizontal velocity
        float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

        float speedOffset = 0.1f;
        float inputMagnitude = _input.analogMovement ? _input.Move.magnitude : 1f;

        if (currentHorizontalSpeed < targetSpeed - speedOffset ||
            currentHorizontalSpeed > targetSpeed + speedOffset)
        {
            
            _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                Time.deltaTime * SpeedChangeRate);

         
            _speed = Mathf.Round(_speed * 1000f) / 1000f;
        }
        else
        {
            _speed = targetSpeed;
        }

        _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
        if (_animationBlend < 0.01f) _animationBlend = 0f;

        // normalise input direction
        Vector3 inputDirection = new Vector3(_input.Move.x, 0.0f, _input.Move.y).normalized;


        // if there is a move input rotate player when the player is moving
        if (_input.Move != Vector2.zero)
        {
            _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                              _mainCamera.transform.eulerAngles.y;
            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity,
                RotationSmoothTime);

            // rotate to face input direction relative to camera position only when player is not aiming
            if (_rotateOnMove)
            {
                transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            }
        }


        Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

        // move the player
        _controller.Move(targetDirection.normalized * (_speed * Time.deltaTime) +
                         new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);

        // update animator if using character
        if (_hasAnimator)
        {
            _animator.SetFloat(_animIDSpeed, _animationBlend);
            
        }
    }

    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }
    private void OnDrawGizmosSelected()
    {
        Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
        Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

        if (Grounded) Gizmos.color = transparentGreen;
        else Gizmos.color = transparentRed;

        // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
        Gizmos.DrawSphere(
            new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z),
            GroundedRadius);
    }

    public void SetSensitivity(float Sensitivity)
    {
        _sensitivity = Sensitivity;
    }

    public void SetRotateOnMove(bool RotateOnMove)
    {
        _rotateOnMove = RotateOnMove;
    }

}
