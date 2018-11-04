using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInputManager))]
public class PlayerController : MonoBehaviour {

    [Header("Player controller variables")]
    public float Speed = 5f;
    public float JumpHeight = 2f;
    public float Gravity = -9.81f;
    public float DashDistance = 5f;
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

    // for Power1
    private float _timeStamp = 0;

    void Start ()
    {
        _controller = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
        _inputManager = GetComponent<PlayerInputManager>();
    }

    void Update ()
    {
        if (_inputManager.IsButtonDown(PlayerInputManager.Key.Jump) && _controller.isGrounded)
        {
            _velocity.y += Mathf.Sqrt(JumpHeight * -2f * Gravity);
            Debug.Log("jump");
            _animator.SetTrigger("Jump");
        }   
        var horizontalInput = _inputManager.GetAxis(PlayerInputManager.Key.MoveHorizontal);
        Vector3 move = new Vector3(horizontalInput, 0, 0);
        _animator.SetFloat ("Speed", Mathf.Abs(horizontalInput));
        _animator.SetBool ("IsGrounded", _controller.isGrounded);

        if (_inputManager.IsButtonDown(PlayerInputManager.Key.Dash))
        {
            Debug.Log("Dash");
            var dashingVelocity = Vector3.Scale(move, DashDistance * new Vector3((Mathf.Log(1f / (Time.deltaTime * Drag.x + 1)) / -Time.deltaTime), 0, (Mathf.Log(1f / (Time.deltaTime * Drag.z + 1)) / -Time.deltaTime)));
            _velocity += dashingVelocity;
            Debug.Log("dashing velo: " + dashingVelocity);
        }

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
        _controller.Move((_velocity + (move * Speed)) * Time.deltaTime);
        
        if (move != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(move);

        if (_inputManager.IsButtonPressed(PlayerInputManager.Key.Power1))
        {
            Power1();
        }  
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
}
