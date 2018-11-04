using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Homing_missile : MonoBehaviour {

	private List<Transform> _characters = new List<Transform>();

	public float distanceToFollow;
	public float angleChangingSpeed;
	public float movementSpeed;
	public GameObject owner;

	// Use this for initialization
	void Start () {
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
			followTarget(target);
		}
    }

	private void flyStraight() {
		var rb = GetComponent<Rigidbody>();

		rb.velocity = transform.forward * movementSpeed;
	}

	private void followTarget(Transform target) {
		Vector3 aimVector = target.position - transform.position;
		Quaternion newRotation = Quaternion.LookRotation(aimVector, transform.up);
		transform.rotation = Quaternion.RotateTowards(transform.rotation, newRotation, Time.deltaTime * angleChangingSpeed);
		transform.eulerAngles = new Vector3(transform.eulerAngles.x, 270, 0);
		//direction.y = 0;
		//direction.z = 0;
		//var lookRotation = Quaternion.LookRotation(direction);
		//lookRotation = Quaternion.Euler(lookRotation.eulerAngles.x, transform.eulerAngles.y, 0);


		//var yCoord = transform.rotation.eulerAngles.y;
		//var temp = Quaternion.Slerp (transform.rotation, lookRotation, Time.deltaTime * angleChangingSpeed);
		//transform.rotation = Quaternion.Euler(temp.eulerAngles.x, transform.rotation.eulerAngles.y, 0f);
		//transform.eulerAngles = new Vector3(transform.rotation.eulerAngles.x, yCoord, 0);
		
		flyStraight();
	}
}
