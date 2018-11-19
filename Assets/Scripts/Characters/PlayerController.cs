using System;
using System.Collections;

using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInputManager))]
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(ICharacterPowerController))]
public class PlayerController : MonoBehaviour
{
    [Header("Player's body parts")]
    public GameObject RightFist; // USE: for punch events
    [SerializeField]
    GameObject RiflePosition;
    private HitEvent _rightFist;

    [Header("Player's hitpoints")]
    public int hitpoints = 0;
    public int maxHitpoints = 300;

    [Header("Player's damage")]
    public int PunchDamage = 5;
    public int KnockValue = 5;
    //public Vector3 KnockDirection = GameObject.transform.forward;

    private WeaponController _weapon;


    [Header("Player controller variables")]
    public float Speed = 5f;
    public float JumpHeight = 2f;
    public float Gravity = -9.81f;
    public float KnockbackFactor = 0.002f;
    public float LandingDelay = 0.1f;
    public float JumpHoldDuration = 1f;
    public float JumpHoldStrength = 1f;
    
    public Vector3 Drag;
    [Header("Player controller variables")]
    public float DashDuration = 2f;
    public float DashDistance = 5f;
    public float DashDelay = 2f;

    private CharacterController _controller;
    private Vector3 _velocity;
    private Animator _animator;
    private AimIK _aimIK;
    private PlayerInputManager _inputManager;
    private ICharacterPowerController _powerController;
    private AudioSource _deathSound;
    private float _deathTime;
    
    // Necessary for jump delay after landing
    private bool _previousGrounded;
    private float _landedTimeStamp = 0;

    // For jumping higher when holding button
    private bool _isJumping;
    private float _jumpTimeStamp = 0;
    
    // ability - dash
    private float _dashStartTime;
    private Vector3 _currentDashingVelocity;

    void Start ()
    {
        _animator = GetComponent<Animator>();
        _aimIK = GetComponent<AimIK>();
        _controller = GetComponent<CharacterController>();
        _inputManager = GetComponent<PlayerInputManager>();
        _deathSound = GetComponent<AudioSource>();
        _powerController = GetComponent<ICharacterPowerController>();
        _rightFist = RightFist ? RightFist.GetComponent<HitEvent>() : null;
    }

    void Update ()
    {
        var horizontalInput = _inputManager.GetAxis(PlayerInputManager.Key.MoveHorizontal);
        var direction = new Vector3(horizontalInput, 0, 0);
        var aimDirection = _inputManager.GetAimDirection();

        ProcessButtonInput(aimDirection);
        

        _animator.SetFloat("Speed", Mathf.Abs(horizontalInput));
        _animator.SetBool("IsGrounded", _controller.isGrounded);
        _animator.SetBool("HasRifle", _weapon != null);


        _aimIK.TargetDirection = aimDirection;

        if (aimDirection.x != 0)
            transform.rotation = Quaternion.LookRotation(new Vector3(aimDirection.x, 0, 0));

        if (_controller.isGrounded && _velocity.y <= 0)
        {
            _velocity.y = Gravity * Time.deltaTime;
        }
        else if (!_controller.isGrounded)
        {
            _velocity.y += Gravity * Time.deltaTime;
        }


        _velocity.x /= (1 + Drag.x * Time.deltaTime) * (_controller.isGrounded ? 5 : 1);
        _velocity.y /= 1 + Drag.y * Time.deltaTime;
        // Debug.Log("velo: " + _velocity + " + grounded: " + _controller.isGrounded);
        _controller.Move((_velocity + (direction * Speed)) * Time.deltaTime);
        
        // Force z-axis lock
        transform.position = new Vector3(transform.position.x, transform.position.y, 0);


        if (_deathTime > 0 && _deathTime <= Time.time)
        {
            Destroy(gameObject);
        }
    }

    private void ProcessButtonInput(Vector2 aimDirection)
    {
        if (_inputManager.IsButtonDown(PlayerInputManager.Key.Dash))
            Dash(aimDirection);

        // It must be called every update
        var jumpAvailable = JumpAvailable();
        if ((jumpAvailable || _isJumping) && _inputManager.IsButtonDown(PlayerInputManager.Key.Jump))
            Jump();
        else
            _isJumping = false;

        if (_inputManager.IsButtonPressed(PlayerInputManager.Key.Punch))
            Hit(_rightFist, HitType.Punch, PunchDamage, KnockValue);

        if (_inputManager.IsButtonPressed(PlayerInputManager.Key.Power1))
            _powerController.StartPower1();

        if (_inputManager.IsButtonPressed(PlayerInputManager.Key.Shoot))
            Shoot();

        if (_inputManager.IsButtonDown(PlayerInputManager.Key.Dash))
            Dash(aimDirection);
        HandleDashing();
    }

    private bool JumpAvailable()
    {
        var grounded = _controller.isGrounded;
        var previousGrounded = _previousGrounded;
        _previousGrounded = grounded;

        if (grounded && !previousGrounded)
            _landedTimeStamp = Time.time + LandingDelay;

        if (grounded && Time.time >= _landedTimeStamp)
            return true;

        return false;
    }

    private void Jump ()
    {
        // If player just started to jump propel him upwards
        if (!_isJumping)
        {
            _velocity.y = Mathf.Sqrt(JumpHeight * -2f * Gravity);
            Debug.Log("jump");
            _animator.SetTrigger("Jump");

            _isJumping = true;
            _jumpTimeStamp = Time.time + JumpHoldDuration;
            return;
        }

        // If player should stop jumping
        if(Time.time > _jumpTimeStamp)
        {
            _isJumping = false;
            return;
        }

        // If player is continuing to hold jump button
        _velocity.y += Mathf.Sqrt(JumpHeight * -2f * Gravity) * Time.deltaTime * JumpHoldStrength;


    }

    private void Dash (Vector3 direction)
    {
        if(_dashStartTime + DashDelay > Time.time)
        {
            // Can't yet dash.
            return;
        }

        Debug.Log("Dash start");
        _dashStartTime = Time.time;
        _currentDashingVelocity = direction * DashDistance;

        // _velocity += dashingVelocity;
        // Debug.Log("dashing velo: " + dashingVelocity);
    }

    private void HandleDashing()
    {
        _animator.SetBool("Dashing", IsDashing);
        if(!IsDashing)
        {
            return;
        }
        _velocity = _currentDashingVelocity;
        _controller.Move(_currentDashingVelocity * Time.deltaTime);
    }

    public bool IsDashing
    {
        get
        {
            return _dashStartTime + DashDuration > Time.time;
        }
     }

    // You use your '_rightHand' to 'HitType.Punch' and deal '50' damage
    private void Hit(HitEvent bodyPart, HitType type, int damage, int knockValue)
    {
        var msg = new HitMessage()
        { HitType = type, Damage = damage, KnockbackValue = knockValue, KnockbackDirection = transform.forward };
        bodyPart.SendMessage("Initialise", msg);

        // Debug.Log("Type: " + hit + " Damage: " + damage);
        _animator.SetTrigger(type.ToString());
    }

    public void Die()
    {
        _deathSound.Play();
        //yield return new WaitForSeconds(1);
        // wait for 1 sec
        _deathTime = Time.time + 2;
    }

    private void Shoot()
    {
        if (_weapon == null)
            return;

        _weapon.Fire();
    }

    // --------------------------------------------
    // ------------------ EVENTS ------------------
    // --------------------------------------------
    public void OnHit(object message)
    {
        if(IsDashing)
        {
            // Invincibility while dashing.
            return;
        }

        var msg = message as HitMessage;
        if (msg == null)
            return;

        if (hitpoints < maxHitpoints)
        {
            if (msg.Damage + hitpoints <= maxHitpoints)
                hitpoints += msg.Damage;
            else if (msg.Damage + hitpoints > maxHitpoints)
                hitpoints = maxHitpoints;
        }

        if (msg.KnockbackValue >= 0 && msg.KnockbackValue <= 100)
            _velocity += msg.KnockbackDirection * msg.KnockbackValue * hitpoints * KnockbackFactor;

        Debug.Log(this.name + " got hit by a '" + msg.HitType + "' and received '" + msg.Damage + "' damage");
        Debug.Log(" Player HP: '" + hitpoints);
        Debug.Log(" Player Velocity: '" + _velocity);
    }

    public void OnWeaponPickup (object message)
    {
        var weapon = message as GameObject;
        if (weapon == null || _weapon != null)
            return;

        // Will make it stick when player jumps instead of detaching
        weapon.GetComponent<Rigidbody>().isKinematic = true;

        weapon.transform.position = RiflePosition.transform.position;
        weapon.transform.rotation = RiflePosition.transform.rotation;
        // Stick it to the player's hand
        weapon.transform.SetParent(RiflePosition.transform, true);
        _weapon = weapon.GetComponent<WeaponController>();

        _aimIK.TransformTargetForLeftHand = _weapon.GetLeftHandPosition();
        _aimIK.RifleHoldingMode = true;

        Debug.Log(gameObject.name + " picked up a " + weapon.name);
    }

    private void OnTriggerEnter(Collider collision)
    {
        // TODO: Won't work if weapon is on the ground
        if (collision.gameObject.CompareTag("Weapon"))
        {
            OnWeaponPickup(collision.gameObject);
        }
    }
}
