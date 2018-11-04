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
    public GameObject RightFist; 
    private HitEvent _rightFist;


    [Header("Player's damage")]
    public int PunchDamage = 54;


    [Header("Player controller variables")]
    public float Speed = 5f;
    public float JumpHeight = 2f;
    public float Gravity = -9.81f;
    public float DashDistance = 5f;
    public Vector3 Drag;


    private CharacterController _controller;
    private Vector3 _velocity;
    private Animator _animator;
    private PlayerInputManager _inputManager;
    private AudioSource _deathSound;
    private float _deathTime;


    void Start ()
    {
        _animator = GetComponent<Animator>();
        _controller = GetComponent<CharacterController>();
        _inputManager = GetComponent<PlayerInputManager>();
        _rightFist = RightFist ? RightFist.GetComponent<HitEvent>() : null;
        _deathSound = GetComponent<AudioSource>();
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

        if (Input.GetKeyDown(KeyCode.Tab))
            Hit(_rightFist, HitType.Punch, PunchDamage);

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
    private void Hit(HitEvent bodyPart, HitType type, int damage)
    {
        var msg = new HitMessage() { HitType = type, Damage = damage };
        bodyPart.SendMessage("Initialise", msg);

        // Debug.Log("Type: " + hit + " Damage: " + damage);
        _animator.SetTrigger(type.ToString());
    }

    public void ReceiveHit(object message)
    {
        var msg = message as HitMessage;
        if (msg == null)
            return;

        Debug.Log(this.name + " got hit by a '" + msg.HitType + "' and received '" + msg.Damage + "' damage");
    }

    public void Die()
    {
        _deathSound.Play();
        //yield return new WaitForSeconds(1);
        // wait for 1 sec
        _deathTime = Time.time + 2;
    }
}