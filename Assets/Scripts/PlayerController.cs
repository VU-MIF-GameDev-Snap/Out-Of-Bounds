using System;
using System.Collections;

using UnityEngine;


[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInputManager))]
[RequireComponent(typeof(AudioSource))]
public class PlayerController : MonoBehaviour 
{
    [Header("Player's body parts")]
    public GameObject RightFist; // USE: for punch events
    private HitEvent _rightFist;

    [Header("Player's hitpoints")]
    public int hitpoints = 0;
    public int maxHitpoints = 300;

    [Header("Player's damage")]
    public int PunchDamage = 5;
    public int KnockValue = 5;
    //public Vector3 KnockDirection = GameObject.transform.forward;

    private GameObject _weapon;


    [Header("Player controller variables")]
    public float Speed = 5f;
    public float JumpHeight = 2f;
    public float Gravity = -9.81f;
    public float DashDistance = 5f;
    public float KnockbackFactor = 0.002f;
    public Vector3 Drag;

    [Header("Power1")]
    public GameObject Rocket;
    public Transform RocketSpawn1;
    public Transform RocketSpawn2;
    public float Power1Cooldown;

    private CharacterController _controller;
    private Vector3 _velocity;
    private Animator _animator;
    private PlayerInputManager _inputManager;
    private AudioSource _deathSound;
    private float _deathTime;

    // for Power1
    private float _timeStamp = 0;

    void Start ()
    {
        _animator = GetComponent<Animator>();
        _controller = GetComponent<CharacterController>();
        _inputManager = GetComponent<PlayerInputManager>();
        _deathSound = GetComponent<AudioSource>();

        _rightFist = RightFist ? RightFist.GetComponent<HitEvent>() : null;
    }

    void Update ()
    {
        if (_inputManager.IsButtonDown(PlayerInputManager.Key.Jump) && _controller.isGrounded)
            Jump();

        var horizontalInput = _inputManager.GetAxis(PlayerInputManager.Key.MoveHorizontal);
        var direction = new Vector3(horizontalInput, 0, 0);
        _animator.SetFloat ("Speed", Mathf.Abs(horizontalInput));
        _animator.SetBool ("IsGrounded", _controller.isGrounded);

        if (_inputManager.IsButtonDown(PlayerInputManager.Key.Dash))
            Dash(direction);

        if (_controller.isGrounded && _velocity.y < 0)
        {
            // When on ground remove downward gravity pull
            _velocity.y = 0f;
        }
        if (!_controller.isGrounded)
        {
            _velocity.y += Gravity * Time.deltaTime;
        }

        _velocity.x /= 1 + Drag.x * Time.deltaTime;
        _velocity.y /= 1 + Drag.y * Time.deltaTime;
        // Debug.Log("velo: " + _velocity + " + grounded: " + _controller.isGrounded);
        _controller.Move((_velocity + (direction * Speed)) * Time.deltaTime);
        
        if (direction != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(direction);

        if (_inputManager.IsButtonPressed(PlayerInputManager.Key.Punch))
            Hit(_rightFist, HitType.Punch, PunchDamage, KnockValue);

        if (_inputManager.IsButtonPressed(PlayerInputManager.Key.Power1))
            Power1();

        if (_inputManager.IsButtonPressed(PlayerInputManager.Key.Shoot))
            Shoot();

        if(_deathTime > 0 && _deathTime <= Time.time)
            Destroy(gameObject);
    }

    private void Jump ()
    {
        _velocity.y += Mathf.Sqrt(JumpHeight * -2f * Gravity);
        Debug.Log("jump");
        _animator.SetTrigger("Jump");
    }

    private void Dash (Vector3 direction)
    {
        Debug.Log("Dash");
        var dashingVelocity = Vector3.Scale(
            direction, 
            DashDistance * new Vector3((Mathf.Log(1f / (Time.deltaTime * Drag.x + 1)) / -Time.deltaTime), 
            0, 
            (Mathf.Log(1f / (Time.deltaTime * Drag.z + 1)) / -Time.deltaTime))
        );
        _velocity += dashingVelocity;
        Debug.Log("dashing velo: " + dashingVelocity);
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

        _weapon.SendMessage("Fire");
    }

    private void Power1()
    {
        if (_timeStamp <= Time.time)
        {
            _timeStamp = Time.time + Power1Cooldown;

            var rocket1 = Instantiate (Rocket, RocketSpawn1.position, RocketSpawn1.rotation);
            var rocket2 = Instantiate (Rocket, RocketSpawn2.position, RocketSpawn2.rotation);

            rocket1.SendMessage("Initialize", transform.root.gameObject);
            rocket2.SendMessage("Initialize", transform.root.gameObject);
        }
    }

    // --------------------------------------------
    // ------------------ EVENTS ------------------
    // --------------------------------------------
    public void OnHit(object message)
    {
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

        // Will stick when player jumps instead of detaching
        weapon.GetComponent<Rigidbody>().isKinematic = true;
        // Make sure the weapon faces the same direction as the player
        var playerDirection = transform.forward;
        weapon.transform.forward = playerDirection;
        // Stick it to the player's body
        weapon.transform.SetParent(this.transform, true);
        _weapon = weapon;

        Debug.Log(gameObject.name + " picked up a " + weapon.name);
    }
}