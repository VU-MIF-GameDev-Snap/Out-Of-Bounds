using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInputManager))]
[RequireComponent(typeof(PlayerAudioController))]
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
	public float GroundedDrag = 1.5f;
	[Header("Player controller variables")]
	public float DashDuration = 0.2f;
	public float DashDistance = 5f;
	public float DashCooldown = 2f;

	[Header("For External Scripts")]
	public float KnockbackResistance = 0f;
	public float DamageResistance = 0f;
	[Header("Audio")]
	public AudioSource AudioSource;

	private CharacterController _controller;
	private Vector3 _velocity;
	public Vector3 Velocity { get{return _velocity;} set{_velocity = value;} }
	private Animator _animator;
	private AimIK _aimIK;
	private PlayerInputManager _inputManager;
	private ICharacterPowerController _powerController;
	private PlayerAudioController _audioController;
	private float _deathTime;
	private Dictionary<PlayerAbility, bool> _abilitiesAvailable = new Dictionary<PlayerAbility, bool>();

	// Necessary for jump delay after landing
	private bool _previousGrounded;
	private float _landedTimeStamp = 0;

	// For jumping higher when holding button
	private bool _isJumping;
	private float _jumpTimeStamp = 0;

	// ability - dash
	private float _dashStartTime;
	private Vector3 _currentDashingVelocity;
	private bool _dashActive = false;

	public enum PlayerAbility
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

	public void AbilityToggle (PlayerAbility ability, Nullable<bool> toggle = null)
	{
		if (!_abilitiesAvailable.ContainsKey(ability))
		{
			_abilitiesAvailable.Add(ability, toggle.HasValue ? toggle.Value : false);
			return;
		}
		_abilitiesAvailable[ability] = toggle.HasValue ? toggle.Value : !_abilitiesAvailable[ability];
	}

	public bool AbilityCheck (PlayerAbility ability)
	{
		bool abilityAvailable = true;
		if (!_abilitiesAvailable.TryGetValue(ability, out abilityAvailable))
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
		_audioController = GetComponent<PlayerAudioController>();
		_powerController = GetComponent<ICharacterPowerController>();
		_rightFist = RightFist ? RightFist.GetComponent<HitEvent>() : null;
	}

	void Update ()
	{
		var horizontalInput = _inputManager.GetAxis(PlayerInputManager.Key.MoveHorizontal);
		if (!AbilityCheck(PlayerAbility.Walk))
		{
			horizontalInput = 0;
		}

		var movementDirection = new Vector3(horizontalInput, 0, 0);
		var aimDirection = _inputManager.GetAimDirection();

		ProcessButtonInput(aimDirection);

		var animMoveSpeed = horizontalInput * Math.Sign(aimDirection.x);
		animMoveSpeed = animMoveSpeed == 0 ? horizontalInput : animMoveSpeed;
		_animator.SetFloat("Speed", animMoveSpeed);
		_animator.SetBool("IsGrounded", _controller.isGrounded);
		_animator.SetBool("HasRifle", _weapon != null);

		if (AbilityCheck(PlayerAbility.Aim))
		{
			_aimIK.TargetDirection = aimDirection;
			if (aimDirection.x != 0)
				transform.rotation = Quaternion.LookRotation(new Vector3(aimDirection.x, 0, 0));
		}

		if (_controller.isGrounded && _velocity.y <= 0)
		{
			_velocity.y = Gravity * 0.01f;
		}
		else if (!_controller.isGrounded)
		{
			_velocity.y += Gravity * Time.deltaTime;
		}

		_velocity.x /= (1 + Drag.x * Time.deltaTime * (_controller.isGrounded ? GroundedDrag  : 1));
		_velocity.y /= 1 + Drag.y * Time.deltaTime;
		_velocity.z = 0;
		// Debug.Log("velo: " + _velocity + " + grounded: " + _controller.isGrounded);
		_controller.Move((_velocity + (movementDirection * Speed)) * Time.deltaTime);

		// Force z-axis lock
		transform.position = new Vector3(transform.position.x, transform.position.y, 0);

		if (_deathTime > 0 && _deathTime <= Time.time)
		{
			AudioSource.clip = _audioController.Death;
			AudioSource.Play();

			// Detach audio emitter from player, will be hang around in scene, shouldn't be a big deal.
			AudioSource.transform.SetParent(null);
			// Destroy player object right now.
			Destroy(gameObject);
		}
	}

	private void ProcessButtonInput (Vector2 aimDirection)
	{
		if (AbilityCheck(PlayerAbility.Jump))
		{
			if (_inputManager.IsButtonDown(PlayerInputManager.Key.Dash))
				Dash(aimDirection);
		}
		// It must be called every update
		var jumpAvailable = JumpAvailable();
		if ((jumpAvailable || _isJumping) && _inputManager.IsButtonDown(PlayerInputManager.Key.Jump) && AbilityCheck(PlayerAbility.Jump))
			Jump();
		else
			_isJumping = false;

		if (AbilityCheck(PlayerAbility.Punch))
		{
			if (_inputManager.IsButtonPressed(PlayerInputManager.Key.Punch))
				Hit(_rightFist, HitType.Punch, PunchDamage, KnockValue);
		}

		if (AbilityCheck(PlayerAbility.Power1))
		{
			if (_inputManager.IsButtonPressed(PlayerInputManager.Key.Power1))
				_powerController.StartPower1();
		}

		if (AbilityCheck(PlayerAbility.Power2))
		{
			if (_inputManager.IsButtonPressed(PlayerInputManager.Key.Power2))
				_powerController.StartPower2();
		}

		if (AbilityCheck(PlayerAbility.Shoot))
		{
			if (_inputManager.IsButtonPressed(PlayerInputManager.Key.Shoot))
				Shoot();
		}


		if (_inputManager.IsButtonDown(PlayerInputManager.Key.Dash))
			Dash(aimDirection);
		HandleDashing();
	}

	private bool JumpAvailable ()
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

			AudioSource.clip = _audioController.Jump;
			AudioSource.Play();

			return;
		}

		// If player should stop jumping
		if (Time.time > _jumpTimeStamp)
		{
			_isJumping = false;
			return;
		}

		// If player is continuing to hold jump button
		_velocity.y += Mathf.Sqrt(JumpHeight * -2f * Gravity) * Time.deltaTime * JumpHoldStrength;


	}

	private void Dash (Vector3 direction)
	{
		if (_dashStartTime + DashDuration + DashCooldown > Time.time)
		{
			// Can't yet dash.
			return;
		}

		Debug.Log("Dash start");
		_dashStartTime = Time.time;
		_currentDashingVelocity = direction * DashDistance;
		_dashActive = true;
		_animator.SetBool("Dashing", _dashActive);

		AudioSource.clip = _audioController.Dash;
		AudioSource.Play();

		// _velocity += dashingVelocity;
		// Debug.Log("dashing velo: " + dashingVelocity);
	}

	private void HandleDashing ()
	{
		if (!_dashActive)
		{
			return;
		}

		_velocity = _currentDashingVelocity;
		_controller.Move(_currentDashingVelocity * Time.deltaTime);

		if (_dashStartTime + DashDuration < Time.time)
		{
			// Dash has ended.
			_dashActive = false;
			_animator.SetBool("Dashing", _dashActive);
			return;
		}
	}

	// You use your '_rightHand' to 'HitType.Punch' and deal '50' damage
	private void Hit (HitEvent bodyPart, HitType type, int damage, int knockValue)
	{
		var msg = new HitMessage()
		{ HitType = type, Damage = damage, KnockbackValue = knockValue, KnockbackDirection = transform.forward };
		bodyPart.Initialise(msg);

		// Debug.Log("Type: " + hit + " Damage: " + damage);
		_animator.SetTrigger(type.ToString());

		AudioSource.clip = _audioController.Attack;
		AudioSource.Play();
	}

	public void OnExitArena ()
	{
		_deathTime = Time.time + 2;
		// Should start something visual to indicate that time is running out.
	}

	public void OnReenterArena ()
	{
		_deathTime = 0;
	}

	private void Shoot ()
	{
		if (_weapon == null)
			return;

		_weapon.Fire();
	}

	// --------------------------------------------
	// ------------------ EVENTS ------------------
	// --------------------------------------------
	public void OnHit (HitMessage message)
	{
		if (_dashActive)
		{
			// Invincibility while dashing.
			return;
		}

		var damageResistanceValue = Mathf.Clamp(1 - DamageResistance, 0, 1);
		var knockbakcResistanceValue = Mathf.Clamp(1 - KnockbackResistance, 0, 1);
		message.KnockbackValue = (int)(message.KnockbackValue * knockbakcResistanceValue);
		message.Damage = (int)(message.Damage * damageResistanceValue);

		int rand = UnityEngine.Random.Range(1, 4);
		if (rand == 1)
		{
			AudioSource.clip = _audioController.Hit_1;
			AudioSource.Play();
		}
		if (rand == 2)
		{
			AudioSource.clip = _audioController.Hit_2;
			AudioSource.Play();
		}
		if (rand == 3)
		{
			AudioSource.clip = _audioController.Hit_3;
			AudioSource.Play();
		}

		if (hitpoints < maxHitpoints)
		{
			if (message.Damage + hitpoints <= maxHitpoints)
				hitpoints += message.Damage;
			else if (message.Damage + hitpoints > maxHitpoints)
				hitpoints = maxHitpoints;
		}

		if (message.KnockbackValue >= 0)
			_velocity += message.KnockbackDirection * message.KnockbackValue * (hitpoints + 1) * KnockbackFactor;

		Debug.Log(this.name + " got hit by a '" + message.HitType + "' and received '" + message.Damage + "' damage");
		Debug.Log(" Player HP: '" + hitpoints);
		Debug.Log(" Player Velocity: '" + _velocity);
	}

	public void OnHit (object message)
	{
		var msg = message as HitMessage;
		if (msg == null)
			return;
		OnHit(msg);
	}

	public void OnWeaponPickup (GameObject weapon)
	{
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

	public void ReduceHitpoints (int amount)
	{
		hitpoints = Mathf.Clamp(hitpoints - amount, 0, maxHitpoints);
	}

	private void OnTriggerEnter (Collider collision)
	{
		if (!AbilityCheck(PlayerAbility.PickupWeapon))
		{
			return;
		}

		if (collision.gameObject.CompareTag("Weapon"))
		{
			if (collision.gameObject.transform.parent != null)
			{
				return;
			}
			OnWeaponPickup(collision.gameObject);
		}
	}
}
