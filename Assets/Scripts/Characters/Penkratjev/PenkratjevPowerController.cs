using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PenkratjevPowerController : MonoBehaviour, ICharacterPowerController
{
	[Header("Laser Power Configuration")]
	[SerializeField]
	private float _laserPowerDuration = 3f;
	[SerializeField]
	private int _laserPowerContinuousKnockbackValue = 10;
	[SerializeField]
	private float _laserPowerDamagePerSecond = 12f;
	[SerializeField]
	private float _laserPowerMaxDistance = 10f;
	[SerializeField]
	private float _laserPowerDelay = 7f;
	private float _laserPowerAvailableAt = 0f;
	private float _laserPowerStartedAt = 0f;
	private bool _laserPowerStarted = false;
	[SerializeField]
	private Transform _laserPowerStartPosition;
	[SerializeField]
	private ParticleSystem _laserPowerHitParticleSystem;
	private int _laserPowerAlreadyDamageDealtAmount = 0;
	[Header("Exchange Power Configuration")]
	[SerializeField]
	private float _exchangePowerDelay = 15f;
	[SerializeField]
	private float _exchangePowerMaxDistance = 4f;
	private float _exchangePowerAvailableAt = 0f;

	private PlayerInputManager _inputManager;
	[SerializeField]
	private LineRenderer _lineRenderer;



	public void StartPower1 ()
	{
		if (_laserPowerAvailableAt > Time.time)
		{
			return;
		}

		_laserPowerStarted = true;
		_laserPowerStartedAt = Time.time;
		_laserPowerAlreadyDamageDealtAmount = 0;
		_lineRenderer.enabled = true;
		// _laserPowerAvailableAt = Time.time + _laserPowerDelay;
		// Start laser.
	}

	public void HandleLaserPower ()
	{
		if (!_laserPowerStarted)
		{
			return;
		}

		var aimDirection = _inputManager.GetAimDirection();
		RaycastHit hit;
		var rayHasHit = Physics.Raycast(_laserPowerStartPosition.position, aimDirection, out hit, _laserPowerMaxDistance);

		_lineRenderer.SetPosition(0, _laserPowerStartPosition.position);
		if (rayHasHit)
		{
			_lineRenderer.SetPosition(1, hit.point);
			_laserPowerHitParticleSystem.transform.position = hit.point;
			_laserPowerHitParticleSystem.Play();
		}
		else
		{
			_lineRenderer.SetPosition(1, _laserPowerStartPosition.position + new Vector3(aimDirection.x, aimDirection.y, 0) * _laserPowerMaxDistance);
		}


		if (rayHasHit)
		{
			// cause particles at hit.point;
			// cause damage and knockback
			Debug.LogWarning(hit.collider.name);

			var duration = Time.time - _laserPowerStartedAt;
			int rightNowDoDamageAmount = (int)Math.Ceiling(duration * _laserPowerDamagePerSecond);
			if (rightNowDoDamageAmount - _laserPowerAlreadyDamageDealtAmount > 0)
			{
				var thisFrameDamage = rightNowDoDamageAmount - _laserPowerAlreadyDamageDealtAmount;
				_laserPowerAlreadyDamageDealtAmount = rightNowDoDamageAmount;
				var playerController = hit.collider.gameObject.GetComponent<PlayerController>();
				if (playerController == null)
				{
					// Not a player... ignore...
					return;
				}

				HitMessage hitMessage = new HitMessage();
				hitMessage.Damage = thisFrameDamage;
				hitMessage.HitType = HitType.None;
				hitMessage.KnockbackDirection = _inputManager.GetAimDirection();
				hitMessage.KnockbackValue = _laserPowerContinuousKnockbackValue;
				playerController.OnHit(hitMessage);
			}
		}

		if (_laserPowerStartedAt + _laserPowerDuration <= Time.time)
		{
			// End the power.
			_lineRenderer.enabled = false;
			_laserPowerStarted = false;
			_laserPowerAvailableAt = Time.time + _laserPowerDelay;
		}
	}

	public void StartPower2 ()
	{
		if (_exchangePowerAvailableAt > Time.time)
		{
			return;
		}

		// Exchange places with another player.
		var nearestPlayer = GameObject.FindGameObjectsWithTag("Player").Select((plGO) =>
		{
			return plGO;
		}).OrderBy((pl) =>
		{
			return Vector3.Distance(pl.transform.position, transform.position);
		}).FirstOrDefault((pl) =>
		{
			return pl != gameObject && Vector3.Distance(pl.transform.position, transform.position) <= _exchangePowerMaxDistance;
		});

		_exchangePowerAvailableAt = Time.time + _exchangePowerDelay;

		if (nearestPlayer == null)
		{
			return;
		}

		var currentPosition = transform.position;
		transform.position = nearestPlayer.transform.position;
		nearestPlayer.transform.position = currentPosition;
	}

	// Use this for initialization
	void Start ()
	{
		_inputManager = GetComponent<PlayerInputManager>();
	}

	// Update is called once per frame
	void Update ()
	{
		HandleLaserPower();
	}
}
