﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingMissile : MonoBehaviour
{
	// Character which fired the missile
	private GameObject Owner;
	private Rigidbody _rb;
	// All potential targets
	private List<Transform> _characters = new List<Transform>();
	private float _destroyTimeStamp = 0;

	// Distance in Unity units, speeds in unknown units

	// If distance to any targer is larger that this, then missile will not follow
	public float DistanceToFollow;
	public float AngleChangingSpeed;
	public float MovementSpeed;
	public ParticleSystem ExplosionParticles;

	public float ExplosionRadius;
	public int KnockbackStrength;
	public float Damage;

	// For now the best way to pass information from one object to another
	public void Initialize(GameObject owner)
	{
		Owner = owner as GameObject;

		// Get all other characters to follow
		var characters = GameObject.FindGameObjectsWithTag("Player");

		foreach(GameObject c in characters)
		{
			var character = c.transform;

			if(character.gameObject != Owner)
			{
				_characters.Add(character);
			}
		}
	}

	// Use this for initialization
	void Start ()
	{
		_rb = GetComponent<Rigidbody>();
	}

	void FixedUpdate ()
    {
		if(_destroyTimeStamp > 0)
		{
			if(Time.time >= _destroyTimeStamp + 5)
			{
				Destroy(transform.gameObject);
			}

			return;
		}


		Transform target = null;

		foreach(Transform c in _characters)
		{
			if(c == null || c.gameObject == null)
				continue;

			if(!target ||
			Vector3.Distance(transform.position, c.position) <
			Vector3.Distance(transform.position, target.position))
			{
				target = c;
			}
		}

		if(!target || DistanceToFollow < Vector3.Distance(transform.position, target.position))
		{
			FlyStraight();
		}
		else
		{
			FlyStraight();
			FollowTarget(target.GetComponent<Collider>().bounds.center);
		}
    }

	void OnTriggerEnter(Collider other)
	{
		if(other.transform.root.gameObject == Owner || other.CompareTag("Missile") || other.CompareTag("Boundary") || _destroyTimeStamp > 0)
		{
			return;
		}

		var missilePosition = transform.position;

		// Hit players with explosion if necessary
		foreach(Transform t in _characters)
		{
			if(t == null || t.gameObject == null)
				continue;

			var target = t.GetComponent<Collider>().bounds.center;
			var distance = Vector3.Distance(missilePosition, target);

			if(distance < ExplosionRadius)
			{
				var factor = 1 - distance / ExplosionRadius;
				var hitMessage = new HitMessage();

				hitMessage.Damage = (int)(Damage * factor);
				hitMessage.KnockbackValue = (int)(KnockbackStrength * factor);
				hitMessage.KnockbackDirection = (target - missilePosition).normalized;

				t.gameObject.GetComponent<PlayerController>().OnHit(hitMessage);
			}
		}

		ExplosionParticles.Play();
		GetComponent<MeshRenderer>().enabled = false;

		_destroyTimeStamp = Time.time;
	}


	private void FlyForward()
	{
		_rb.velocity = transform.forward * MovementSpeed;
	}

	private void FlyStraight()
	{
		_rb.angularVelocity = new Vector3(0, 0, 0);
		FlyForward();
	}

	private void FollowTarget(Vector3 target)
	{
		var target2D = new Vector2(target.x, target.y);
		var rocket2D = new Vector2(transform.position.x, transform.position.y);
		var rocketRotation2D = new Vector2(transform.forward.x, transform.forward.y);

		Vector2 aimVector = (target2D - rocket2D).normalized;
		var angle = Vector2.SignedAngle(aimVector, rocketRotation2D);

		_rb.angularVelocity = new Vector3(0, 0, AngleChangingSpeed * Mathf.Clamp(-angle, -1, 1));

		FlyForward();
	}
}
