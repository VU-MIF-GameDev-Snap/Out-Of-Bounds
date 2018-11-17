using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// VERY TEMPORARY PLS DELETE AS SOON AS POSSIBLE!!
using UnityEngine.SceneManagement;


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
    public float DashDistance = 5f;
    public float KnockbackFactor = 0.002f;
    public Vector3 Drag;

    private CharacterController _controller;
    private Vector3 _velocity;
    private Animator _animator;
    private AimIK _aimIK;
    private PlayerInputManager _inputManager;
    private ICharacterPowerController _powerController;
    private AudioSource _deathSound;
    private float _deathTime;
    private Dictionary<PlayerAbilities, bool> _abilitiesAvailable = new Dictionary<PlayerAbilities, bool>();
    public enum PlayerAbilities
    {
        Walk,
        Jump,
        Dash,
        Punch,
        Shoot,
        Aim,
        PickupWeapon,
        Power1,
        Power2,
    };

    public void AbilityToggle (PlayerAbilities ability, Nullable<bool> toggle = null)
    {
        if(!_abilitiesAvailable.ContainsKey(ability))
        {
            _abilitiesAvailable.Add(ability, toggle.HasValue ? toggle.Value : false);
            return;
        }
        _abilitiesAvailable[ability] = toggle.HasValue ? toggle.Value : !_abilitiesAvailable[ability];
    }

    public bool AbilityCheck (PlayerAbilities ability)
    {
        bool abilityAvailable = true;
        if(!_abilitiesAvailable.TryGetValue(ability, out abilityAvailable))
        {
            // Ability not in list, return true by default.
            abilityAvailable = true;
        }
        return abilityAvailable;
    }

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
        if(AbilityCheck(PlayerAbilities.Jump))
        {
            if (_inputManager.IsButtonDown(PlayerInputManager.Key.Jump) && _controller.isGrounded)
                Jump();
        }

        var aimDirection = _inputManager.GetAimDirection();
        _animator.SetBool("IsGrounded", _controller.isGrounded);
        _animator.SetBool("HasRifle", _weapon != null);

        if(AbilityCheck(PlayerAbilities.Jump))
        {
            if (_inputManager.IsButtonDown(PlayerInputManager.Key.Dash))
                Dash(aimDirection);
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

        // Force z-axis lock
        transform.position = new Vector3(transform.position.x, transform.position.y, 0);

        _velocity.x /= 1 + Drag.x * Time.deltaTime;
        _velocity.y /= 1 + Drag.y * Time.deltaTime;
        _controller.Move(_velocity * Time.deltaTime);

        if(AbilityCheck(PlayerAbilities.Walk))
        {
            var horizontalInput = _inputManager.GetAxis(PlayerInputManager.Key.MoveHorizontal);
            _animator.SetFloat("Speed", Mathf.Abs(horizontalInput));
            var movementDirection = new Vector3(horizontalInput, 0, 0);
            _controller.Move(movementDirection * Speed * Time.deltaTime);
        }

        if(AbilityCheck(PlayerAbilities.Aim))
        {
            _aimIK.TargetDirection = aimDirection;
            if (aimDirection.x != 0)
                transform.rotation = Quaternion.LookRotation(new Vector3(aimDirection.x, 0, 0));
        }

        if(AbilityCheck(PlayerAbilities.Punch))
        {
            if (_inputManager.IsButtonPressed(PlayerInputManager.Key.Punch))
                Hit(_rightFist, HitType.Punch, PunchDamage, KnockValue);
        }

        if(AbilityCheck(PlayerAbilities.Power1))
        {
            if (_inputManager.IsButtonPressed(PlayerInputManager.Key.Power1))
                _powerController.StartPower1();
        }

        if(AbilityCheck(PlayerAbilities.Shoot))
        {
            if (_inputManager.IsButtonPressed(PlayerInputManager.Key.Shoot))
                Shoot();
        }


        if(_deathTime > 0 && _deathTime <= Time.time)
        {
            Destroy(gameObject);
            // THIS SHOULD NOT BE HERE DELETE THIS NOW
            SceneManager.LoadScene(1);
        }
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
            (Mathf.Log(1f / (Time.deltaTime * Drag.y + 1)) / -Time.deltaTime),
            0)
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

        _weapon.Fire();
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
        if(!AbilityCheck(PlayerAbilities.PickupWeapon))
        {
            return;
        }

        // TODO: Won't work if weapon is on the ground
        if (collision.gameObject.CompareTag("Weapon"))
        {
            OnWeaponPickup(collision.gameObject);
        }
    }
}
