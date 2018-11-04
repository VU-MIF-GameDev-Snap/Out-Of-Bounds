using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Homing_missile : MonoBehaviour
{

	private Rigidbody _rb;
	private List<Transform> _characters = new List<Transform>();

	public float distanceToFollow;
	public float angleChangingSpeed;
	public float movementSpeed;
	public GameObject owner;

	// Use this for initialization
	void Start ()
	{
		_rb = GetComponent<Rigidbody>();

		// Get all other characters to follow
		var characters = GameObject.FindGameObjectsWithTag("Character");
		
		foreach(GameObject c in characters)
		{
			var character = c.transform.parent.parent;

			if(character.gameObject != owner)
			{
				_characters.Add(character);
			}
		}
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
			flyStraight();
		}
		else
		{
			flyStraight();
			followTarget(target.GetComponent<Collider>().bounds.center);
		}
    }

	private void flyForward()
	{
		_rb.velocity = transform.forward * movementSpeed;
	}

	private void flyStraight()
	{
		_rb.angularVelocity = new Vector3(0, 0, 0);
		flyForward();
	}

	private void followTarget(Vector3 target)
	{
		var target2D = new Vector2(target.x, target.y);
		var rocket2D = new Vector2(transform.position.x, transform.position.y);
		var rocketRotation2D = new Vector2(transform.forward.x, transform.forward.y);

		Vector2 aimVector = (target2D - rocket2D).normalized;
		var angle = Vector2.SignedAngle(aimVector, rocketRotation2D);

		_rb.angularVelocity = new Vector3(0, 0, angleChangingSpeed * Mathf.Clamp(-angle, -1, 1));
		
		flyForward();
	}
}
