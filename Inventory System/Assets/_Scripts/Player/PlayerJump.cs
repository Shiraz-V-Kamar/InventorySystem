using System;
using UnityEngine;
using UnityEngine.Windows;

public class PlayerJump : MonoBehaviour
{
    PlayerMovementInput playerMovementInput;
    public bool Grounded;

    

    // timeout deltatime
    private float _jumpTimeoutDelta;
    private float _fallTimeoutDelta;
    private float _verticalVelocity;
    private float _terminalVelocity = 53.0f;


    private InputsManager _input;

    [Space(10)]
    public float JumpHeight = 1.2f;
    public float Gravity = -15.0f;

    [Space(10)]
    public float JumpTimeout = 0.50f;
    public float FallTimeout = 0.15f;

    #region Player Animation
    [Header("Player Animation")]
    [SerializeField]private Animator _animator;

    // animation IDs
    private int _animIDSpeed;
    private int _animIDGrounded;
    private int _animIDJump;
    private int _animIDFreeFall;

    [SerializeField]private bool _jumphasAnimator;
    
    #endregion

    #region Events
    public Action<float> OnAddingGravity;
    #endregion
    private void Start()
    {
        playerMovementInput = GetComponent<PlayerMovementInput>();
       
        _input = InputsManager.instance;

        _animIDJump = playerMovementInput._animIDJump;
        _animIDJump = playerMovementInput._animIDFreeFall;
        if(_animator!= null)
        {
            _jumphasAnimator = true;
        }
        else
        {
            _jumphasAnimator = false;
        }
    }


    private void Update()
    {
        JumpAndGravity();
    }

    private void JumpAndGravity()
    {
        Grounded = playerMovementInput.Grounded;
        if (Grounded)
        {
            // reset the fall timeout timer
            _fallTimeoutDelta = FallTimeout;

            // update animator if using character
     /*       if (_jumphasAnimator)
            {
                _animator.SetBool(_animIDJump, false);
                _animator.SetBool(_animIDFreeFall, false);
            }*/

            // stop our velocity dropping infinitely when grounded
            if (_verticalVelocity < 0.0f)
            {
                _verticalVelocity = -2f;
            }

            // Jump
            if (_input.IsJump && _jumpTimeoutDelta <= 0.0f)
            {
                // the square root of H * -2 * G = how much velocity needed to reach desired height
                _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);

                // update animator if using character
             /*   if (_jumphasAnimator)
                {
                    _animator.SetBool(_animIDJump, true);
                }*/
            }
            

            // jump timeout
            if (_jumpTimeoutDelta >= 0.0f)
            {
                _jumpTimeoutDelta -= Time.deltaTime;
            }

            
        }
        else
        {
            // reset the jump timeout timer
            _jumpTimeoutDelta = JumpTimeout;

            // fall timeout
            if (_fallTimeoutDelta >= 0.0f)
            {
                _fallTimeoutDelta -= Time.deltaTime;
            }
            else
            {
                // update animator if using character
                /*if (_jumphasAnimator)
                {
                    _animator.SetBool(_animIDFreeFall, true);
                }*/
            }

            // if we are not grounded, do not jump
            _input.IsJump = false;
        }

        // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
        if (_verticalVelocity < _terminalVelocity)
        {
            _verticalVelocity += Gravity * Time.deltaTime;

        }
        OnAddingGravity?.Invoke(_verticalVelocity);
    }
}
