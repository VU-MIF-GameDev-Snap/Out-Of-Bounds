using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingMissile : MonoBehaviour
{
	private GameObject Owner;
	private Rigidbody _rb;
	private List<Transform> _characters = new List<Transform>();

	public float distanceToFollow;
	public float angleChangingSpeed;
	public float movementSpeed;

	public void Initialize(object owner)
	{
		if(owner as GameObject)
		{
			Owner = owner as GameObject;
		}

		// Get all other characters to follow
		var characters = GameObject.FindGameObjectsWithTag("Character");

		foreach(GameObject c in characters)
		{
			var character = c.transform.parent.parent;

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
		Transform target = null;

		foreach(Transform c in _characters)
		{
			if(!target || 
			Vector3.Distance(transform.position, c.position) <
			Vector3.Distance(transform.position, target.position))
			{
				target = c;
			}
		}

		if(!target || distanceToFollow < Vector3.Distance(transform.position, target.position))
		{
			FlyStraight();
		}
		else
		{
			FlyStraight();
			FollowTarget(target.GetComponent<Collider>().bounds.center);
		}
    }

	private void FlyForward()
	{
		_rb.velocity = transform.forward * movementSpeed;
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

		_rb.angularVelocity = new Vector3(0, 0, angleChangingSpeed * Mathf.Clamp(-angle, -1, 1));
		
		FlyForward();
	}
}
