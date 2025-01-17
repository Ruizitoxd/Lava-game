using System;
using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour, IPlayerController
{
    //References
    [Header("References")]
    public PlayerMovementStats movementStats;
    [SerializeField] private CapsuleCollider2D _bodyCollider;
    private Rigidbody2D _rb;
    private FrameInput _frameInput;
    private Vector2 _frameVelocity;
    private bool _cachedQueryStartInColliders;
    [SerializeField] private ParticleSystem particulas;

    #region interface

    public Vector2 FrameInput => _frameInput.Move;
    public event Action<bool, float> groundedChanged;
    public event Action jumped;

    #endregion

    //Movement variables
    private float _time;
    private bool _isFacingRight;

    //Jumping variables
    private bool _jumpToConsume;
    private bool _bufferedJumpUsable;
    private bool _endedJumpEarly;
    private bool _coyoteUsable;
    private float _timeJumpWasPressed;

    //Dashing variables
    private bool _dashToConsume;
    private bool _isDashing;

    private bool HasBufferedJump => _bufferedJumpUsable && _time < _timeJumpWasPressed + movementStats.jumpBufferTime;
    private bool CanUseCoyote => _coyoteUsable && !_isGrounded && _time < _frameLeftGrounded + movementStats.jumpCoyoteTime;


    private bool _isGrounded;
    private float _frameLeftGrounded = float.MinValue;

    void Awake()
    {
        _isFacingRight = true;
        _rb = GetComponent<Rigidbody2D>();

        _cachedQueryStartInColliders = Physics2D.queriesStartInColliders;
    }

    private void Update()
    {
        _time += Time.deltaTime;
        GatherInput();
    }

    void FixedUpdate()
    {
        if (_isDashing)
        {
            return;
        }

        //Check Colissions
        CheckCollisions();

        if (_isGrounded)
        {
            Move(movementStats.groundAcceleration, movementStats.groundDeceleration);
        }
        else
        {
            Move(movementStats.airAcceleration, movementStats.airDeceleration);
        }

        HandleJump();
        HandleGravity();

        ApplyMovement();
    }

    private void GatherInput()
    {
        _frameInput = new FrameInput
        {
            jumpDown = Input.GetButtonDown("Jump"),
            jumpHeld = Input.GetButton("Jump"),
            Move = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")),
            dashDown = Input.GetButtonDown("Dash")
        };

        if (_frameInput.jumpDown)
        {
            _jumpToConsume = true;
            _timeJumpWasPressed = _time;
        }

        if (_frameInput.dashDown)
        {
            _dashToConsume = true;
            if (_dashToConsume && !_isDashing)
                StartCoroutine("Dash");
        }
    }

    #region Movement
    private void Move(float acceleration, float deceleration)
    {
        Vector2 targetVelocity = Vector2.zero;
        if (_frameInput.Move != Vector2.zero)
        {
            //Check if he needs to turn
            turnCheck(_frameInput.Move.x);

            targetVelocity = Input.GetButton("Run") ? new Vector2(_frameInput.Move.x, 0f) * movementStats.maxRunSpeed : new Vector2(_frameInput.Move.x, 0f) * movementStats.maxWalkSpeed;

            _frameVelocity = Vector2.Lerp(_frameVelocity, targetVelocity, acceleration * Time.fixedDeltaTime);
        }
        else
        {
            _frameVelocity = Vector2.Lerp(_frameVelocity, Vector2.zero, deceleration * Time.fixedDeltaTime);
        }
    }

    private void turnCheck(float moveInput)
    {
        if (moveInput == transform.localScale.x * -1)
        {
            flip();
        }

    }

    private void flip()
    {
        _isFacingRight = !_isFacingRight;
        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
        if (_isGrounded)
        {
            particulas.Play();
        }
    }

    private IEnumerator Dash()
    {
        _dashToConsume = false;
        _isDashing = !_isDashing;
        float originalGravity = _frameVelocity.y;
        if (_frameInput.Move == Vector2.zero)
        {
            _frameVelocity = new Vector2(transform.localScale.x * movementStats.dashPower, 0f);
        }
        else
        {
            _frameVelocity = _frameInput.Move * movementStats.dashPower;
        }
        ApplyMovement();
        particulas.Play();
        yield return new WaitForSeconds(movementStats.dashTime);
        _frameVelocity.y = originalGravity;
        _isDashing = false;
    }
    #endregion

    #region Jump

    private void HandleJump()
    {
        if (!_endedJumpEarly && !_isGrounded && !_frameInput.jumpHeld && _rb.linearVelocityY > 0) _endedJumpEarly = true;

        if (!_jumpToConsume && !HasBufferedJump) return;

        if (_isGrounded || CanUseCoyote) ExecuteJump();

        _jumpToConsume = false;
    }

    private void ExecuteJump()
    {
        _endedJumpEarly = false;
        _timeJumpWasPressed = 0;
        _bufferedJumpUsable = false;
        _coyoteUsable = false;
        _frameVelocity.y = movementStats.jumpPower;
        jumped?.Invoke();
        particulas.Play();
    }
    public void JumpBoost(float jumpBoost)
    {
        _frameVelocity.y = jumpBoost;
        ApplyMovement();
    }

    private void HandleGravity()
    {
        if (_isGrounded && _frameVelocity.y <= 0f)
        {
            _frameVelocity.y = movementStats.groundingForce;
        }
        else
        {
            var inAirGravity = movementStats.fallAcceleration;
            if (_endedJumpEarly && _frameVelocity.y > 0) inAirGravity *= movementStats.JumpEndEarlyGravityModifier;
            _frameVelocity.y = Mathf.MoveTowards(_frameVelocity.y, -movementStats.maxFallSpeed, inAirGravity * Time.fixedDeltaTime);
        }


    }

    private void ApplyMovement()
    {
        _rb.linearVelocity = _frameVelocity;
        //_rb.linearVelocityX = _frameVelocity.x;
        //_rb.linearVelocityY = _frameVelocity.y;
    }
    #endregion

    #region Collision Checks
    private void CheckCollisions()
    {
        Physics2D.queriesStartInColliders = false;

        bool _groundHit = Physics2D.CapsuleCast(_bodyCollider.bounds.center, _bodyCollider.size, _bodyCollider.direction, 0, Vector2.down, movementStats.colissionDetectionRayLenght, movementStats.groundLayer);
        bool _headHit = Physics2D.CapsuleCast(_bodyCollider.bounds.center, _bodyCollider.size, _bodyCollider.direction, 0, Vector2.up, movementStats.colissionDetectionRayLenght, movementStats.groundLayer);

        if (_headHit)
        {
            _frameVelocity.y = Mathf.Min(0, _frameVelocity.y);
        }

        if (!_isGrounded && _groundHit)
        {
            _isGrounded = true;
            _coyoteUsable = true;
            _bufferedJumpUsable = true;
            _endedJumpEarly = false;
            _dashToConsume = true;
            groundedChanged?.Invoke(true, Mathf.Abs(_frameVelocity.y));
        }
        if (_isGrounded && !_groundHit)
        {
            _isGrounded = false;
            _frameLeftGrounded = _time;
            groundedChanged?.Invoke(false, 0);
        }

        Physics2D.queriesStartInColliders = _cachedQueryStartInColliders;
    }
    #endregion
}

public struct FrameInput
{
    public bool jumpDown;
    public bool jumpHeld;
    public Vector2 Move;
    public bool dashDown;
}

public interface IPlayerController
{
    public event Action<bool, float> groundedChanged;
    public event Action jumped;
    public Vector2 FrameInput { get; }
}